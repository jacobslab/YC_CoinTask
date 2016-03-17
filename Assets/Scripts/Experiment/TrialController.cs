using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//hardware connection
	bool isConnectingToHardware = false;

	//paused?!
	public static bool isPaused = false;

	//TIMER
	public SimpleTimer trialTimer;
	public SimpleTimer MRITimer;

	TrialLogTrack trialLogger;

	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's

	int numDefaultObjectsCollected = 0;
	public int NumDefaultObjectsCollected { get { return numDefaultObjectsCollected; } }

	int timeBonus = 0;
	int memoryScore = 0;
	public CanvasGroup scoreInstructionsGroup;

	Trial currentTrial;

	[HideInInspector] public GameObject currentDefaultObject; //current treasure chest we're looking for. assuming a one-by-one reveal.
	
	List<List<Trial>> ListOfTrialBlocks;
	List<Trial> practiceTrials;

	void Start(){
		#if MRIVERSION
		if(Config_CoinTask.isPractice){
			InitPracticeTrials();
		}
		else{
			InitTrials();
		}
		#else
		if(Config_CoinTask.isPractice){
			InitPracticeTrials();
		}
		InitTrials ();
		#endif

		trialLogger = GetComponent<TrialLogTrack> ();
	}

	void InitPracticeTrials(){
		practiceTrials = new List<Trial>();

		#if MRIVERSION
		for(int i = 0; i < Config_CoinTask.numTrialsPract; i++){
			Trial practiceTrial = new Trial(Config_CoinTask.numSpecialObjectsPract[i]);	//2 special objects for practice trial
			practiceTrials.Add(practiceTrial);
		}
		#else
		Trial  practiceTrial = new Trial(Config_CoinTask.numSpecialObjectsPract);	//2 special objects for practice trial
		practiceTrials.Add(practiceTrial);
		#endif
	}

	void InitTrials(){

		ListOfTrialBlocks = new List<List<Trial>> ();

		int numTestTrials = Config_CoinTask.numTestTrials;

		//int numTrialsPerBlock = (int)(Config_CoinTask.trialBlockDistribution [0] + Config_CoinTask.trialBlockDistribution [1]);

		if (numTestTrials % Config_CoinTask.numTrialsPerBlock != 0) {
			Debug.Log("CANNOT EXECUTE THIS TRIAL DISTRIBUTION");
		}

		//generate all trials, two & three object, including counter-balanced trials
		List<Trial> ListOfTwoItemTrials = GenerateTrials(Config_CoinTask.numTwoItemTrials, 2);
		List<Trial> ListOfThreeItemTrials = GenerateTrials(Config_CoinTask.numThreeItemTrials, 3);

		//generate blocks from trials
		int numTrialBlocks = numTestTrials / Config_CoinTask.numTrialsPerBlock;
		GenerateTrialBlocks(ListOfTwoItemTrials, ListOfThreeItemTrials, numTrialBlocks, Config_CoinTask.numTrialsPerBlock);

	}

	List<Trial> GenerateTrials(int numTrialsToGenerate, int numSpecial){
		List<Trial> trialList = new List<Trial>();
		for(int i = 0; i < numTrialsToGenerate / 2; i++){ //we're adding trial and a counter trial
			Trial trial = new Trial(numSpecial);
			Trial counterTrial = trial.GetCounterSelf();
			
			trialList.Add(trial);
			trialList.Add(counterTrial);
		}

		return trialList;
	}

	void GenerateTrialBlocks(List<Trial> twoItemTrials, List<Trial> threeItemTrials, int numBlocks, int numTrialsPerBlock){
		for(int i = 0; i < numBlocks; i++){
			List<Trial> newBlock = new List<Trial>();
			for(int j = 0; j < numTrialsPerBlock / 2; j++){ //half two item, half one item
				int randomTwoItemIndex = Random.Range (0, twoItemTrials.Count);
				int randomThreeItemIndex = Random.Range (0, threeItemTrials.Count);

				newBlock.Add(twoItemTrials[randomTwoItemIndex]);
				newBlock.Add(threeItemTrials[randomThreeItemIndex]);

				twoItemTrials.RemoveAt(randomTwoItemIndex);
				threeItemTrials.RemoveAt(randomThreeItemIndex);
			}
			ListOfTrialBlocks.Add(newBlock);
		}
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
		if(!isConnectingToHardware){
			GetPauseInput ();
		}
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
			exp.uiController.PauseUI.alpha = 1.0f;
			Time.timeScale = 0.0f;
			TCPServer.Instance.SetState (TCP_Config.DefineStates.PAUSED, true);
		} 
		else {
			Time.timeScale = 1.0f;
			//exp.player.controls.Pause(false);
			//exp.player.controls.ShouldLockControls = false;
			exp.uiController.PauseUI.alpha = 0.0f;
			TCPServer.Instance.SetState (TCP_Config.DefineStates.PAUSED, false);
		}
	}


	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings_CoinTask.isReplay) {
			exp.player.controls.ShouldLockControls = true;

			if(Config_CoinTask.isSystem2 || Config_CoinTask.isSyncbox){
				yield return StartCoroutine( WaitForEEGHardwareConnection() );
			}
			else{
				exp.uiController.ConnectionUI.alpha = 0.0f;
			}
				
#if !(MRIVERSION)
			trialLogger.LogVideoEvent(true);
			yield return StartCoroutine(exp.instrVideoPlayer.Play());
			trialLogger.LogVideoEvent(false);
#endif

			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent();
			yield return StartCoroutine(exp.uiController.pirateController.PlayWelcomingPirate());

#if MRIVERSION
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions1, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			scoreInstructionsGroup.alpha = 0.0f;

			if(Config_CoinTask.isPractice){
				yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions2, true, true, false, Config_CoinTask.minInitialInstructionsTime));
				scoreInstructionsGroup.alpha = 0.0f;
				yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions3, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			}
			else{
				yield return StartCoroutine( WaitForMRIConnectionKey());
				yield return StartCoroutine( WaitForMRIFixationRest());
			}
