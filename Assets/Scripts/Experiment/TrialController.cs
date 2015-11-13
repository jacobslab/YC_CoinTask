using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//paused?!
	public static bool isPaused = false;

	//TIMER
	public SimpleTimer trialTimer;

	//UI
	public QuestionUI doYouRememberUI;
	public QuestionUI areYouSureUI;
	public BlockCompleteUI blockCompletedUI;
	public ScoreRecapUI scoreRecapUI;
	public CanvasGroup PauseUI;
	public GoText goText;

	TrialLogTrack trialLogger;

	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numDefaultObjectsCollected = 0;

	int timeBonus = 0;
	int memoryScore = 0;

	Trial currentTrial;
	Trial practiceTrial;

	[HideInInspector] public GameObject currentDefaultObject; //current treasure chest we're looking for. assuming a one-by-one reveal.

	List<List<Trial>> ListOfTrialBlocks;

	void Start(){
		InitTrials ();
		trialLogger = GetComponent<TrialLogTrack> ();
	}

	void InitTrials(){
		ListOfTrialBlocks = new List<List<Trial>> ();

		int numTestTrials = Config_CoinTask.numTestTrials;

		int numTrialsPerBlock = (int)(Config_CoinTask.trialBlockDistribution [0] + Config_CoinTask.trialBlockDistribution [1] + Config_CoinTask.trialBlockDistribution [2]);

		if (numTestTrials % numTrialsPerBlock != 0) {
			Debug.Log("CANNOT EXECUTE THIS TRIAL DISTRIBUTION");
		}

		int numTrialBlocks = numTestTrials / numTrialsPerBlock;
		for (int i = 0; i < numTrialBlocks; i++) {
			ListOfTrialBlocks.Add(GenerateTrialBlock());
		}

		if(Config_CoinTask.doPracticeTrial){
			practiceTrial = new Trial(Config_CoinTask.numSpecialObjectsPract);	//2 special objects for practice trial
		}

	}

	List<Trial> GenerateTrialBlock(){
		List<Trial> trialBlock = new List<Trial> ();
		int numTrials = (int)(Config_CoinTask.trialBlockDistribution [0] + Config_CoinTask.trialBlockDistribution [1] + Config_CoinTask.trialBlockDistribution [2]);

		int numSpecial = 1;
		for (int i = 0; i < numTrials / 2; i++) { //divide by two because we're adding a regular and a counterbalanced trial

			if(i < Config_CoinTask.trialBlockDistribution[0] / 2){
				numSpecial = 1;
			}
			else if (i < ( Config_CoinTask.trialBlockDistribution[0] + Config_CoinTask.trialBlockDistribution[1] ) / 2){
				numSpecial = 2;
			}
			else{
				numSpecial = 3;
			}

			Trial trial = new Trial(numSpecial);
			Trial counterTrial = trial.GetCounterSelf();
			
			trialBlock.Add(trial);
			trialBlock.Add(counterTrial);
		}

		return trialBlock;

	}

	Trial PickRandomTrial(List<Trial> trialBlock){
		if (trialBlock.Count > 0) {
			int randomTrialIndex = Random.Range (0, trialBlock.Count);
			Trial randomTrial = trialBlock [randomTrialIndex];

			trialBlock.RemoveAt (randomTrialIndex);
			return randomTrial;
		} 
		else {
			Debug.Log("No more trials left!");
			return null;
		}
	}


	
	void Update(){
		GetPauseInput ();
	}

	bool isPauseButtonPressed = false;
	void GetPauseInput(){
		//if (Input.GetAxis ("Pause Button") > 0) {
		if(Input.GetKeyDown(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton2)){ //B JOYSTICK BUTTON TODO: move to input manager.
			Debug.Log("PAUSE BUTTON PRESSED");
			if(!isPauseButtonPressed){
				isPauseButtonPressed = true;
				Debug.Log ("PAUSE OR UNPAUSE");
				TogglePause (); //pause
			}
		} 
		else{
			isPauseButtonPressed = false;
		}
	}

	public void TogglePause(){
		isPaused = !isPaused;
		trialLogger.LogPauseEvent (isPaused);

		if (isPaused) {
			//exp.player.controls.Pause(true);
			PauseUI.alpha = 1.0f;
			Time.timeScale = 0.0f;
		} 
		else {
			Time.timeScale = 1.0f;
			//exp.player.controls.Pause(false);
			//exp.player.controls.ShouldLockControls = false;
			PauseUI.alpha = 0.0f;
		}
	}


	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings_CoinTask.isReplay) {
			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent();
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions1, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions2, true, true, false, Config_CoinTask.minInitialInstructionsTime));

			//let player explore until the button is pressed again
			//trialLogger.LogBeginningExplorationEvent();
			//yield return StartCoroutine (exp.WaitForActionButton ());
			
			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings_CoinTask.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if (Config_CoinTask.doPracticeTrial) {
				if (numRealTrials >= 2) { //otherwise, leave numRealTrials at zero.
					numRealTrials -= 1; //-1 for practice trial
				}
			}

			
			//run practice trials
			if(Config_CoinTask.doPracticeTrial){
				isPracticeTrial = true;
			}
			
			if (isPracticeTrial) {

				yield return StartCoroutine (RunTrial ( practiceTrial ));

				Debug.Log ("PRACTICE TRIALS COMPLETED");
				totalTrialCount += 1;
				isPracticeTrial = false;
			}


			//RUN THE REST OF THE BLOCKS
			for( int i = 0; i < ListOfTrialBlocks.Count; i++){
				List<Trial> currentTrialBlock = ListOfTrialBlocks[i];
				while (currentTrialBlock.Count > 0) {
					Trial nextTrial = PickRandomTrial (currentTrialBlock);

					yield return StartCoroutine (RunTrial ( nextTrial ));

					totalTrialCount += 1;

					Debug.Log ("TRIALS COMPLETED: " + totalTrialCount);
				}

				//FINISHED A TRIAL BLOCK, SHOW UI
				trialLogger.LogInstructionEvent();
				blockCompletedUI.Play(i, exp.scoreController.score, ListOfTrialBlocks.Count);
				yield return StartCoroutine(exp.WaitForActionButton());

				blockCompletedUI.Stop();

				exp.scoreController.Reset();

				Debug.Log ("TRIAL Block: " + i);
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

	void CreateNextDefaultObject ( int currentIndex ){
		if (currentTrial != null) {
			//SetUpNextDefaultObject (currentIndex);
			if (currentIndex < currentTrial.DefaultObjectLocationsXZ.Count) {

				Vector2 positionXZ = currentTrial.DefaultObjectLocationsXZ [currentIndex];
				currentDefaultObject = exp.objectController.SpawnDefaultObject (positionXZ, currentTrial.SpecialObjectLocationsXZ, currentIndex);
			} else {
				Debug.Log ("Can't create a default object at that index. Index is too big.");
			}
		}
	}
	
	void SetUpNextDefaultObject (int currentIndex){
		//look through the objects from the current index to the end of the list -- only these haven't been spawned yet
		int minIndex = currentIndex;

		/*
		//will put the default object most in the player's field of view at the next position in the default object position list
		float minAngleBetweenChestAndPlayer = 0;
		for (int i = currentIndex; i < currentTrial.DefaultObjectLocationsXZ.Count; i++) {
			Vector2 currChestPositionXZ = currentTrial.DefaultObjectLocationsXZ [i];
			float angleBetweenChestAndPlayer = exp.player.controls.GetYAngleBetweenFacingDirAndObjectXZ( currChestPositionXZ ) ;
			angleBetweenChestAndPlayer = Mathf.Abs(angleBetweenChestAndPlayer);
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
		}*/

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
		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarStartPos, currentTrial.avatarStartRot));


		if (ExperimentSettings_CoinTask.isOneByOneReveal) {
			//Spawn the first default object
			CreateNextDefaultObject (0);
		}
		else{
			exp.objectController.SpawnDefaultObjects (currentTrial.DefaultObjectLocationsXZ, currentTrial.SpecialObjectLocationsXZ);
		}


		//disable selection
		exp.environmentController.myPositionSelector.EnableSelection(false);

		exp.player.controls.ShouldLockControls = true;


		if(numRealTrials > 1 || trial.avatarStartPos != exp.player.controls.startPositionTransform1.position){ //note: if numRealTrials > 1, not a practice trial.
			trialLogger.LogInstructionEvent ();
			yield return StartCoroutine (exp.ShowSingleInstruction ("Press (X) to start!", true, true, false, Config_CoinTask.minDefaultInstructionTime));
		}

		//START NAVIGATION --> TODO: make this its own function. or a delegate. ...clean it up.
		trialLogger.LogTrialNavigationStarted ();

		goText.Play ();

		//start a game timer
		trialTimer.ResetTimer ();
		trialTimer.StartTimer ();

		//unlock avatar controls
		exp.player.controls.ShouldLockControls = false;

		//wait for player to collect all default objects
		int numDefaultObjectsToCollect = currentTrial.DefaultObjectLocationsXZ.Count;
		while (numDefaultObjectsCollected < numDefaultObjectsToCollect) {
			yield return 0;
		}

		//Add time bonus
		trialTimer.StopTimer ();
		timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSecondsInt());

		//reset num default objects collected
		numDefaultObjectsCollected = 0;

		//lock player movement
		exp.player.controls.ShouldLockControls = true;

		//bring player to tower
		//exp.player.TurnOnVisuals (false);
		trialLogger.LogTransportationToTowerEvent ();
		currentDefaultObject = null; //set to null so that arrows stop showing up...
		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarTowerPos, currentTrial.avatarTowerRot));//PlayerControls.toTowerTime) );


		//RUN DISTRACTOR GAME
		//TODO: LOG THIS
		trialLogger.LogDistractorGameStarted ();
		yield return StartCoroutine(exp.boxGameController.RunGame());



		//jitter before the first object is shown
		yield return StartCoroutine(exp.WaitForJitter(Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));

		//show instructions for location selection 
		trialLogger.LogRecallPhaseStarted();

		//ask player to locate each object individually
		List<int> randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order
		List<bool> rememberResponses = new List<bool> (); //keep track of whether or not the player remembered each object
		List<bool> areYouSureResponses = new List<bool> (); //keep track of whether or not the player wanted to double down on each object

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

			trialLogger.LogInstructionEvent();
			yield return StartCoroutine( doYouRememberUI.Play(specialObjUICopy) );

			yield return StartCoroutine (exp.WaitForActionButton());
			bool rememberResponse = doYouRememberUI.myAnswerSelector.IsYesPosition();
			rememberResponses.Add(rememberResponse);
			trialLogger.LogRememberResponse(rememberResponse);

			//IF THEY SAID 'YES, I REMEMBER', SELECT LOCATION
			//if(response == true){
			//enable position selection, turn off fancy selection UI
			exp.environmentController.myPositionSelector.Reset();
			exp.environmentController.myPositionSelector.EnableSelection (true);

			doYouRememberUI.Stop();

			//show single selection instruction and wait for selection button press
			string selectObjectText = "Select the location of the " + specialItemName + " (X).";
			yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, false, Config_CoinTask.minDefaultInstructionTime));

			//log the chosen position and correct position
			exp.environmentController.myPositionSelector.logTrack.LogPositionChosen( exp.environmentController.myPositionSelector.GetSelectorPosition(), specialObj.transform.position, specialSpawnable );

			//wait for the position selector to choose the position, runs color changing of the selector
			yield return StartCoroutine (exp.environmentController.myPositionSelector.ChoosePosition());


			//add current chosen position to list of chosen positions
			chosenPositions.Add(exp.environmentController.myPositionSelector.GetSelectorPosition());

			//disable position selection
			exp.environmentController.myPositionSelector.EnableSelection (false);

			trialLogger.LogInstructionEvent();

			if(rememberResponse == true){
				yield return StartCoroutine( areYouSureUI.Play() );

				yield return StartCoroutine (exp.WaitForActionButton());
				bool areYouSureResponse = areYouSureUI.myAnswerSelector.IsYesPosition();
				trialLogger.LogAreYouSureResponse(areYouSureResponse);
				areYouSureResponses.Add(areYouSureResponse);
			}
			else{
				//if you chose not to remember, you should not get to double down.
				areYouSureResponses.Add(false);
			}

			if(i <= exp.objectController.CurrentTrialSpecialObjects.Count - 1){
				//jitter if it's not the last object to be shown
				yield return StartCoroutine(exp.WaitForJitter(Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));
			}

			if(rememberResponse == true){
				areYouSureUI.Stop();
			}
			//}

			//IF THEY SAID 'NO, I DON'T REMEMBER', DON'T SELECT LOCATION
			/*else{

				if(i <= exp.objectController.CurrentTrialSpecialObjects.Count - 1){
					//jitter if it's not the last object to be shown
					yield return StartCoroutine(exp.WaitForJitter(Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));
				}

				doYouRememberUI.Stop();
				areYouSureResponses.Add(false);

				//add a placeholder vector position here...
				chosenPositions.Add(Vector3.zero);
			}*/

		}

		trialLogger.LogFeedbackStarted();
		yield return StartCoroutine (ShowFeedback (randomSpecialObjectOrder, chosenPositions, rememberResponses, areYouSureResponses) );
		
		//increment subject's trial count
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();

	}

	IEnumerator ShowFeedback(List<int> specialObjectOrder, List<Vector3> chosenPositions, List<bool> rememberResponses, List<bool> areYouSureResponses){
		memoryScore = 0;

		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		List<GameObject> specialObjectListRecallOrder = new List<GameObject>();

		List<int> objectScores = new List<int> ();
		
		for (int i = 0; i < specialObjectOrder.Count; i++){

			Vector3 chosenPosition = chosenPositions[i];

			//throw bomb to selected location
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't actually want its visuals showing up as we wait

			yield return StartCoroutine( exp.objectController.ThrowExplosive( exp.player.transform.position, chosenPosition ) );


			int randomOrderIndex = specialObjectOrder[i];

			//turn on each special object & scale up for better visibility
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			specialObjectListRecallOrder.Add(specialObj);

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

			//create an indicator for each chosen position
			//spawn the indicator at the height of the original indicator
			exp.environmentController.myPositionSelector.EnableSelection (true); //turn on selector for spawning indicator
			Vector3 chosenIndicatorPosition = new Vector3(chosenPosition.x, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y, chosenPosition.z);
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;

			chosenPositionIndicator.GetComponent<SpawnableObject>().SetNameID(i);
			chosenPositionIndicator.GetComponent<VisibilityToggler>().TurnVisible(true);


			ChosenPositionIndicators.Add(chosenPositionIndicator);
			
			//calculate the memory points and display them
			exp.environmentController.myPositionSelector.PositionSelector.transform.position = chosenPosition;

			int points = exp.scoreController.CalculateMemoryPoints( specialObj.transform.position, rememberResponses[i], areYouSureResponses[i] );

			//change chosen indicator color to reflect right or wrong
			ChosenIndicatorController chosenIndicatorController = chosenPositionIndicator.GetComponent<ChosenIndicatorController>();
			Color chosenPositionColor = chosenIndicatorController.RightColor;
			if(points > 0){
				chosenIndicatorController.ChangeToRightColor();
			}
			else if (points <= 0){
				chosenIndicatorController.ChangeToWrongColor();
				chosenPositionColor = chosenIndicatorController.WrongColor;
			}


			//connect the chosen and correct indicators via a line
			SetConnectingLines( correctPositionIndicator, chosenPosition, chosenPositionColor);


			CorrectPositionIndicatorController correctPosController = correctPositionIndicator.GetComponent<CorrectPositionIndicatorController>();

			correctPosController.SetPointsText(points);
			memoryScore += points;

			objectScores.Add(points);
		

		}
		
		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection(false);
		
		//wait for selection button press
		yield return StartCoroutine (exp.ShowSingleInstruction ("Press the button to continue.", false, true, false, Config_CoinTask.minDefaultInstructionTime));

		int currTrialNum = numRealTrials;
		if (Config_CoinTask.doPracticeTrial) {
			currTrialNum++;
		}

		trialLogger.LogInstructionEvent();
		scoreRecapUI.Play(currTrialNum, timeBonus + memoryScore, Config_CoinTask.GetTotalNumTrials(), objectScores, specialObjectListRecallOrder, timeBonus, trialTimer.GetSecondsFloat());
		yield return StartCoroutine (exp.WaitForActionButton ());
		scoreRecapUI.Stop ();


		//delete all indicators & special objects
		DestroyGameObjectList (CorrectPositionIndicators);
		DestroyGameObjectList (ChosenPositionIndicators);
		DestroyGameObjectList (exp.objectController.CurrentTrialSpecialObjects);
		
		yield return 0;
	}

	void SetConnectingLines( GameObject correctPositionIndicator, Vector3 chosenPosition, Color chosenIndicatorColor){//, EnvironmentPositionSelector.SelectionRadiusType chosenRadiusSize ){

		//render a line between the special object and its corresponding chosen position
		LineRenderer positionConnector = correctPositionIndicator.GetComponent<LineRenderer>();
		Vector3 correctPosition = correctPositionIndicator.transform.position;

		correctPositionIndicator.GetComponent<CorrectPositionIndicatorController>().SetLineTarget(chosenPosition, chosenIndicatorColor);
	}

	void DestroyGameObjectList(List<GameObject> listOfGameObjects){
		int numObjects = listOfGameObjects.Count;
		for (int i = 0; i < numObjects; i++) {
			Destroy (listOfGameObjects [i]);
		}
		listOfGameObjects.Clear ();
	}

	public void LogAnswerSelectorPositionChanged(bool isYesPosition){
		if (doYouRememberUI.isPlaying) {
			trialLogger.LogAnswerPositionMoved( isYesPosition, true );
		} 
		else if (areYouSureUI.isPlaying) {
			trialLogger.LogAnswerPositionMoved( isYesPosition, false );
		}
	}

	public IEnumerator WaitForPlayerRotationToTreasure(GameObject treasureChest){
		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		
		yield return StartCoroutine (exp.player.controls.RotateTowardSpecialObject (treasureChest));

		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;
	}

	public IEnumerator WaitForTreasurePause( GameObject specialObject){
		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		yield return new WaitForSeconds (Config_CoinTask.pauseAtTreasureTime);
		
		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;

		//turn the special object invisible
		if(specialObject != null){
			specialObject.GetComponent<SpawnableObject> ().TurnVisible (false);
		}


		//only after the pause should we increment the number of coins collected...
		//...because the trial controller waits for this to move on to the next part of the trial.
		IncrementNumDefaultObjectsCollected();
	}
}
