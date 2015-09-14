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
			//TODO: change back to practice difficulty once a difficulty-selection system is decided on.
			//Trial one = new Trial(false, Trial.practiceDifficulty);		//non-stim for practice
			//Trial two = new Trial(false, Trial.practiceDifficulty);		//non-stim for practice
			Trial one = new Trial(false, ExperimentSettings_CoinTask.difficultySetting);		//non-stim for practice
			Trial two = new Trial(false, ExperimentSettings_CoinTask.difficultySetting);		//non-stim for practice
			practiceBlock = new Block(one, two);
		}

		for(int i = 0; i < Config_CoinTask.numBlocks/2; i++){
			Trial one = new Trial(true, ExperimentSettings_CoinTask.difficultySetting);			//stim
			Trial two = new Trial(false, ExperimentSettings_CoinTask.difficultySetting);			//non-stim
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
		yield return StartCoroutine( exp.player.controls.SmoothMoveTo(trial.avatarStartPos, trial.avatarStartRot, PlayerControls.toStartTime) );

		//exp.player.TurnOnVisuals (true);

		exp.environmentController.myGrid.Clear ();

		//exp.environmentController.myGrid.SetConfiguration (trial.DefaultObjectGridIndices, trial.SpecialObjectIndices);
		exp.objectController.SpawnDefaultObjects (trial.DefaultObjectLocationsXZ, trial.SpecialObjectLocationsXZ);

		//turn off grid visibility
		exp.environmentController.myGrid.TurnOnTileVisibility(false);
		//disable selection
		exp.environmentController.myPositionSelector.EnableSelection(false);

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
		int numDefaultObjectsToCollect = trial.DefaultObjectLocationsXZ.Count;

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
		yield return StartCoroutine( exp.player.controls.SmoothMoveTo (trial.avatarTowerPos, trial.avatarTowerRot, PlayerControls.toTowerTime) );

		//show instructions for location selection 
		yield return StartCoroutine (exp.ShowSingleInstruction ("Use the joystick to select the location of the requested object.", true, true, Config_CoinTask.minDefaultInstructionTime));

		//ask player to locate each object individually on the grid
		int memoryScore = 0;
		List<int> randomSpecialObjectOrder = GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order
		List<EnvironmentPositionSelector.SelectionRadiusType> chosenSelectorSizes = new List<EnvironmentPositionSelector.SelectionRadiusType> (); //chosen sizes will be in the same order as the random special object order
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {

			//show instructions for location selection
			int randomOrderIndex = randomSpecialObjectOrder[i];
			SpawnableObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			string specialItemName = specialObj.GetName();


			string selectObjectText = "Select the location of the " + specialItemName + ".";

			exp.environmentController.myPositionSelector.EnableSelection (true);


			//show instructions and wait for selection button press
			Debug.Log("Should wait for button press");
			yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, Config_CoinTask.minDefaultInstructionTime));

			//TODO: log the chosen position

			//TODO: log correct position

			//add current chosen position to list of chosen positions
			chosenPositions.Add(exp.environmentController.myPositionSelector.GetSelectorPosition());
			chosenSelectorSizes.Add(exp.environmentController.myPositionSelector.currentRadiusType);

		}


		//SHOW ALL FEEDBACK: TODO: make this its own function.
		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		for (int i = 0; i < randomSpecialObjectOrder.Count; i++){
			int randomOrderIndex = randomSpecialObjectOrder[i];

			//turn on each special object & scale up for better visibility
			SpawnableObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			specialObj.GetComponent<SpawnableObject>().TurnVisible(true);
			float scaleMult = 2.0f;//TODO: PUT SOMEWHERE ELSE THAT ACTUALLY MAKES SENSE. ORGANIZE THIS MESS. UGH.
			Vector3 specialObjOrigScale = specialObj.transform.localScale;
			specialObj.transform.localScale = specialObjOrigScale*scaleMult;

			//create an indicator for each special object
			float indicatorHeight = exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.position.y;
			Vector3 correctPosition = new Vector3 (specialObj.transform.position.x, indicatorHeight, specialObj.transform.position.z);
			GameObject correctPositionIndicator = Instantiate( exp.environmentController.myPositionSelector.CorrectPositionIndicator, correctPosition, exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.rotation) as GameObject;
			CorrectPositionIndicators.Add(correctPositionIndicator);
			correctPositionIndicator.GetComponentInChildren<FacePosition>().TargetPositionTransform = exp.player.transform;

			//create an indicator for each chosen position -- of the appropriate radius
			Vector3 chosenPosition = chosenPositions[i];
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;

			chosenPositionIndicator.GetComponent<VisibilityToggler>().TurnVisible(true);

			EnvironmentPositionSelector.SelectionRadiusType chosenRadiusSize = chosenSelectorSizes[i];
			if( chosenRadiusSize == EnvironmentPositionSelector.SelectionRadiusType.big ){
				chosenPositionIndicator.transform.localScale = new Vector3 ( Config_CoinTask.bigSelectionSize, chosenPositionIndicator.transform.localScale.y, Config_CoinTask.bigSelectionSize );
			} 
			else if ( chosenRadiusSize == EnvironmentPositionSelector.SelectionRadiusType.small ){
				chosenPositionIndicator.transform.localScale = new Vector3 ( Config_CoinTask.smallSelectionSize, chosenPositionIndicator.transform.localScale.y, Config_CoinTask.smallSelectionSize );
			} 
			else {
				chosenPositionIndicator.SetActive(false);
			}
			ChosenPositionIndicators.Add(chosenPositionIndicator);

			//render a line between each special object and its corresponding chosen position
			LineRenderer positionConnector = correctPositionIndicator.GetComponent<LineRenderer>();
			if(chosenRadiusSize != EnvironmentPositionSelector.SelectionRadiusType.none){
				float lineHeight = correctPosition.y;
				if(chosenPosition.y > correctPosition.y){
					lineHeight = chosenPosition.y;
				}
				lineHeight += 0.3f; //0.3f is arbitrary...
				Vector3 chosenLineHeightVec = new Vector3(chosenPosition.x, lineHeight, chosenPosition.z); //height amount is arbitrary...
				Vector3 correctLineHeightVec = new Vector3(correctPosition.x, lineHeight, correctPosition.z); //height amount is arbitrary...

				positionConnector.SetPosition(0, chosenLineHeightVec);
				positionConnector.SetPosition(1, correctLineHeightVec);

			}
			else {
				positionConnector.SetPosition(0, Vector3.zero);
				positionConnector.SetPosition(1, Vector3.zero);
			}

			//calculate the memory points and display them
			exp.environmentController.myPositionSelector.PositionSelector.transform.position = chosenPosition;
			exp.environmentController.myPositionSelector.SetRadiusSize( chosenRadiusSize );
			int points = exp.scoreController.CalculateMemoryPoints( specialObj.transform.position );
			correctPositionIndicator.GetComponentInChildren<TextMesh>().text = "+ " + points + "!";
			memoryScore += points;

			//set the position selector back to big or small -- otherwise it will be invisible when cloned in the next iteration of indicator creation
			exp.environmentController.myPositionSelector.SetRadiusSize( EnvironmentPositionSelector.SelectionRadiusType.small );
		}

		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection(false);

		//wait for button press
		//after object location has been chosen, show them how close they were / give them points
		yield return StartCoroutine (exp.ShowSingleInstruction ("Press the button to continue.", false, true, Config_CoinTask.minDefaultInstructionTime));


		//turn off grid visibility
		exp.environmentController.myGrid.TurnOnTileVisibility(false);

		//once all objects have been located, tell them their official score based on memory and time bonus
		yield return StartCoroutine (exp.ShowSingleInstruction ("You scored " + memoryScore + " memory points and a " + timeBonus + " point time bonus! Continue to the next trial.", true, true, Config_CoinTask.minDefaultInstructionTime));


		//delete all indicators
		int numIndicators = CorrectPositionIndicators.Count;
		for (int i = 0; i < numIndicators; i++) {
			Destroy (CorrectPositionIndicators[i]);

			//should be the same # of correct indicators as chosen position indicators
			//can delete both in same loop
			Destroy (ChosenPositionIndicators[i]);
		}
		CorrectPositionIndicators.Clear ();
		ChosenPositionIndicators.Clear ();


		//clear the special objects
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects[i].gameObject;
			Destroy(specialObj); //destroy the special objects
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