#else
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions1, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			scoreInstructionsGroup.alpha = 1.0f;
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions2, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			scoreInstructionsGroup.alpha = 0.0f;
			yield return StartCoroutine (exp.ShowSingleInstruction (Config_CoinTask.initialInstructions3, true, true, false, Config_CoinTask.minInitialInstructionsTime));
#endif

			#if MRIVERSION
			if(Config_CoinTask.isPractice){
				yield return StartCoroutine(RunPracticeTrials()); //if MRI practice, ONLY run the practice trials
			}
			else{
				yield return StartCoroutine(RunTrials());
			}
			#else
			yield return StartCoroutine(RunPracticeTrials);
			yield return StartCoroutine(RunTrials());
			#endif	

			yield return 0;
		}

		StartCoroutine(exp.uiController.pirateController.PlayEndingPirate ());
		yield return StartCoroutine(exp.ShowSingleInstruction("You have finished your trials! \nPress (X) to proceed.", true, true, false, 0.0f));

#if MRIVERSION
		yield return StartCoroutine( WaitForMRIFixationRest());
#endif
	}

	IEnumerator RunPracticeTrials(){
		//run practice trials
		if(Config_CoinTask.isPractice){
			isPracticeTrial = true;
		}

		if (isPracticeTrial) {
			int numPracticeTrialsRun = 0;
			while(numPracticeTrialsRun < practiceTrials.Count){

				yield return StartCoroutine (RunTrial ( practiceTrials[numPracticeTrialsRun] ));
				numPracticeTrialsRun++;
			}
			Debug.Log ("PRACTICE TRIALS COMPLETED");
			isPracticeTrial = false;
		}
	}

	IEnumerator RunTrials(){
		//RUN THE REST OF THE BLOCKS
		for( int i = 0; i < ListOfTrialBlocks.Count; i++){
			List<Trial> currentTrialBlock = ListOfTrialBlocks[i];
			while (currentTrialBlock.Count > 0) {
				Trial nextTrial = PickRandomTrial (currentTrialBlock);

				yield return StartCoroutine (RunTrial ( nextTrial ));

			}

			//FINISHED A TRIAL BLOCK, SHOW UI
			trialLogger.LogInstructionEvent();
			StartCoroutine(exp.uiController.pirateController.PlayEncouragingPirate());
			exp.uiController.blockCompletedUI.Play(i, exp.scoreController.score, ListOfTrialBlocks.Count);
			trialLogger.LogBlockScreenStarted(true);
			TCPServer.Instance.SetState (TCP_Config.DefineStates.BLOCKSCREEN, true);

			yield return StartCoroutine(exp.WaitForActionButton());

			exp.uiController.blockCompletedUI.Stop();
			trialLogger.LogBlockScreenStarted(false);
			TCPServer.Instance.SetState (TCP_Config.DefineStates.BLOCKSCREEN, false);

			exp.scoreController.Reset();

			Debug.Log ("TRIAL Block: " + i);
		}
	}

	IEnumerator WaitForEEGHardwareConnection(){
		isConnectingToHardware = true;

		exp.uiController.ConnectionUI.alpha = 1.0f;
		if(Config_CoinTask.isSystem2){
			while(!TCPServer.Instance.isConnected || !TCPServer.Instance.canStartGame){
				Debug.Log("Waiting for system 2 connection...");
				yield return 0;
			}
		}
		if (Config_CoinTask.isSyncbox){
			while(!SyncboxControl.Instance.isUSBOpen){
				Debug.Log("Waiting for sync box to open...");
				yield return 0;
			}
		}
		exp.uiController.ConnectionUI.alpha = 0.0f;
		isConnectingToHardware = false;
	}

