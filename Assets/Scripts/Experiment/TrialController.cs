using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public SimpleTimer trialTimer;


	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numDefaultObjectsCollected = 0;

	private class Block{
		public Trial trial1;
		public Trial trial2;

		public Block(Trial one, Trial two){
			trial1 = one;
			trial2 = two;
		}
	}

	Block practiceBlock; //for use when Config.doPracticeBlock == true!


	List<Block> BlockList;

	void Start(){
		InitTrials ();
	}

	void InitTrials(){
		BlockList = new List<Block> ();

		if (Config_CoinTask.doPracticeBlock) {
			Trial one = new Trial(false, Trial.practiceDifficulty);		//non-stim for practice
			Trial two = new Trial(false, Trial.practiceDifficulty);		//non-stim for practice
			practiceBlock = new Block(one, two);
		}

		for(int i = 0; i < Config_CoinTask.numBlocks/2; i++){
			Trial one = new Trial(true, Trial.DifficultySetting.easy);			//stim
			Trial two = new Trial(false, Trial.DifficultySetting.easy);			//non-stim
			Block block = new Block(one, two);

			Trial counterOne = one.GetCounterSelf();//non-stim
			Trial counterTwo = two.GetCounterSelf();//stim
			Block counterBlock = new Block(counterTwo, counterOne); //add in opposite order to ensure that the [stim, non-stim] pattern is consistent across all blocks

			BlockList.Add(block);
			BlockList.Add(counterBlock);
		}

	}

	Block PickRandomBlock(){
		if (BlockList.Count > 0) {
			int randomBlockIndex = Random.Range (0, BlockList.Count);
			Block randomBlock = BlockList [randomBlockIndex];

			BlockList.RemoveAt (randomBlockIndex);
			return randomBlock;
		} 
		else {
			Debug.Log("No more blocks left!");
			return null;
		}
	}



	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings_CoinTask.isReplay) {
			//show instructions for exploring
			//yield return StartCoroutine (exp.ShowSingleInstruction ("Use the arrow keys to explore the environment. When finished exploring, press the button.", true, true, 0.0f));
			
			//let player explore
			//yield return StartCoroutine (exp.WaitForActionButton ());
			
			
			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings_CoinTask.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if (Config_CoinTask.doPracticeBlock) {
				if (numRealTrials >= 2) { //otherwise, leave numRealTrials at zero.
					numRealTrials -= Config_CoinTask.numTestTrialsPract;
				}
			}

			
			//run practice trials
			isPracticeTrial = true;
			
			if (isPracticeTrial && Config_CoinTask.doPracticeBlock) {

				yield return StartCoroutine (RunTrial ( practiceBlock.trial1 ));

				yield return StartCoroutine (RunTrial ( practiceBlock.trial2 ));

				Debug.Log ("PRACTICE TRIALS COMPLETED");
				totalTrialCount += 2;
				isPracticeTrial = false;
			}


			//RUN THE REST OF THE BLOCKS
			while (BlockList.Count > 0) {
				Block nextBlock = PickRandomBlock ();

				yield return StartCoroutine (RunTrial ( nextBlock.trial1 ));

				yield return StartCoroutine (RunTrial ( nextBlock.trial2 ));

				totalTrialCount += 2;

				Debug.Log ("TRIALS COMPLETED: " + totalTrialCount);
			}

			yield return 0;
		}
		
	}

	public void IncrementNumDefaultObjectsCollected(){
		numDefaultObjectsCollected++;
	}

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	IEnumerator RunTrial(Trial trial){

		if (isPracticeTrial) {
			GetComponent<TrialLogTrack> ().Log (-1, trial.isStim);
			Debug.Log("Logged practice trial.");
		} 
		else {
			GetComponent<TrialLogTrack> ().Log (numRealTrials, trial.isStim);
			numRealTrials++;
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		Debug.Log ("IS STIM: " + trial.isStim);

		//move player to first location & rotation
		exp.player.controls.SmoothMoveToPos(trial.avatarStartPos, trial.avatarStartRot);

		//exp.player.TurnOnVisuals (true);

		exp.environmentController.myGrid.Clear ();

		exp.environmentController.myGrid.SetConfiguration (trial.DefaultObjectGridIndices, trial.SpecialObjectIndices);
		exp.objectController.SpawnDefaultObjects (trial.DefaultObjectGridIndices);

		//turn off grid visibility
		exp.environmentController.myGrid.TurnOnTileVisibility(false);
		//disable grid selection
		exp.player.tileSelector.Disable(true);

		exp.player.controls.ShouldLockControls = true;

		exp.instructionsController.EnableScoreInstructions(true);
		yield return StartCoroutine (exp.ShowSingleInstruction ("Drive around and collect all of the coins. Pay attention to the surprise object locations!"+
		                                                        "\n\nFinish quickly enough and you will receive a time bonus on your score!", true, true, Config_CoinTask.minDefaultInstructionTime));
		exp.instructionsController.EnableScoreInstructions(false);

		//start a game timer
		trialTimer.ResetTimer ();
		trialTimer.StartTimer ();

		//unlock avatar controls, wait for player to collect all coins
		exp.player.controls.ShouldLockControls = false;
		int numDefaultObjectsToCollect = trial.DefaultObjectGridIndices.Count;

		while (numDefaultObjectsCollected < numDefaultObjectsToCollect) {
			yield return 0;
		}

		//Add time bonus
		trialTimer.StopTimer ();
		int timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSeconds());
		//TODO: do nice animation for adding time bonus...

		//reset num coins collected
		numDefaultObjectsCollected = 0;

		//lock player movement
		exp.player.controls.ShouldLockControls = true;

		//bring player to tower
		//exp.player.TurnOnVisuals (false);
		exp.player.controls.SmoothMoveToTower ();

		//show instructions for location selection 
		yield return StartCoroutine (exp.ShowSingleInstruction ("You will now be shown the environment as a grid. Use the joystick to select the location of the requested object.", true, true, Config_CoinTask.minDefaultInstructionTime));

		//ask player to locate each object individually on the grid
		int memoryScore = 0;
		List<int> randomSpecialObjectOrder = GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {
			//turn on grid visibility
			exp.environmentController.myGrid.TurnOnTileVisibility(true);
			//enable grid selection
			exp.player.tileSelector.Enable ();

			//show instructions for location selection -- TODO: use the image of the object instead?
			int randomOrderIndex = randomSpecialObjectOrder[i];
			SpawnableObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			string specialItemName = specialObj.GetName();
			//show instructions and wait for selection button press
			yield return StartCoroutine (exp.ShowSingleInstruction ("Select the location of the " + specialItemName + ".", false, true, Config_CoinTask.minDefaultInstructionTime));

			//log the chosen tile
			Tile chosenTile = exp.player.tileSelector.selectedTile;
			exp.environmentController.myGrid.MyGridLogTrack.LogGridTile(chosenTile, GridLogTrack.LoggedTileType.chosenTestTile);

			Tile correctTile = exp.environmentController.myGrid.GetGridTile(specialObj.GetComponent<GridItem>().rowIndex, specialObj.GetComponent<GridItem>().colIndex);
			//log correct tile
			exp.environmentController.myGrid.MyGridLogTrack.LogGridTile(correctTile, GridLogTrack.LoggedTileType.correctTestTile);
			//light up correct tile
			correctTile.myHighlighter.HighlightHigh();
			correctTile.myHighlighter.SetSpecialColor(Color.green);

			//make object visible on the field, up the scale, move upward for better visibility
			specialObj.GetComponent<SpawnableObject>().TurnVisible(true);
			float scaleMult = 3.5f;//TODO: PUT SOMEWHERE ELSE THAT ACTUALLY MAKES SENSE. ORGANIZE THIS MESS. UGH.
			Vector3 liftVector = new Vector3(0.0f, 1.0f, 0.0f); //for better visibility
			Vector3 specialObjOrigScale = specialObj.transform.localScale;
			specialObj.transform.localScale = specialObjOrigScale*scaleMult;
			specialObj.transform.position += liftVector;

			//Add memory score
			Vector2 chosenTileGridPos = chosenTile.GridIndices;
			Vector2 correctTileGridPos = correctTile.GridIndices;
			memoryScore += exp.scoreController.CalculateMemoryPoints(correctTileGridPos, chosenTileGridPos);

			exp.player.tileSelector.Disable(false);

			//after object location has been chosen, show them how close they were / give them points
			yield return StartCoroutine (exp.ShowSingleInstruction ("Press the button to continue.", false, true, Config_CoinTask.minDefaultInstructionTime));
			correctTile.myHighlighter.HighlightLow();
			correctTile.myHighlighter.ResetColor();
			//make object invisible on the field, scale back down, move back to orig position
			specialObj.GetComponent<SpawnableObject>().TurnVisible(false);
			specialObj.transform.localScale = specialObjOrigScale;
			specialObj.transform.position -= liftVector;
		}

		//turn off grid visibility
		exp.environmentController.myGrid.TurnOnTileVisibility(false);

		//once all objects have been located, tell them their official score based on memory and time bonus
		yield return StartCoroutine (exp.ShowSingleInstruction ("You scored " + memoryScore + " memory points and a " + timeBonus + " point time bonus! Continue to the next trial.", true, true, Config_CoinTask.minDefaultInstructionTime));

		//clear the special objects
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {
			Destroy(exp.objectController.CurrentTrialSpecialObjects[i]); //destroy the special objects
		}
		exp.objectController.CurrentTrialSpecialObjects.Clear (); //clear the object controller's list of special objects

		yield return 0;
		
		//increment subject's trial count
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();

	}

	//given the size of an array or a list, will return a list of indices in a random order
	public List<int> GetRandomIndexOrder(int count){
		List<int> inOrderList = new List<int>();
		for(int i = 0; i < count; i++){
			inOrderList.Add(i);
		}

		List<int> randomOrderList = new List<int>();
		for(int i = 0; i < count; i++){
			int randomIndex = Random.Range(0, inOrderList.Count);
			randomOrderList.Add( inOrderList[randomIndex] );
			inOrderList.RemoveAt(randomIndex);
		}

		return randomOrderList;
	}

	public IEnumerator WaitForSpecialAnimation(GameObject specialObject){
		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		yield return StartCoroutine (exp.player.controls.RotateTowardSpecialObject (specialObject));
		yield return new WaitForSeconds (Config_CoinTask.pauseAtSpecialObjectTime);
		
		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;

		//turn the special object invisible
		specialObject.GetComponent<SpawnableObject> ().TurnVisible (false);


		//only after the pause should we increment the number of coins collected...
		//...because the trial controller waits for this to move on to the next part of the trial.
		Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();
	}
}
