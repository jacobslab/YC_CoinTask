using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public SimpleTimer trialTimer;
	public QuestionUI doYouRememberUI;
	public QuestionUI doubleDownUI;

	TrialLogTrack trialLogger;

	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numDefaultObjectsCollected = 0;

	int timeBonus = 0;
	int memoryScore = 0;

	Trial currentTrial;
	Trial practiceTrial;


	List<Trial> TrialList;

	void Start(){
		InitTrials ();
		trialLogger = GetComponent<TrialLogTrack> ();
	}

	void InitTrials(){
		TrialList = new List<Trial> ();

		Vector3 specialObjDistribution = GetSpecialObjectDistribution ();

		if(Config_CoinTask.doPracticeTrial){
			practiceTrial = new Trial(Config_CoinTask.numSpecialObjectsPract);	//2 special objects for practice trial
		}

		GenerateTrials((int)specialObjDistribution.x, 1); //one special
		GenerateTrials((int)specialObjDistribution.y, 2); //two special
		GenerateTrials((int)specialObjDistribution.z, 3); //three special

	}

	//ASSUMES NUMTRIALS IS EVEN
	void GenerateTrials(int numTrials, int numSpecial){
		for(int j = 0; j < numTrials / 2; j++){ //divide by two because we are adding a trial and a counter trial at the same time
			Trial one = new Trial(numSpecial);
			
			Trial counterOne = one.GetCounterSelf();
			
			TrialList.Add(one);
			TrialList.Add(counterOne);
		}

		if ((float)numTrials % 2.0f != 0) {
			Debug.Log("ODD NUMBER OF TRIALS.");
		}
	}

	Vector3 GetSpecialObjectDistribution(){
		//dist.x = 1 object trials, dist.y = 2 object trials, dist.z = 3 object trials
		Vector3 distribution = Vector3.zero;

		int numTestTrials = Config_CoinTask.numTestTrials;

		int numTrialsLeft = 0; //number of trials to assign to 1 or 3 objects

		if ((float)numTestTrials % 2.0f == 0) {
			distribution.y = numTestTrials / 2;
			numTrialsLeft = numTestTrials / 2;
		} 
		else {
			Debug.Log("TRIAL DISTRIBUTION WILL NOT WORK PROPERLY.");
			/*distribution.y = (numTestTrials + 1) / 2;
			numTrialsLeft = numTestTrials - (int)distribution.y;*/
		}
		
		if( (numTrialsLeft % 2) == 0 ){
			distribution.x = numTrialsLeft / 2;
			distribution.z = numTrialsLeft / 2;
		}
		else {
			Debug.Log("TRIAL DISTRIBUTION WILL NOT WORK PROPERLY.");
			/*int fiftyFiftyChance = Random.Range (0, 2); //will pick 1 or 0
			if (fiftyFiftyChance == 0) { //more 1 trials
				distribution.x = ( (numTrialsLeft - 1) / 2 ) + 1;
				distribution.z = ( (numTrialsLeft - 1) / 2 );
			}
			else { //more 3 trials
				distribution.x = ( (numTrialsLeft - 1) / 2 );
				distribution.z = ( (numTrialsLeft - 1) / 2 ) + 1;
			}*/
		}

		return distribution;
	}

	Trial PickRandomTrial(){
		if (TrialList.Count > 0) {
			int randomTrialIndex = Random.Range (0, TrialList.Count);
			Trial randomTrial = TrialList [randomTrialIndex];

			TrialList.RemoveAt (randomTrialIndex);
			return randomTrial;
		} 
		else {
			Debug.Log("No more trials left!");
			return null;
		}
	}



	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings_CoinTask.isReplay) {
			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent();
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions, true, true, Config_CoinTask.minInitialInstructionsTime));

			//let player explore until the button is pressed again
			trialLogger.LogBeginningExplorationEvent();
			yield return StartCoroutine (exp.WaitForActionButton ());
			
			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings_CoinTask.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if (Config_CoinTask.doPracticeTrial) {
				if (numRealTrials >= 2) { //otherwise, leave numRealTrials at zero.
					numRealTrials -= 1; //-1 for practice trial
				}
			}

			
			//run practice trials
			isPracticeTrial = true;
			
			if (isPracticeTrial && Config_CoinTask.doPracticeTrial) {

				yield return StartCoroutine (RunTrial ( practiceTrial ));

				Debug.Log ("PRACTICE TRIALS COMPLETED");
				totalTrialCount += 1;
				isPracticeTrial = false;
			}


			//RUN THE REST OF THE BLOCKS
			while (TrialList.Count > 0) {
				Trial nextTrial = PickRandomTrial ();

				yield return StartCoroutine (RunTrial ( nextTrial ));

				totalTrialCount += 1;

				Debug.Log ("TRIALS COMPLETED: " + totalTrialCount);
			}

			yield return 0;
		}
		
	}

	public void IncrementNumDefaultObjectsCollected(){
		numDefaultObjectsCollected++;
		if (ExperimentSettings_CoinTask.isOneByOneReveal) {
			CreateNextDefaultObject ( numDefaultObjectsCollected );
		}
	}

	void CreateNextDefaultObject ( int index ){
		if (currentTrial != null) {
			SetUpNextDefaultObject (index);
			if (index < currentTrial.DefaultObjectLocationsXZ.Count) {

				Vector2 positionXZ = currentTrial.DefaultObjectLocationsXZ [index];
				exp.objectController.SpawnDefaultObject (positionXZ, currentTrial.SpecialObjectLocationsXZ, index);
			} else {
				Debug.Log ("Can't create a default object at that index. Index is too big.");
			}
		}
	}

	//will put the default object most in the player's field of view at the next position in the default object position list
	void SetUpNextDefaultObject (int currentIndex){
		//look through the objects from the current index to the end of the list -- only these haven't been spawned yet
		float minAngleBetweenChestAndPlayer = 0;
		int minIndex = currentIndex;

		for (int i = currentIndex; i < currentTrial.DefaultObjectLocationsXZ.Count; i++) {
			Vector2 currChestPositionXZ = currentTrial.DefaultObjectLocationsXZ [i];
			float angleBetweenChestAndPlayer = exp.player.controls.GetYAngleBetweenFacingDirAndObjectXZ( currChestPositionXZ ) ; //TODO: CALCULATE THIS.
			if (i == currentIndex){
				minAngleBetweenChestAndPlayer = angleBetweenChestAndPlayer;
			}
			else {
				if(minAngleBetweenChestAndPlayer > angleBetweenChestAndPlayer){
					minAngleBetweenChestAndPlayer = angleBetweenChestAndPlayer;
					minIndex = i;
				}
			}
		}

		//now that we have the index, let's swap the current index's value with the min index's value
		if (minIndex != currentIndex) {
			Vector2 tempCurrPosition = currentTrial.DefaultObjectLocationsXZ[currentIndex];
			currentTrial.DefaultObjectLocationsXZ[currentIndex] = currentTrial.DefaultObjectLocationsXZ[minIndex];
			currentTrial.DefaultObjectLocationsXZ[minIndex] = tempCurrPosition;
		}

	}

	bool questionAnswer = false;
	public void SetQuestionAnswer(bool answer){ //SET IN EXPERIMENT. TODO: REORGANIZE?!
		questionAnswer = answer;
	}

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	IEnumerator RunTrial(Trial trial){

		currentTrial = trial;

		if (isPracticeTrial) {
			trialLogger.Log (-1, currentTrial.DefaultObjectLocationsXZ.Count, currentTrial.SpecialObjectLocationsXZ.Count, ExperimentSettings_CoinTask.isOneByOneReveal);
			Debug.Log("Logged practice trial.");
		} 
		else {
			trialLogger.Log (numRealTrials, currentTrial.DefaultObjectLocationsXZ.Count, currentTrial.SpecialObjectLocationsXZ.Count, ExperimentSettings_CoinTask.isOneByOneReveal);
			numRealTrials++;
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		//move player to home location & rotation
		trialLogger.LogTransportationToHomeEvent ();
		yield return StartCoroutine( exp.player.controls.SmoothMoveTo(currentTrial.avatarStartPos, currentTrial.avatarStartRot, PlayerControls.toStartTime) );

		//exp.player.TurnOnVisuals (true);

		if (ExperimentSettings_CoinTask.isOneByOneReveal) {
			//Spawn the first default object
			CreateNextDefaultObject (0);
			//THE FOLLOWING IS FOR DEBUGGING PURPOSES.
			//exp.objectController.SpawnDefaultObjects (currentTrial.DefaultObjectLocationsXZ, currentTrial.SpecialObjectLocationsXZ);
		}
		else{
			exp.objectController.SpawnDefaultObjects (currentTrial.DefaultObjectLocationsXZ, currentTrial.SpecialObjectLocationsXZ);
		}


		//disable selection
		exp.environmentController.myPositionSelector.EnableSelection(false);

		exp.player.controls.ShouldLockControls = true;

		exp.instructionsController.EnableScoreInstructions(true);
		trialLogger.LogInstructionEvent ();
		yield return StartCoroutine (exp.ShowSingleInstruction ("Drive around and collect all of the coins. Pay attention to the surprise object locations!"+
		                                                        "\n\nFinish quickly enough and you will receive a time bonus on your score!", true, true, Config_CoinTask.minDefaultInstructionTime));
		exp.instructionsController.EnableScoreInstructions(false);
		trialLogger.LogTrialNavigationStarted ();

		//start a game timer
		trialTimer.ResetTimer ();
		trialTimer.StartTimer ();

		//unlock avatar controls, wait for player to collect all default objects
		exp.player.controls.ShouldLockControls = false;
		int numDefaultObjectsToCollect = currentTrial.DefaultObjectLocationsXZ.Count;

		while (numDefaultObjectsCollected < numDefaultObjectsToCollect) {
			yield return 0;
		}

		//Add time bonus
		trialTimer.StopTimer ();
		timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSeconds());
		//TODO: do nice animation for adding time bonus...

		//reset num default objects collected
		numDefaultObjectsCollected = 0;

		//lock player movement
		exp.player.controls.ShouldLockControls = true;

		//bring player to tower
		//exp.player.TurnOnVisuals (false);
		trialLogger.LogTransportationToTowerEvent ();
		yield return StartCoroutine( exp.player.controls.SmoothMoveTo (currentTrial.avatarTowerPos, currentTrial.avatarTowerRot, PlayerControls.toTowerTime) );

		//show instructions for location selection 
		trialLogger.LogRecallPhaseStarted();
		trialLogger.LogInstructionEvent ();
		yield return StartCoroutine (exp.ShowSingleInstruction ("Use the joystick to select the location of the requested object.", true, true, Config_CoinTask.minDefaultInstructionTime));

		//ask player to locate each object individually
		List<int> randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order
		List<bool> doubleDownResponses = new List<bool> (); //keep track of whether or not the player wanted to double down on each object
		List<EnvironmentPositionSelector.SelectionRadiusType> chosenSelectorSizes = new List<EnvironmentPositionSelector.SelectionRadiusType> (); //chosen sizes will be in the same order as the random special object order
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {

			//show instructions for location selection
			int randomOrderIndex = randomSpecialObjectOrder[i];
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject>();
			string specialItemName = specialSpawnable.GetName();


			//show nice UI
			trialLogger.LogObjectToRecall(specialSpawnable);
			GameObject specialObjUICopy = Instantiate (specialObj, Vector3.zero, specialObj.transform.rotation) as GameObject;

			//set layer of object & children to PlayerUI
			specialObjUICopy.GetComponent<SpawnableObject>().SetLayer ("PlayerUI");

			yield return StartCoroutine( doYouRememberUI.Play(specialObjUICopy) );

			yield return StartCoroutine (exp.WaitForYesNoResponse());
			trialLogger.LogRememberResponse(questionAnswer); //question answer gets set with WaitForYesNoResponse...

			//enable position selection, turn off fancy selection UI
			exp.environmentController.myPositionSelector.EnableSelection (true);
			exp.environmentController.myPositionSelector.SetRadiusSize( EnvironmentPositionSelector.SelectionRadiusType.big ); //always start with the big selector. this choice was fairly arbitrary.
			doYouRememberUI.Stop();

			//show single selection instruction and wait for selection button press
			string selectObjectText = "Select the location of the " + specialItemName + ".";
			yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, Config_CoinTask.minDefaultInstructionTime));

			//log the chosen position and correct position
			exp.environmentController.myPositionSelector.logTrack.LogPositionChosen( exp.environmentController.myPositionSelector.GetSelectorPosition(), specialObj.transform.position, specialSpawnable );

			//add current chosen position to list of chosen positions
			chosenPositions.Add(exp.environmentController.myPositionSelector.GetSelectorPosition());
			chosenSelectorSizes.Add(exp.environmentController.myPositionSelector.currentRadiusType);

			//disable position selection
			exp.environmentController.myPositionSelector.EnableSelection (false);

			yield return StartCoroutine( doubleDownUI.Play() );

			yield return StartCoroutine (exp.WaitForYesNoResponse());
			trialLogger.LogDoubleDownResponse(questionAnswer); //question answer gets set with WaitForYesNoResponse...

			doubleDownResponses.Add(questionAnswer);
			doubleDownUI.Stop();
		}

		trialLogger.LogFeedbackStarted();
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
			SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject>();
			specialSpawnable.TurnVisible(true);
			specialSpawnable.Scale(2.0f);
			UsefulFunctions.FaceObject( specialObj, exp.player.gameObject, false);
			
			//create an indicator for each special object
			float indicatorHeight = exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.position.y;
			Vector3 correctPosition = new Vector3 (specialObj.transform.position.x, indicatorHeight, specialObj.transform.position.z);
			GameObject correctPositionIndicator = Instantiate( exp.environmentController.myPositionSelector.CorrectPositionIndicator, correctPosition, exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.rotation) as GameObject;
			correctPositionIndicator.GetComponent<SpawnableObject>().SetNameID(i);
			CorrectPositionIndicators.Add(correctPositionIndicator); 
			
			//create an indicator for each chosen position -- of the appropriate radius
			//spawn the indicator at the height of the original indicator
			exp.environmentController.myPositionSelector.EnableSelection (true); //turn on selector for spawning indicator
			Vector3 chosenIndicatorPosition = new Vector3(chosenPosition.x, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y, chosenPosition.z);
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;

			chosenPositionIndicator.GetComponent<SpawnableObject>().SetNameID(i);
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
			exp.environmentController.myPositionSelector.SetRadiusSize( EnvironmentPositionSelector.SelectionRadiusType.big );
		}
		
		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection(false);
		
		//wait for selection button press
		yield return StartCoroutine (exp.ShowSingleInstruction ("Press the button to continue.", false, true, Config_CoinTask.minDefaultInstructionTime));
		
		//once all objects have been located, tell them their official score based on memory and time bonus, wait for button press
		trialLogger.LogInstructionEvent ();
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

			correctPositionIndicator.GetComponent<CorrectPositionIndicatorController>().SetLineTarget(chosenPosition);
			
		}
		else {
			correctPositionIndicator.GetComponent<CorrectPositionIndicatorController>().SetLineTarget(correctPosition);
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