#if MRIVERSION
	IEnumerator WaitForMRIConnectionKey(){
		exp.uiController.WaitingForMRIUI.alpha = 1.0f;
		while (Input.GetAxis("MRI Start Button") <= 0.0f) {
			yield return 0;
		}
		exp.uiController.WaitingForMRIUI.alpha = 0.0f;
	}

	IEnumerator WaitForMRIFixationRest(){
		exp.uiController.FixationRestUI.alpha = 1.0f;
		yield return new WaitForSeconds(Config_CoinTask.MRIFixationTime);
		exp.uiController.FixationRestUI.alpha = 0.0f;
	}

	public IEnumerator WaitForMRITimeout(float maxSeconds){
		exp.uiController.MRITimerUI.alpha = 1.0f;
		MRITimer.ResetTimerNoDelegate(maxSeconds);
		MRITimer.StartTimer ();

		bool hasPressedButton = false;

		while(MRITimer.GetSecondsFloat() > 0.0f && Input.GetAxis("Action Button") <= 0f){
			yield return 0;
		}
		while(!hasPressedButton && MRITimer.GetSecondsFloat() > 0.0f){
			if(Input.GetAxis("Action Button") == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}

		MRITimer.StopTimer ();
		exp.uiController.MRITimerUI.alpha = 0.0f;
	}

	public IEnumerator WaitForMRINavigationTimeout(float maxSeconds){
		exp.uiController.MRITimerUI.alpha = 1.0f;
		MRITimer.ResetTimerNoDelegate(maxSeconds);
		MRITimer.StartTimer ();

		bool hasPressedButton = false;

		int currNumCollected = numDefaultObjectsCollected;
		while(MRITimer.GetSecondsFloat() > 0.0f && currNumCollected == numDefaultObjectsCollected){
			yield return 0;
		}

		if(MRITimer.GetSecondsFloat() <= 0.0f){
			Vector2 currChestPos = currentTrial.DefaultObjectLocationsXZ[currNumCollected];
			Vector3 targetChestPos = new Vector3(currChestPos.x, exp.player.transform.position.y, currChestPos.y);

			float collisionBuffer = Experiment_CoinTask.Instance.objectController.GetMaxDefaultObjectColliderBoundXZ ();
			Vector3 direction = targetChestPos - exp.player.transform.position;
			targetChestPos = targetChestPos - (direction.normalized*collisionBuffer);

			Quaternion desiredRot = UsefulFunctions.GetDesiredRotation(exp.player.transform, targetChestPos);

			yield return StartCoroutine(exp.player.controls.SmoothMoveTo(targetChestPos, desiredRot));
		}

		MRITimer.StopTimer ();
		exp.uiController.MRITimerUI.alpha = 0.0f;
	}
	#endif

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
			TCPServer.Instance.SendTrialNum(numRealTrials);
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		//move player to home location & rotation
		trialLogger.LogTransportationToHomeEvent (true);
		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarStartPos, currentTrial.avatarStartRot));
		trialLogger.LogTransportationToHomeEvent (false);

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

		//reset game timer
		trialTimer.ResetTimer (0);

		if(numRealTrials > 1 || trial.avatarStartPos != exp.player.controls.startPositionTransform1.position){ //note: if numRealTrials > 1, not a practice trial.
			trialLogger.LogInstructionEvent ();

			#if !(MRIVERSION)
			yield return StartCoroutine (exp.ShowSingleInstruction ("Press (X) to start!", true, true, false, Config_CoinTask.minDefaultInstructionTime));
			#endif
		}

		//START NAVIGATION --> TODO: make this its own function. or a delegate. ...clean it up.
		trialLogger.LogTrialNavigation (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.NAVIGATION, true);

		exp.uiController.goText.Play ();

		//start a game timer
		trialTimer.StartTimer ();

		//unlock avatar controls
		exp.player.controls.ShouldLockControls = false;

		//wait for player to collect all default objects
		int numDefaultObjectsToCollect = currentTrial.DefaultObjectLocationsXZ.Count;

		#if MRIVERSION
		for(int i = 0; i <numDefaultObjectsToCollect; i++){
			yield return StartCoroutine(WaitForMRINavigationTimeout(Config_CoinTask.maxChestNavigationTime));
		}
		#else //if not MRI version, just wait until all chests are collected;
		while (numDefaultObjectsCollected < numDefaultObjectsToCollect) {
			yield return 0;
		}
		#endif

		//Add time bonus
		trialTimer.StopTimer ();
		timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSecondsInt());

		//reset num default objects collected
		numDefaultObjectsCollected = 0;

		//lock player movement
		exp.player.controls.ShouldLockControls = true;

		//bring player to tower
		//exp.player.TurnOnVisuals (false);
		trialLogger.LogTrialNavigation (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.NAVIGATION, false);
		trialLogger.LogTransportationToTowerEvent (true);
		currentDefaultObject = null; //set to null so that arrows stop showing up...
		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarTowerPos, currentTrial.avatarTowerRot));//PlayerControls.toTowerTime) );
		trialLogger.LogTransportationToTowerEvent (false);

		//RUN DISTRACTOR GAME
		trialLogger.LogDistractorGame (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.DISTRACTOR, true);
		yield return StartCoroutine(exp.boxGameController.RunGame());
		trialLogger.LogDistractorGame (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.DISTRACTOR, false);

		//jitter before the first object is shown
		yield return StartCoroutine(exp.WaitForJitter(Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));

		//show instructions for location selection 
		trialLogger.LogRecallPhaseStarted(true);

		//ask player to locate each object individually
		List<int> randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder( exp.objectController.CurrentTrialSpecialObjects.Count );
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order
		List<Config_CoinTask.MemoryState> rememberResponses = new List<Config_CoinTask.MemoryState> (); //keep track of whether or not the player remembered each object
		//List<bool> areYouSureResponses = new List<bool> (); //keep track of whether or not the player wanted to double down on each object

		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {

			//show instructions for location selection
			int randomOrderIndex = randomSpecialObjectOrder[i];
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject>();
			string specialItemName = specialSpawnable.GetName();

			//set TCP state true
			switch(randomOrderIndex){
			case 0:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_1, true);
				break;
			case 1:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_2, true);
				break;
			case 2:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_3, true);
				break;
			}

			//show nice UI, log special object
			trialLogger.LogObjectToRecall(specialSpawnable);
			GameObject specialObjUICopy = Instantiate (specialObj, Vector3.zero, specialObj.transform.rotation) as GameObject;
			specialObjUICopy.name += "UICopy";

			//set layer of object & children to PlayerUI
			specialObjUICopy.GetComponent<SpawnableObject>().SetLayer ("PlayerUI");

			trialLogger.LogInstructionEvent();
			yield return StartCoroutine( exp.uiController.doYouRememberUI.Play(specialObjUICopy, specialSpawnable.GetName()) );

