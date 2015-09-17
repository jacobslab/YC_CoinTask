using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public SimpleTimer trialTimer;
	public SelectObjectUI mySelectObjectUI;


	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numDefaultObjectsCollected = 0;

	int timeBonus = 0;
	int memoryScore = 0;

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

		exp.objectController.SpawnDefaultObjects (trial.DefaultObjectLocationsXZ, trial.SpecialObjectLocationsXZ);

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
		timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSeconds());
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

		//ask player to locate each object individually
		List<int> randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order
		List<EnvironmentPositionSelector.SelectionRadiusType> chosenSelectorSizes = new List<EnvironmentPositionSelector.SelectionRadiusType> (); //chosen sizes will be in the same order as the random special object order
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {

			//show instructions for location selection
			int randomOrderIndex = randomSpecialObjectOrder[i];
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			string specialItemName = specialObj.GetComponent<SpawnableObject>().GetName();


			//show nice UI
			Debug.Log("Should wait for button press");
			GameObject specialObjUICopy = Instantiate (specialObj, Vector3.zero, specialObj.transform.rotation) as GameObject;
			yield return StartCoroutine( mySelectObjectUI.Play(specialObjUICopy) );
			yield return StartCoroutine (exp.WaitForActionButton());


			//enable position selection, turn off fancy selection UI
			exp.environmentController.myPositionSelector.EnableSelection (true);
			mySelectObjectUI.Stop();

			//show single selection instruction and wait for selection button press
			string selectObjectText = "Select the location of the " + specialItemName + ".";
			yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, Config_CoinTask.minDefaultInstructionTime));

			//TODO: log the chosen position

			//TODO: log correct position

			//add current chosen position to list of chosen positions
			chosenPositions.Add(exp.environmentController.myPositionSelector.GetSelectorPosition());
			chosenSelectorSizes.Add(exp.environmentController.myPositionSelector.currentRadiusType);

			//disable position selection
			exp.environmentController.myPositionSelector.EnableSelection (false);
		}

		yield return StartCoroutine (ShowFeedback (randomSpecialObjectOrder, chosenPositions, chosenSelectorSizes) );
		
		//increment subject's trial count
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();

	}

	IEnumerator ShowFeedback(List<int> specialObjectOrder, List<Vector3> chosenPositions, List<EnvironmentPositionSelector.SelectionRadiusType> chosenSelectorSizes){
		memoryScore = 0;

		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		
		for (int i = 0; i < specialObjectOrder.Count; i++){

			Vector3 chosenPosition = chosenPositions[i];

			//throw bomb to selected location
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't actually want its visuals showing up as we wait
			if(chosenSelectorSizes[i] != EnvironmentPositionSelector.SelectionRadiusType.none){
				yield return StartCoroutine( exp.objectController.ThrowBomb( exp.player.transform.position, chosenPosition ) );
			}

			int randomOrderIndex = specialObjectOrder[i];

			//turn on each special object & scale up for better visibility
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			specialObj.GetComponent<SpawnableObject>().TurnVisible(true);
			specialObj.GetComponent<SpawnableObject>().Scale(2.0f);
			
			//create an indicator for each special object
			float indicatorHeight = exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.position.y;
			Vector3 correctPosition = new Vector3 (specialObj.transform.position.x, indicatorHeight, specialObj.transform.position.z);
			GameObject correctPositionIndicator = Instantiate( exp.environmentController.myPositionSelector.CorrectPositionIndicator, correctPosition, exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.rotation) as GameObject;
			CorrectPositionIndicators.Add(correctPositionIndicator);
			correctPositionIndicator.GetComponentInChildren<FacePosition>().TargetPositionTransform = exp.player.transform;
			
			//create an indicator for each chosen position -- of the appropriate radius
			//spawn the indicator at the height of the original indicator
			exp.environmentController.myPositionSelector.EnableSelection (true); //turn on selector for spawning indicator
			Vector3 chosenIndicatorPosition = new Vector3(chosenPosition.x, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y, chosenPosition.z);
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;
			
			chosenPositionIndicator.GetComponent<VisibilityToggler>().TurnVisible(true);

			//scale the chosen indicators appropriately
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

			//connect the chosen and correct indicators via a line
			SetConnectingLines( correctPositionIndicator, chosenPosition, chosenRadiusSize );
			
			//calculate the memory points and display them
			exp.environmentController.myPositionSelector.PositionSelector.transform.position = chosenPosition;
			exp.environmentController.myPositionSelector.SetRadiusSize( chosenRadiusSize );
			int points = exp.scoreController.CalculateMemoryPoints( specialObj.transform.position );
			correctPositionIndicator.GetComponentInChildren<TextMesh>().text = "+" + points + "!";
			memoryScore += points;
			
			//set the position selector back to big or small -- otherwise it will be invisible when cloned in the next iteration of indicator creation
			exp.environmentController.myPositionSelector.SetRadiusSize( EnvironmentPositionSelector.SelectionRadiusType.small );
		}
		
		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection(false);
		
		//wait for button press
		yield return StartCoroutine (exp.ShowSingleInstruction ("Press the button to continue.", false, true, Config_CoinTask.minDefaultInstructionTime));
		
		//once all objects have been located, tell them their official score based on memory and time bonus, wait for button press
		yield return StartCoroutine (exp.ShowSingleInstruction ("You scored " + memoryScore + " memory points and a " + timeBonus + " point time bonus! Continue to the next trial.", true, true, Config_CoinTask.minDefaultInstructionTime));
		
		
		//delete all indicators & special objects
		DestroyGameObjectList (CorrectPositionIndicators);
		DestroyGameObjectList (ChosenPositionIndicators);
		DestroyGameObjectList (exp.objectController.CurrentTrialSpecialObjects);
		
		yield return 0;
	}

	void SetConnectingLines( GameObject correctPositionIndicator, Vector3 chosenPosition, EnvironmentPositionSelector.SelectionRadiusType chosenRadiusSize ){

		//render a line between the special object and its corresponding chosen position
		LineRenderer positionConnector = correctPositionIndicator.GetComponent<LineRenderer>();
		Vector3 correctPosition = correctPositionIndicator.transform.position;
		if(chosenRadiusSize != EnvironmentPositionSelector.SelectionRadiusType.none){
			float lineHeight = correctPosition.y;
			if(chosenPosition.y > correctPosition.y){
				lineHeight = chosenPosition.y;
			}
			lineHeight += 0.3f; //0.3f is arbitrary height
			Vector3 chosenLineHeightVec = new Vector3(chosenPosition.x, lineHeight, chosenPosition.z); //height amount is arbitrary...
			Vector3 correctLineHeightVec = new Vector3(correctPosition.x, lineHeight, correctPosition.z); //height amount is arbitrary...
			
			positionConnector.SetPosition(0, chosenLineHeightVec);
			positionConnector.SetPosition(1, correctLineHeightVec);
			
		}
		else {
			positionConnector.SetPosition(0, Vector3.zero);
			positionConnector.SetPosition(1, Vector3.zero);
		}
	}

	void DestroyGameObjectList(List<GameObject> listOfGameObjects){
		int numObjects = listOfGameObjects.Count;
		for (int i = 0; i < numObjects; i++) {
			Destroy (listOfGameObjects [i]);
		}
		listOfGameObjects.Clear ();
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