#if MRIVERSION
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxAnswerTime));
#else
			yield return StartCoroutine (exp.WaitForActionButton());
#endif
			Config_CoinTask.MemoryState rememberResponse = exp.uiController.doYouRememberUI.myAnswerSelector.GetMemoryState();
			rememberResponses.Add(rememberResponse);
			trialLogger.LogRememberResponse(rememberResponse);


			//SELECT LOCATION
			//enable position selection, turn off fancy selection UI
			exp.environmentController.myPositionSelector.Reset();
			exp.environmentController.myPositionSelector.EnableSelection (true);

			switch(randomOrderIndex){
			case 0:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_1, false);
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_1, true);
				break;
			case 1:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_2, false);
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_2, true);
				break;
			case 2:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_3, false);
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_3, true);
				break;
			}
			trialLogger.LogRecallChoiceStarted(true);

			exp.uiController.doYouRememberUI.Stop();

			//show single selection instruction and wait for selection button press
			string selectObjectText = "Select the location of the " + specialItemName + " (X).";

#if MRIVERSION
			exp.instructionsController.SetInstructionsTransparentOverlay();
			exp.instructionsController.DisplayText(selectObjectText);
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxLocationChooseTime));
			exp.instructionsController.SetInstructionsBlank();
#else
			yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, false, Config_CoinTask.minDefaultInstructionTime));
#endif
			//log the chosen position and correct position
			exp.environmentController.myPositionSelector.logTrack.LogPositionChosen( exp.environmentController.myPositionSelector.GetSelectorPosition(), specialObj.transform.position, specialSpawnable );

			//wait for the position selector to choose the position, runs color changing of the selector
			yield return StartCoroutine (exp.environmentController.myPositionSelector.ChoosePosition());


			//add current chosen position to list of chosen positions
			chosenPositions.Add(exp.environmentController.myPositionSelector.GetSelectorPosition());

			//disable position selection
			exp.environmentController.myPositionSelector.EnableSelection (false);
			trialLogger.LogRecallChoiceStarted(false);

			switch(randomOrderIndex){
			case 0:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_1, false);
				break;
			case 1:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_2, false);
				break;
			case 2:
				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCHOOSE_3, false);
				break;
			}


			trialLogger.LogInstructionEvent();

			if(i <= exp.objectController.CurrentTrialSpecialObjects.Count - 1){
				//jitter if it's not the last object to be shown
				yield return StartCoroutine(exp.WaitForJitter(Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));
			}

		}

		trialLogger.LogRecallPhaseStarted(false);
		
		yield return StartCoroutine (ShowFeedback (randomSpecialObjectOrder, chosenPositions, rememberResponses));//, areYouSureResponses) );

		//increment subject's trial count
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();

	}

	int currTrialNum = 0; //TODO: just use currentSubject.trialNum...
	IEnumerator ShowFeedback(List<int> specialObjectOrder, List<Vector3> chosenPositions, List<Config_CoinTask.MemoryState> rememberResponses){//, List<bool> areYouSureResponses){
		trialLogger.LogFeedback(true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, true);

		memoryScore = 0;

		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		List<GameObject> specialObjectListRecallOrder = new List<GameObject>();

		List<int> objectScores = new List<int> ();

		//float indicatorHeightIncrement = 0.3f;

		for (int i = 0; i < specialObjectOrder.Count; i++){

			Vector3 chosenPosition = chosenPositions[i];

			//throw bomb to selected location
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't actually want its visuals showing up as we wait

#if !(MRIVERSION)
			yield return StartCoroutine( exp.objectController.ThrowExplosive( exp.player.transform.position, chosenPosition, i ) );
#endif

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
			correctPositionIndicator.GetComponent<SpawnableObject>().SetNameID(correctPositionIndicator.transform, i);
			CorrectPositionIndicators.Add(correctPositionIndicator); 

			//create an indicator for each chosen position
			//spawn the indicator at the height of the original indicator
			exp.environmentController.myPositionSelector.EnableSelection (true); //turn on selector for spawning indicator
			float chosenIndicatorHeight = exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y;
			Vector3 chosenIndicatorPosition = new Vector3(chosenPosition.x, chosenIndicatorHeight, chosenPosition.z);
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;

			chosenPositionIndicator.GetComponent<SpawnableObject>().SetNameID(chosenPositionIndicator.transform, i);
			chosenPositionIndicator.GetComponent<VisibilityToggler>().TurnVisible(true);


			ChosenPositionIndicators.Add(chosenPositionIndicator);
			
			//calculate the memory points and display them
			exp.environmentController.myPositionSelector.PositionSelector.transform.position = chosenPosition;

			int points = exp.scoreController.CalculateMemoryPoints( specialObj.transform.position, rememberResponses[i]);//, areYouSureResponses[i] );

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


			//WAIT BEFORE NEXT FEEDBACK
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't want its visuals showing up as we wait
#if !(MRIVERSION)
			yield return new WaitForSeconds(Config_CoinTask.feedbackTimeBetweenObjects);
#endif
		}
		
		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection(false);

#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxFeedbackTime));
#else
		//wait for selection button press
		yield return StartCoroutine (exp.ShowSingleInstruction ("Press (X) to continue.", false, true, false, Config_CoinTask.minDefaultInstructionTime));
#endif

		currTrialNum++;


		trialLogger.LogInstructionEvent();
		trialLogger.LogScoreScreenStarted(true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, true);
		exp.uiController.scoreRecapUI.Play(currTrialNum, timeBonus + memoryScore, Config_CoinTask.GetTotalNumTrials(), objectScores, specialObjectListRecallOrder, timeBonus, trialTimer.GetSecondsFloat());

#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxScoreScreenTime));
#else
		yield return StartCoroutine (exp.WaitForActionButton ());
#endif

		exp.uiController.scoreRecapUI.Stop ();
		trialLogger.LogScoreScreenStarted(false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, false);


		//delete all indicators & special objects
		DestroyGameObjectList (CorrectPositionIndicators);
		DestroyGameObjectList (ChosenPositionIndicators);
		DestroyGameObjectList (exp.objectController.CurrentTrialSpecialObjects);

		trialLogger.LogFeedback(false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, false);

		yield return 0;
	}

	void SetConnectingLines( GameObject correctPositionIndicator, Vector3 chosenPosition, Color chosenIndicatorColor){//, EnvironmentPositionSelector.SelectionRadiusType chosenRadiusSize ){
		correctPositionIndicator.GetComponent<CorrectPositionIndicatorController>().SetLineTarget(chosenPosition, chosenIndicatorColor);
	}

	void DestroyGameObjectList(List<GameObject> listOfGameObjects){
		int numObjects = listOfGameObjects.Count;
		for (int i = 0; i < numObjects; i++) {
			Destroy (listOfGameObjects [i]);
		}
		listOfGameObjects.Clear ();
	}

	public void LogAnswerSelectorPositionChanged(Config_CoinTask.MemoryState memoryState){
		if (exp.uiController.doYouRememberUI.isPlaying) {
			trialLogger.LogAnswerPositionMoved( memoryState, true );
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
