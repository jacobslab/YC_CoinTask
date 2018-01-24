using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class TrialController : MonoBehaviour
{
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
	int numRealTrials = 0;
	//used for logging trial ID's

	int numDefaultObjectsCollected = 0;

	public int NumDefaultObjectsCollected { get { return numDefaultObjectsCollected; } }


	KeywordRecognizer keywordRecognizer;
	Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action> ();

	int timeBonus = 0;
	int memoryScore = 0;
	public int temporalScore = 0;
	public CanvasGroup scoreInstructionsGroup;
	public Trial currentTrial;

	#if FREIBURG
	string initialInstructions1= "Willkommen bei der Schatzsuche! " +
		"\n\n Benutzen Sie den Joystick zum navigieren. " +
		"\n\nSchatzkisten öffnen sich bei Kontakt. " +
		"\n\nVersuchen Sie sich  für jeden Gegenstand zu merken wo Sie ihn gefunden haben. " +
		"\n\nDrücken Sie (X) zum Fortfahren.";
	

	string initialInstructions2 = "Nachdem Sie zu den Schatzkisten navigiert sind, werden Sie automatisch zum Ende der Umgebung transportiert." +
		"\n\n Wir werden einen Ort markieren und ihre Aufgabe ist es, den Namen des Gegenstandes, den Sie an diesem Ort gefunden haben, zu sagen. " +
		"\n\nWenn Sie sich an den Gegenstand nicht erinnern können, sagen Sie bitte 'VERGESSEN'" +
		"\n\nWenn wir einen Ort markieren, an dem keine Schatzkiste war, sagen Sie bitte 'NICHTS' ins Mikrophon.  " +
		"\n\nSie haben 6 Sekunden Zeit zum Antworten. " +
		"\n\nDrücken Sie (X) zum Fortfahren";

	

#else
	string initialInstructions1 = "Welcome to Treasure Island!" +
	                              "\n\nYou are going on a treasure hunt." +
	                              "\n\nUse the joystick to control your movement." +
	                              "\nDrive into treasure chests to open them. Your job is to try to remember where each object is located!" +
	                              "\n\nPress (X) to continue.";

	string initialInstructions2 = "After traveling to the treasure chests, you will be moved to the edge of the environment and we will highlight a location on the ground." +
	                              "\n\nYour job is now to try to say out loud the object that you saw at that location.\n" +
	                              "\n\nIf you can’t remember, say, “PASS" +
	                              "\nIf we show you a new location where there was no treasure chest, say “TRICK”\n" +
	                              "\nYou will have 6 seconds to respond to each item." +
	                              "\n\n Press (X) to Continue.";


	#endif
	bool objectRecall = false;
	int currentTrialBlockNumber;
	int totalTrialNumber = 0;
	int currentTrialNumber;
	int recallTime = 4;
	//set in InitConfigVariables()
	[HideInInspector] public GameObject currentDefaultObject;
	//current treasure chest we're looking for. assuming a one-by-one reveal.
	
	List<List<Trial>> ListOfTrialBlocks;
	List<Trial> practiceTrials;

	void Start ()
	{
		#if MRIVERSION
		if(Config_CoinTask.isPractice){
			InitPracticeTrials();
		}
		else{
			InitTrials();
		}
		#else
		if (Config_CoinTask.isPractice) {
			InitPracticeTrials ();
		}
		InitTrials ();
		#endif

		trialLogger = GetComponent<TrialLogTrack> ();
	}

	void InitPracticeTrials ()
	{
		practiceTrials = new List<Trial> ();

		#if MRIVERSION
		for(int i = 0; i < Config_CoinTask.numTrialsPract; i++){
			Trial practiceTrial = new Trial(Config_CoinTask.numSpecialObjectsPract[i], false);	//2 special objects for practice trial
			practiceTrials.Add(practiceTrial);
		}
		#else
		Trial practiceTrial = new Trial (Config_CoinTask.numSpecialObjectsPract, 0, false);	//2 special objects for practice trial
		practiceTrials.Add (practiceTrial);
		#endif
	}

	void InitTrials ()
	{

		if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH3) {
			InitTH3Trials ();
		} else {
			InitStandardTrials ();
		}

	}

	void InitStandardTrials ()
	{
		ListOfTrialBlocks = new List<List<Trial>> ();

		if (File.Exists (ExperimentSettings_CoinTask.manualTrialFilePath)) {
			GenerateTrialsFromFile ();
		} else {
			int numTestTrials = Config_CoinTask.numTestTrials;

			//int numTrialsPerBlock = (int)(Config_CoinTask.trialBlockDistribution [0] + Config_CoinTask.trialBlockDistribution [1]);

			if (numTestTrials % Config_CoinTask.numTrialsPerBlock != 0) {
				Debug.Log ("CANNOT EXECUTE THIS TRIAL DISTRIBUTION");
			}

			//generate all trials, two & three object, including counter-balanced trials
			List<Trial> ListOfTwoItemTrials = GenerateTrialsWithCounterTrials (Config_CoinTask.numTwoItemTrials, 2, false, false);
			List<Trial> ListOfThreeItemTrials = GenerateTrialsWithCounterTrials (Config_CoinTask.numThreeItemTrials, 3, false, false);

			//generate blocks from trials
			int numTrialBlocks = numTestTrials / Config_CoinTask.numTrialsPerBlock;
			Debug.Log ("NUMBER OF BLOCKS: " + numTrialBlocks.ToString ());
			GenerateTrialBlocks (ListOfTwoItemTrials, ListOfThreeItemTrials, numTrialBlocks, Config_CoinTask.numTrialsPerBlock);
		}
	}

	void InitTH3Trials ()
	{
		ListOfTrialBlocks = new List<List<Trial>> ();

		List<Trial> currBlock = new List<Trial> ();
		//generate first block of completely unique, no-stim trials (no counter-trials)
		//half should be two-item trials, half should be three-item trials
		for (int i = 0; i < Config_CoinTask.numTrialsPerBlock / 2; i++) {
			Trial t2 = new Trial (2, 0, false);
			Trial t3 = new Trial (3, 0, false);

			currBlock.Add (t2);
			currBlock.Add (t3);
		}

		ListOfTrialBlocks.Add (currBlock);

		//Now generate the rest of the trials: the original, non-stim trials, and the counter, stim trials.
		List<Trial> twoItemOrigTrials = new List<Trial> ();
		List<Trial> threeItemOrigTrials = new List<Trial> ();
		List<Trial> twoItemCounterTrials = new List<Trial> ();
		List<Trial> threeItemCounterTrials = new List<Trial> ();

		int blocksLeft = (Config_CoinTask.numTestTrials / Config_CoinTask.numTrialsPerBlock) - 1; //as of 7/5/2016, should be 4 trials left
		int numOrigTrials = Config_CoinTask.numTrialsPerBlock * blocksLeft;
		for (int i = 0; i < numOrigTrials / 2; i++) {
			Trial t2 = new Trial (2, 0, false);
			Trial t3 = new Trial (3, 0, false);

			twoItemOrigTrials.Add (t2);
			threeItemOrigTrials.Add (t3);

			Trial t2Counter = t2.GetCounterSelf (true);
			Trial t3Counter = t3.GetCounterSelf (true);
			twoItemCounterTrials.Add (t2Counter);
			threeItemCounterTrials.Add (t3Counter);
		}
			
		//now split up the trials into two (shuffled) lists of 16 -- 8 non-stim trials (4 2-item, 4 3-item) and 8 stim trials (4 2-item, 4 3-item) each.
		List<Trial> listOf16A = GetListOf16Trials (twoItemOrigTrials, threeItemOrigTrials, twoItemCounterTrials, threeItemCounterTrials);
		List<Trial> listOf16B = GetListOf16Trials (twoItemOrigTrials, threeItemOrigTrials, twoItemCounterTrials, threeItemCounterTrials);

		//add blocks 2,3,4,5
		AddBlocksFromListOf16 (listOf16A);
		AddBlocksFromListOf16 (listOf16B);
	
	}

	void AddBlocksFromListOf16 (List<Trial> listOf16)
	{

		//ORDER OF TRIAL TYPES IN THE 16 LIST: 4 2-item non-stim, 4 2-item stim, 4 3-item non-stim, 4 3-item stim

		List<Trial> currBlock;

		//HARD CODED.
		int numTwoItemTrialsLeft = 8; //includes stim & non-stim
		int numThreeItemTrialsLeft = 8; //includes stim & non-stim

		if (listOf16.Count == 16) {
			//now split each 16-trial list into two random lists of 8 (each with 4 2-item trials and 4 3-item trials)
			//16A --> block 2, block 3
			currBlock = new List<Trial> ();
			for (int i = 0; i < 4; i++) {
				//pick any 4 2-item trials per block
				int randIndex = Random.Range (0, numTwoItemTrialsLeft);
				currBlock.Add (listOf16 [randIndex]);
				listOf16.RemoveAt (randIndex);
				numTwoItemTrialsLeft--;
			}
			for (int i = 0; i < 4; i++) { //num 3 item trials should be 4
				//pick any 4 3-item trials
				int randIndex = Random.Range (4, 4 + numThreeItemTrialsLeft); //start at 4 because there should still be 4 2-item trials left.
				currBlock.Add (listOf16 [randIndex]);
				listOf16.RemoveAt (randIndex);
				numThreeItemTrialsLeft--;
			}

			ListOfTrialBlocks.Add (currBlock);

			//make second block from this list of 16 -- add the remaining trials to the new block
			currBlock = new List<Trial> ();
			for (int i = 0; i < listOf16.Count; i++) {
				currBlock.Add (listOf16 [i]);
			}
			ListOfTrialBlocks.Add (currBlock);
		} else {
			Debug.Log ("Error: Not exactly 16 trials!");
		}
	}

	//makes a list of 16 -- 8 non-stim trials (4 2-item, 4 3-item) and 8 stim trials (4 2-item, 4 3-item) each.
	List<Trial> GetListOf16Trials (List<Trial> twoItemNonStimList, List<Trial> threeItemNonStimList, List<Trial> twoItemStimList, List<Trial> threeItemStimList)
	{
		//TODO
		List<Trial> listOf16 = new List<Trial> ();

		//add 2-item non stim trials
		for (int i = 0; i < 4; i++) {
			int randIndex = Random.Range (0, twoItemNonStimList.Count);
			listOf16.Add (twoItemNonStimList [randIndex]);
			twoItemNonStimList.RemoveAt (randIndex);
		}

		//add 2-item stim trials
		for (int i = 0; i < 4; i++) {
			int randIndex = Random.Range (0, twoItemStimList.Count);
			listOf16.Add (twoItemStimList [randIndex]);
			twoItemStimList.RemoveAt (randIndex);
		}

		//add 3-item non stim trials
		for (int i = 0; i < 4; i++) {
			int randIndex = Random.Range (0, threeItemNonStimList.Count);
			listOf16.Add (threeItemNonStimList [randIndex]);
			threeItemNonStimList.RemoveAt (randIndex);
		}

		//add 3-item stim trials
		for (int i = 0; i < 4; i++) {
			int randIndex = Random.Range (0, threeItemStimList.Count);
			listOf16.Add (threeItemStimList [randIndex]);
			threeItemStimList.RemoveAt (randIndex);
		}

		return listOf16;
	}

	//ALWAYS NON-STIM. COULD CHANGE THINGS.
	void GenerateTrialsFromFile ()
	{
		StreamReader trialReader = new StreamReader (ExperimentSettings_CoinTask.manualTrialFilePath);

		List<Trial> currBlock = new List<Trial> ();
		int numBlocks = 0;

		string line = trialReader.ReadLine ();
		while (line != null) {
			string[] splitLine = line.Split (','); //CSV

			if (splitLine [0] == "BLOCK") {
				if (numBlocks != 0) { //can initialize again if this isn't the first block.
					currBlock = new List<Trial> ();
				}
				ListOfTrialBlocks.Add (currBlock);
				numBlocks++;
			} else if (splitLine [0] == "TRIAL") {

				int numSpecial = 0;
				if (splitLine [2] == "NUMOBJS") {
					numSpecial = int.Parse (splitLine [3]);
				}

				//make and add manual trials
				Trial trial = new Trial (numSpecial, 0, false); //non-stim.
				//add specific objects to the generated trial
				int objectNameIndex = 5;
				for (int i = 0; i < numSpecial; i++) {
					trial.AddToManualSpecialObjectNames (splitLine [objectNameIndex]);
					objectNameIndex++;
				}
				currBlock.Add (trial);
			}

			line = trialReader.ReadLine ();
		}
		Debug.Log ("GENERATED TRIALS FROM FILE");
	}

	List<Trial> GenerateTrialsWithCounterTrials (int numTrialsToGenerate, int numSpecial, bool shouldStim, bool shouldStimCounter)
	{
		List<Trial> trialList = new List<Trial> ();
		int halfChance = Random.Range (0, 2);
		for (int i = 0; i < numTrialsToGenerate / 2; i++) { //we're adding trial and a counter trial

			//			Trial trial;
			//			if (halfChance == 0) {
			//				trial = new Trial(numSpecial,1, shouldStim);
			//				halfChance = 1;
			//			} else {
			//				trial = new Trial(numSpecial,2, shouldStim);
			//				halfChance = 0;
			//			}
			Trial trial = new Trial (numSpecial, 1, shouldStim);

			Trial anotherTrial = new Trial (numSpecial, 1, shouldStim);
//			Trial counterTrial = trial.GetCounterSelf(shouldStimCounter);
			trialList.Add (trial);
			trialList.Add (anotherTrial);
		}
		//		Trial oneFoilTrial=new Trial(numSpecial,1,shouldStim);
		//		Trial twoFoilTrial = new Trial (numSpecial, 2, shouldStimCounter);
		//
		//		trialList.Add (oneFoilTrial);
		//		trialList.Add (twoFoilTrial);

		Debug.Log ("trials in this list: " + trialList.Count.ToString ());
		return trialList;
	}

	void GenerateTrialBlocks (List<Trial> threeItemTrials, List<Trial> fourItemTrials, int numBlocks, int numTrialsPerBlock)
	{
		for (int i = 0; i < numBlocks; i++) {
			List<Trial> newBlock = new List<Trial> ();
			for (int j = 0; j < 4; j++) { //half two item, half one item
				int randomThreeItemIndex = Random.Range (0, threeItemTrials.Count);
				int randomFourItemIndex = Random.Range (0, fourItemTrials.Count);
				//				int randomFiveItemIndex = Random.Range (0, fiveItemTrials.Count);
				//				int randomSixItemIndex = Random.Range (0, sixItemTrials.Count);

				newBlock.Add (threeItemTrials [randomThreeItemIndex]);
				newBlock.Add (fourItemTrials [randomFourItemIndex]);
				if (Random.value < 0.5f) {
					threeItemTrials [randomThreeItemIndex].trialRecallType = ExperimentSettings_CoinTask.RecallType.Location;
					fourItemTrials [randomFourItemIndex].trialRecallType = ExperimentSettings_CoinTask.RecallType.Object;
				} else {
					threeItemTrials [randomThreeItemIndex].trialRecallType = ExperimentSettings_CoinTask.RecallType.Object;
					fourItemTrials [randomFourItemIndex].trialRecallType = ExperimentSettings_CoinTask.RecallType.Location;
				}
				//				newBlock.Add(fiveItemTrials[randomFiveItemIndex]);
				//				newBlock.Add(sixItemTrials[randomSixItemIndex]);

				threeItemTrials.RemoveAt (randomThreeItemIndex);
				fourItemTrials.RemoveAt (randomFourItemIndex);
				//				fiveItemTrials.RemoveAt(randomFiveItemIndex);
				//				sixItemTrials.RemoveAt(randomSixItemIndex);
			}
			Debug.Log ("trials in this BLOCK: " + newBlock.Count.ToString ());
			ListOfTrialBlocks.Add (newBlock);
		}
	}

	Trial PickRandomTrial (List<Trial> trialBlock)
	{
		if (trialBlock.Count > 0) {
			int randomTrialIndex = Random.Range (0, trialBlock.Count);
			Trial randomTrial = trialBlock [randomTrialIndex];

			trialBlock.RemoveAt (randomTrialIndex);
			return randomTrial;
		} else {
			Debug.Log ("No more trials left!");
			return null;
		}
	}


	
	void Update ()
	{
		if (!isConnectingToHardware) {
#if MRIVERSION
			if(Config_CoinTask.isPractice){ // only pause in MRI practice, not in the real task
				GetPauseInput();
			}
#else
			GetPauseInput ();
#endif
		}
	}

	bool isPauseButtonPressed = false;

	void GetPauseInput ()
	{
		if (Input.GetButtonDown ("Pause Button")) {//.GetAxis(Input.GetKeyDown(KeyCode.B) || Input.GetKey(KeyCode.JoystickButton2)){ //B JOYSTICK BUTTON TODO: move to input manager.
			Debug.Log ("PAUSE BUTTON PRESSED");
			if (!isPauseButtonPressed) {
				isPauseButtonPressed = true;
				Debug.Log ("PAUSE OR UNPAUSE");
				TogglePause (); //pause
			}
		} else {
			isPauseButtonPressed = false;
		}
	}

	public void TogglePause ()
	{
		isPaused = !isPaused;
		trialLogger.LogPauseEvent (isPaused);

		if (isPaused) {
			//exp.player.controls.Pause(true);
			exp.uiController.PauseUI.alpha = 1.0f;
			Time.timeScale = 0.0f;
			TCPServer.Instance.SetState (TCP_Config.DefineStates.PAUSED, true);
		} else {
			Time.timeScale = 1.0f;
			//exp.player.controls.Pause(false);
			//exp.player.controls.ShouldLockControls = false;
			exp.uiController.PauseUI.alpha = 0.0f;
			TCPServer.Instance.SetState (TCP_Config.DefineStates.PAUSED, false);
		}
	}


	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment ()
	{
		if (!ExperimentSettings_CoinTask.isReplay) {
			exp.player.controls.ShouldLockControls = true;

			if (Config_CoinTask.isSystem2 || Config_CoinTask.isSyncbox) {
				yield return StartCoroutine (WaitForEEGHardwareConnection ());
			} else {
				exp.uiController.exitPanel.alpha = 0.0f;
				exp.uiController.ConnectionUI.alpha = 0.0f;
			}
				
#if (!(MRIVERSION))
			#if (!UNITY_WEBPLAYER)
			//		if(!ExperimentSettings_CoinTask.Instance.isWebBuild){
			trialLogger.LogVideoEvent (true);
			exp.instrVideoPlayer.EnableVideo ();
			yield return StartCoroutine (exp.instrVideoPlayer.Play ());
			exp.instrVideoPlayer.DisableVideo ();
			trialLogger.LogVideoEvent (false);
			//		}
			#endif
#endif

			//CREATE SESSION STARTED FILE!
			exp.CreateSessionStartedFile ();

			//show instructions for exploring, wait for the action button
			trialLogger.LogInstructionEvent ();
			yield return StartCoroutine (exp.uiController.pirateController.PlayWelcomingPirate ());

#if MRIVERSION
			if(Config_CoinTask.isPractice){
				yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.initialInstructions1, true, true, false, Config_CoinTask.minInitialInstructionsTime));
				yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.initialInstructions2, true, true, false, Config_CoinTask.minInitialInstructionsTime));
				scoreInstructionsGroup.alpha = 0.0f;
				yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.initialInstructions3, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			}
			else{
				exp.currInstructions.SetInstructionsColorful();
				exp.currInstructions.DisplayText(exp.currInstructions.initialInstructions1);
				scoreInstructionsGroup.alpha = 0.0f;
				yield return StartCoroutine( WaitForMRIConnectionKey());
				exp.currInstructions.SetInstructionsBlank();
				exp.currInstructions.SetInstructionsTransparentOverlay();
				yield return StartCoroutine( WaitForMRIFixationRest());
			}
#else
			Debug.Log ("showing instructions");
			initialInstructions1 = initialInstructions1.Replace ("\n", "\n");
			yield return StartCoroutine (exp.ShowSingleInstruction (initialInstructions1, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			scoreInstructionsGroup.alpha = 1.0f;
			yield return StartCoroutine (exp.ShowSingleInstruction (initialInstructions2, true, true, false, Config_CoinTask.minInitialInstructionsTime));
			scoreInstructionsGroup.alpha = 0.0f;
			//yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.initialInstructions3, true, true, false, Config_CoinTask.minInitialInstructionsTime));
#endif

			#if MRIVERSION
			if(Config_CoinTask.isPractice){
				yield return StartCoroutine(RunPracticeTrials()); //if MRI practice, ONLY run the practice trials
			}
			else{
				yield return StartCoroutine(RunTrials());
			}
			#else
			yield return StartCoroutine (RunPracticeTrials ());
			yield return StartCoroutine (RunTrials ());
			#endif	

			yield return 0;
		}

		yield return StartCoroutine (exp.uiController.pirateController.PlayEndingPirate ());
#if UNITY_WEBPLAYER
		yield return StartCoroutine(exp.ShowSingleInstruction("You have finished your trials! \nPress (X) to restart.", true, true, false, 0.0f));
#else
		yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.youHaveFinishedText, true, true, false, 0.0f));
#endif

#if MRIVERSION
		yield return StartCoroutine( WaitForMRIFixationRest());
#endif
	}

	IEnumerator RunPracticeTrials ()
	{
		//run practice trials
		if (Config_CoinTask.isPractice) {
			isPracticeTrial = true;
		}

		if (isPracticeTrial) {
			int numPracticeTrialsRun = 0;
			while (numPracticeTrialsRun < practiceTrials.Count) {

				yield return StartCoroutine (RunTrial (practiceTrials [numPracticeTrialsRun]));
				numPracticeTrialsRun++;
			}
			Debug.Log ("PRACTICE TRIALS COMPLETED");
			isPracticeTrial = false;
		}
	}

	IEnumerator RunTrials ()
	{
		//RUN THE REST OF THE BLOCKS
		for (int i = 0; i < ListOfTrialBlocks.Count; i++) {
			List<Trial> currentTrialBlock = ListOfTrialBlocks [i];
			while (currentTrialBlock.Count > 0) {
				Trial nextTrial = PickRandomTrial (currentTrialBlock);

				yield return StartCoroutine (RunTrial (nextTrial));

			}

			//FINISHED A TRIAL BLOCK, SHOW UI
			trialLogger.LogInstructionEvent ();
			exp.uiController.blockCompletedUI.Play (i, exp.scoreController.score, ListOfTrialBlocks.Count);
			trialLogger.LogBlockScreenStarted (true);
			TCPServer.Instance.SetState (TCP_Config.DefineStates.BLOCKSCREEN, true);

			yield return StartCoroutine (exp.WaitForActionButton ());

			exp.uiController.blockCompletedUI.Stop ();
			trialLogger.LogBlockScreenStarted (false);

			TCPServer.Instance.SetState (TCP_Config.DefineStates.BLOCKSCREEN, false);
			if (i < ListOfTrialBlocks.Count - 1)
				yield return StartCoroutine (exp.uiController.pirateController.PlayEncouragingPirate ());
			exp.scoreController.Reset ();

			Debug.Log ("TRIAL Block: " + i);
		}
	}

	IEnumerator WaitForEEGHardwareConnection ()
	{
		isConnectingToHardware = true;
		Debug.Log ("waiting for eeg connection");
		exp.uiController.ConnectionUI.alpha = 1.0f;
		if (Config_CoinTask.isSystem2) {
			while (!TCPServer.Instance.isConnected) {
				Debug.Log ("Waiting for system 2 connection...");
				yield return 0;
			}
			exp.uiController.ConnectionText.text = "Press START on host PC...";
			while (!TCPServer.Instance.canStartGame) {
				Debug.Log ("Waiting for system 2 start command...");
				yield return 0;
			}
		}
		if (Config_CoinTask.isSyncbox) {
			while (!SyncboxControl.Instance.isUSBOpen) {
				//Debug.Log("Waiting for sync box to open...");
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

	bool isMRITimeout = false;
	public IEnumerator WaitForMRITimeout(float maxSeconds){
		exp.uiController.MRITimerUI.alpha = 1.0f;
		MRITimer.ResetTimerNoDelegate(maxSeconds);
		MRITimer.StartTimer ();

		Debug.Log("MRI TIMEOUT STARTED");

		bool hasPressedButton = false;

		float actionInput = Input.GetAxis(Config_CoinTask.ActionButtonName);
		float currSeconds = MRITimer.GetSecondsFloat();
		while(currSeconds > 0.0f && actionInput != 0.0f){ //if button is down, must lift up before we can continue
			Debug.Log(actionInput);
			currSeconds = MRITimer.GetSecondsFloat();
			actionInput = Input.GetAxis(Config_CoinTask.ActionButtonName);
			yield return 0;
		}
		while(!hasPressedButton && MRITimer.GetSecondsFloat() > 0.0f){
			if(Input.GetAxis(Config_CoinTask.ActionButtonName) == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}

		if(MRITimer.GetSecondsFloat() <= 0.0f){
			isMRITimeout = true;
			trialLogger.LogMRITimeout();
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

			trialLogger.LogPlayerNavigationTimeout();
			trialLogger.LogPlayerAutodrive(true);

			Vector2 currChestPos = currentTrial.DefaultObjectLocationsXZ[currNumCollected];
			Vector3 targetChestPos = new Vector3(currChestPos.x, exp.player.transform.position.y, currChestPos.y);

			float collisionBuffer = Experiment_CoinTask.Instance.objectController.GetMaxDefaultObjectColliderBoundXZ ();
			Vector3 direction = targetChestPos - exp.player.transform.position;
			targetChestPos = targetChestPos - (direction.normalized*collisionBuffer);

			Quaternion desiredRot = UsefulFunctions.GetDesiredRotation(exp.player.transform, targetChestPos);

			yield return StartCoroutine(exp.player.controls.SmoothMoveTo(targetChestPos, desiredRot, true));

			//wait until we've actually collected the chest
			while(currNumCollected == numDefaultObjectsCollected){
				yield return 0;
				if(!exp.player.controls.ShouldLockControls){
					float tinyMovement = 0.1f;
					exp.player.transform.position += (exp.player.transform.forward*tinyMovement);
				}
			}

			trialLogger.LogPlayerAutodrive(false);
		}

		MRITimer.StopTimer ();
		exp.uiController.MRITimerUI.alpha = 0.0f;
	}
	#endif

	//GETS CALLED IN WAITFORTREASUREPAUSE() -- this is when treasure is paused in its open state.
	//...one the pause for encoding ends, we increment.
	public void IncrementNumDefaultObjectsCollected ()
	{
		numDefaultObjectsCollected++;
		if (ExperimentSettings_CoinTask.isOneByOneReveal) {
			CreateNextDefaultObject (numDefaultObjectsCollected);
		}
	}

	void CreateNextDefaultObject (int currentIndex)
	{
		if (currentTrial != null) {
			//SetUpNextDefaultObject (currentIndex);
			if (currentIndex < currentTrial.DefaultObjectLocationsXZ.Count) {

				Vector2 positionXZ = currentTrial.DefaultObjectLocationsXZ [currentIndex];
				currentDefaultObject = exp.objectController.SpawnDefaultObject (positionXZ, currentTrial.SpecialObjectLocationsXZ, currentIndex, currentTrial.trialRecallType);
			} else {
				Debug.Log ("Can't create a default object at that index. Index is too big.");
			}
		}
	}

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	IEnumerator RunTrial (Trial trial)
	{

		currentTrial = trial;
		Debug.Log ("RECALL TYPE: " + currentTrial.trialRecallType.ToString ());
		if (currentTrial.trialRecallType == ExperimentSettings_CoinTask.RecallType.Object) {
			exp.objectController.CreateFoilObjects ();
			trialLogger.LogTrialType (ExperimentSettings_CoinTask.RecallType.Object);
		} else {
			trialLogger.LogTrialType (ExperimentSettings_CoinTask.RecallType.Location);
		}
		

		if (isPracticeTrial) {
			trialLogger.Log (-1, currentTrial.DefaultObjectLocationsXZ.Count, currentTrial.SpecialObjectLocationsXZ.Count, ExperimentSettings_CoinTask.isOneByOneReveal, false);
			Debug.Log ("Logged practice trial.");
		} else {
			trialLogger.Log (numRealTrials, currentTrial.DefaultObjectLocationsXZ.Count, currentTrial.SpecialObjectLocationsXZ.Count, ExperimentSettings_CoinTask.isOneByOneReveal, currentTrial.isStim);
			numRealTrials++;
			TCPServer.Instance.SendTrialNum (numRealTrials);
			Debug.Log ("Logged trial #: " + numRealTrials);
		}

		//move player to home location & rotation
		trialLogger.LogTransportationToHomeEvent (true, currentTrial.avatarStartPos);
//		Vector3 avatarStartPos = new Vector3 (Random.Range (exp.environmentController.WallsXNeg.position.x+5f, exp.environmentController.WallsXPos.position.x-5f), currentTrial.avatarStartPos.y, Random.Range (exp.environmentController.WallsZNeg.position.z+5f, exp.environmentController.WallsZPos.position.z-5f));

		Debug.Log ("transported to : " + currentTrial.avatarStartPos.ToString ());
		trialLogger.LogStartPosition (currentTrial.avatarStartPos);

		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarStartPos, currentTrial.avatarStartRot, false));
		trialLogger.LogTransportationToHomeEvent (false, currentTrial.avatarStartPos);
		Debug.Log ("done transporting");



		//disable selection
		exp.environmentController.myPositionSelector.EnableSelection (false);

		exp.player.controls.ShouldLockControls = true;

		//reset game timer
		trialTimer.ResetTimer (0);

		if (numRealTrials > 1) { //note: if numRealTrials > 1, not a practice trial.
			trialLogger.LogInstructionEvent ();

			#if !(MRIVERSION)
			yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.pressToStart, true, true, false, Config_CoinTask.minDefaultInstructionTime));
			#endif
		}

		//START NAVIGATION --> TODO: make this its own function. or a delegate. ...clean it up.
		trialLogger.LogTrialNavigation (true);
		if (currentTrial.isStim) {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.STIM_NAVIGATION, true);
		} else {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.NAVIGATION, true);
		}

		exp.uiController.goText.Play ();

		//start a game timer
		trialTimer.StartTimer ();

		//unlock avatar controls
		exp.player.controls.ShouldLockControls = false;

		if (ExperimentSettings_CoinTask.isOneByOneReveal) {
			//Spawn the first default object
			CreateNextDefaultObject (0);
		} else {
			exp.objectController.SpawnDefaultObjects (currentTrial.DefaultObjectLocationsXZ, currentTrial.SpecialObjectLocationsXZ, currentTrial.trialRecallType);
		}

		//wait for player to collect all default objects
		int numDefaultObjectsToCollect = currentTrial.DefaultObjectLocationsXZ.Count;

		#if MRIVERSION
		for(int i = 0; i <numDefaultObjectsToCollect; i++){
			Debug.Log("WAIT FOR NAVIGATION TIMEOUT");
			yield return StartCoroutine(WaitForMRINavigationTimeout(Config_CoinTask.maxChestNavigationTime));
		}
		#else //if not MRI version, just wait until all chests are collected;
		while (numDefaultObjectsCollected < numDefaultObjectsToCollect) {
			yield return 0;
		}
		#endif

		//Add time bonus
		trialTimer.StopTimer ();
		timeBonus = exp.scoreController.CalculateTimeBonus (trialTimer.GetSecondsInt ());

		//reset num default objects collected
		numDefaultObjectsCollected = 0;

		//lock player movement
		exp.player.controls.ShouldLockControls = true;

		//bring player to tower
		//exp.player.TurnOnVisuals (false);
		trialLogger.LogTrialNavigation (false);
		if (currentTrial.isStim) {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.STIM_NAVIGATION, false);
		} else {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.NAVIGATION, false);
		}
		trialLogger.LogTransportationToTowerEvent (true);
		currentDefaultObject = null; //set to null so that arrows stop showing up...
		yield return StartCoroutine (exp.player.controls.SmoothMoveTo (currentTrial.avatarTowerPos, currentTrial.avatarTowerRot, false));//PlayerControls.toTowerTime) );
		trialLogger.LogTransportationToTowerEvent (false);

		//RUN DISTRACTOR GAME
		trialLogger.LogDistractorGame (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.DISTRACTOR, true);
		yield return StartCoroutine (exp.boxGameController.RunGame ());
		trialLogger.LogDistractorGame (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.DISTRACTOR, false);

		//jitter before the first object is shown
		yield return StartCoroutine (exp.WaitForJitter (Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));

		//show instructions for location selection 
		trialLogger.LogRecallPhaseStarted (true);
		List<int> randomSpecialObjectOrder = new List<int> ();
		List<Vector3> chosenPositions = new List<Vector3> (); //chosen positions will be in the same order as the random special object order


//TH1 style


		if (currentTrial.trialRecallType == ExperimentSettings_CoinTask.RecallType.Location) {
			//ask player to locate each object individually
			//List<Config_CoinTask.MemoryState> rememberResponses = new List<Config_CoinTask.MemoryState> (); //keep track of whether or not the player remembered each object
			//List<bool> areYouSureResponses = new List<bool> (); //keep track of whether or not the player wanted to double down on each object

			randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder (exp.objectController.CurrentTrialSpecialObjects.Count);
			for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {

				//show instructions for location selection
				int randomOrderIndex = randomSpecialObjectOrder [i];
				GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
				SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject> ();

				//changed for german version, we will get the name with appropriate articles to display during the question
				string specialItemDisplayName = specialSpawnable.GermanName;
				//string specialItemDisplayName = specialSpawnable.GetDisplayName ();

//			//set TCP state true
//			switch(randomOrderIndex){
//			case 0:
//				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_1, true);
//				break;
//			case 1:
//				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_2, true);
//				break;
//			case 2:
//				TCPServer.Instance.SetState (TCP_Config.DefineStates.RECALLCUE_3, true);
//				break;
//			}

				//show nice UI, log special object
				trialLogger.LogObjectToRecall (specialSpawnable);
//			GameObject specialObjUICopy = Instantiate (specialObj, Vector3.zero, specialObj.transform.rotation) as GameObject;
//			specialObjUICopy.name += "UICopy";
//
//			specialObjUICopy.transform.parent = exp.cameraController.UICamera.transform; //make this copy follow camera/head movement. mainly for VR.
//
//			//set layer of object & children to PlayerUI
//			specialObjUICopy.GetComponent<SpawnableObject>().SetLayer ("PlayerUI");
//
//			trialLogger.LogInstructionEvent();
//			yield return StartCoroutine( exp.uiController.doYouRememberUI.Play(specialObjUICopy, specialItemDisplayName) );

#if MRIVERSION
			Config_CoinTask.MemoryState rememberResponse;
			isMRITimeout = false;
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxAnswerTime));
			if(isMRITimeout){
				rememberResponse = Config_CoinTask.MemoryState.no;
			}
			else{
				rememberResponse = exp.uiController.doYouRememberUI.myAnswerSelector.GetMemoryState();
			}
			rememberResponses.Add(rememberResponse);
			trialLogger.LogRememberResponse(rememberResponse);
#else
//			yield return StartCoroutine (exp.WaitForActionButton());
//			Config_CoinTask.MemoryState rememberResponse = exp.uiController.doYouRememberUI.myAnswerSelector.GetMemoryState();
//			rememberResponses.Add(rememberResponse);
//			trialLogger.LogRememberResponse(rememberResponse);
#endif



				//SELECT LOCATION
				//enable position selection, turn off fancy selection UI
				exp.environmentController.myPositionSelector.Reset ();
				exp.environmentController.myPositionSelector.EnableSelection (true);

				switch (randomOrderIndex) {
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
				trialLogger.LogRecallChoiceStarted (true, 2);

//			exp.uiController.doYouRememberUI.Stop();

				//show single selection instruction and wait for selection button press
				string selectObjectText = exp.currInstructions.selectTheLocationText;
				if (ExperimentSettings_CoinTask.myLanguage == ExperimentSettings_CoinTask.LanguageSetting.Spanish) {
					//check for masculine article
					string[] splitName = specialItemDisplayName.Split (' ');
					if (splitName [0] == "el") {
						string displayNameNoEl = specialItemDisplayName.Remove (0, 3);
						selectObjectText = selectObjectText + "l" + " " + displayNameNoEl + "? (X)."; //add the 'l' to "de" to make "del"
					} else {
						selectObjectText = selectObjectText + " " + specialItemDisplayName + "? (X).";
					}
				} else { //english
					selectObjectText = selectObjectText + " " + specialItemDisplayName + "? (X).";
				}

#if MRIVERSION
			exp.currInstructions.SetInstructionsTransparentOverlay();
			exp.currInstructions.DisplayText(selectObjectText);
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxLocationChooseTime));
			exp.currInstructions.SetInstructionsBlank();
#else
				exp.uiController.instructionPanel.SetActive (true);
				yield return StartCoroutine (exp.ShowSingleInstruction (selectObjectText, false, true, false, Config_CoinTask.minDefaultInstructionTime));
				exp.uiController.instructionPanel.SetActive (false);
#endif
				//log the chosen position and correct position
				exp.environmentController.myPositionSelector.logTrack.LogPositionChosen (exp.environmentController.myPositionSelector.GetSelectorPosition (), specialObj.transform.position, specialSpawnable);

				//wait for the position selector to choose the position, runs color changing of the selector
				yield return StartCoroutine (exp.environmentController.myPositionSelector.ChoosePosition ());


				//add current chosen position to list of chosen positions
				chosenPositions.Add (exp.environmentController.myPositionSelector.GetSelectorPosition ());

				//disable position selection
				exp.environmentController.myPositionSelector.EnableSelection (false);
				trialLogger.LogRecallChoiceStarted (false, 2);

				switch (randomOrderIndex) {
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


				trialLogger.LogInstructionEvent ();

				if (i <= exp.objectController.CurrentTrialSpecialObjects.Count - 1) {
					//jitter if it's not the last object to be shown
					yield return StartCoroutine (exp.WaitForJitter (Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax));
				}

			}

			//temporal retrieval
			trialLogger.LogTemporalRetrievalStarted ();
			yield return StartCoroutine (RunTemporalRetrieval ());
			trialLogger.LogTemporalRetrievalEnded ();
//			RunTemporalRetrieval();
			trialLogger.LogRecallPhaseStarted (false);

			yield return StartCoroutine (ShowFeedback (randomSpecialObjectOrder, chosenPositions));

		}


		//THR-style
		else if (currentTrial.trialRecallType == ExperimentSettings_CoinTask.RecallType.Object) {
			randomSpecialObjectOrder = UsefulFunctions.GetRandomIndexOrder (numDefaultObjectsToCollect);
			List<Vector3> correctPositions = new List<Vector3> ();
			List<bool> isFoil = new List<bool> ();
			List<int> recallAnswers = new List<int> ();

			string trialLstContents = "";
			int foilObjectsAdded = 0;
			int specialObjectsAdded = 0;
			//List<GameObject> recallObjects = new List<GameObject> ();
			for (int i = 0; i < randomSpecialObjectOrder.Count; i++) {

				int halfChance = Random.Range (0, 2);
				if (halfChance == 1 && specialObjectsAdded < currentTrial.SpecialObjectLocationsXZ.Count) {
					exp.objectController.RecallObjectList.Add (exp.objectController.CurrentTrialSpecialObjects [specialObjectsAdded++]);
					isFoil.Add (false);
				} else if (foilObjectsAdded < exp.objectController.CurrentTrialFoilObjects) {
					exp.objectController.RecallObjectList.Add (exp.objectController.FoilObjects [foilObjectsAdded++]);
					isFoil.Add (true);
				} else {
					exp.objectController.RecallObjectList.Add (exp.objectController.CurrentTrialSpecialObjects [specialObjectsAdded++]);
					isFoil.Add (false);
				}
			}
			Debug.Log ("number of recall objects is: " + exp.objectController.RecallObjectList.Count + " and should be" + randomSpecialObjectOrder.Count);
			Debug.Log ("number of special objects is: " + specialObjectsAdded + "/" + exp.objectController.CurrentTrialSpecialObjects.Count);
			Debug.Log ("number of foil objects is: " + foilObjectsAdded + "/" + exp.objectController.CurrentTrialFoilObjects);
			for (int i = 0; i < exp.objectController.RecallObjectList.Count; i++) {
				correctPositions.Add (exp.objectController.RecallObjectList [randomSpecialObjectOrder [i]].transform.position);
				recallAnswers.Add (0);
				trialLstContents += exp.objectController.RecallObjectList [randomSpecialObjectOrder [i]].GetComponent<SpawnableObject> ().GetDisplayName () + "\n";
			}

			List<Config_CoinTask.MemoryState> rememberResponses = new List<Config_CoinTask.MemoryState> (); //keep track of whether or not the player remembered each object
			//List<bool> areYouSureResponses = new List<bool> (); //keep track of whether or not the player wanted to double down on each object
			List<int> recallTypes = new List<int> ();


			for (int i = 0; i < exp.objectController.RecallObjectList.Count; i++) {

				//show instructions for location selection
				int randomOrderIndex = randomSpecialObjectOrder [i];
				GameObject specialObj = exp.objectController.RecallObjectList [randomOrderIndex];
				Vector3 specialObjPosition = correctPositions [randomOrderIndex];
				SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject> ();
				string specialItemDisplayName = specialSpawnable.GetDisplayName ();
				string kws_threshold = specialSpawnable.sphinxThreshold;
				int currentRecallNumber = i;
		

				objectRecall = false;
//			recallTypes.Add (1);
				Debug.Log ("starting object recall");
				exp.environmentController.myPositionSelector.EnableObjectSelectorVisuals (true);
				Debug.Log ("this object is: " + specialObj.name);
				exp.environmentController.myPositionSelector.MoveObjectIndicatorToPosition (specialObj.transform.position);
				string currentRecallObject = specialItemDisplayName.ToLower ();
				chosenPositions.Add (specialObj.transform.position);
				//yield return new WaitForSeconds (2f);
				//trialLogger.LogInstructionEvent ();
				//yield return StartCoroutine (exp.uiController.doYouRememberObjectUI.PlayObjectRecall());
				#if MRIVERSION
			Config_CoinTask.MemoryState rememberResponse;
			isMRITimeout = false;
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxAnswerTime));
			if(isMRITimeout){
			rememberResponse = Config_CoinTask.MemoryState.no;
			}
			else{
			rememberResponse = exp.uiController.doYouRememberUI.myAnswerSelector.GetMemoryState();
			}
			rememberResponses.Add(rememberResponse);
			trialLogger.LogRememberResponse(rememberResponse);
				#else
				#endif
				//yield return StartCoroutine (exp.WaitForActionButton ());
				//				Config_CoinTask.MemoryState rememberResponse = exp.uiController.doYouRememberObjectUI.myAnswerSelector.GetMemoryState ();
				//				rememberResponses.Add (rememberResponse);
				//				trialLogger.LogRememberResponse (rememberResponse);
				//
				//				#endif
				////
				//			
				trialLogger.LogRecallChoiceStarted (true, 1);
				//
				//				exp.uiController.doYouRememberObjectUI.Stop ();

				//show single selection instruction and wait for selection button press
				string selectObjectText = exp.currInstructions.selectTheLocationText;
				selectObjectText = "Welcher Gegenstand war an diesem Ort?";
				//selectObjectText = "Name the object remembered in this location";
				exp.currInstructions.SetTextPanelOn ();


				#if MRIVERSION
			exp.currInstructions.SetInstructionsTransparentOverlay();
			exp.currInstructions.DisplayText(selectObjectText);
			yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxLocationChooseTime));
			exp.currInstructions.SetInstructionsBlank();
				#else


				//set your own instructions
				exp.currInstructions.SetInstructionsTransparentOverlay ();
				exp.currInstructions.DisplayText (selectObjectText);

				//yield return new WaitForSeconds(3f);
				#endif


				int logTrialNumber = i; //just the object number for this current trial

				string fileName = totalTrialNumber.ToString () + "_" + logTrialNumber.ToString (); //totalTrialNumber_logTrialNumber



				//DO AUDIO RECORDING
				//exp.environmentController.myPositionSelector.StartTimer ();
				//play on record beep
				exp.audioController.audioRecorder.beepHigh.Play ();

				//initialize it to zero; it will remain this if the keyword isn't detected
				recallAnswers [randomOrderIndex] = 0;

				//Create keywords for keyword recognizer
				keywords.Add (currentRecallObject, () => {
					// action to be performed when this keyword is spoken
					Debug.Log ("found " + currentRecallObject);
					recallAnswers [randomOrderIndex] = 1;
				});
				keywordRecognizer = new KeywordRecognizer (keywords.Keys.ToArray ());
				keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

				keywordRecognizer.Start ();
				UnityEngine.Debug.Log ("STARTED KEYWORD REC for" + currentRecallObject);
			
				//start recording
				yield return StartCoroutine (exp.audioController.audioRecorder.Record (exp.sessionDirectory + "audio", fileName, recallTime));

				//play off beep
				exp.audioController.audioRecorder.beepLow.Play ();

				//turn off instructions
				exp.currInstructions.TurnOffInstructions ();
				exp.cameraController.SetInGame ();
				exp.currInstructions.SetTextPanelOff ();

				//make a .TXT file with name of current recall object; meant for pocketsphinx
				string trialTxtName = exp.sessionDirectory + "audio/" + fileName + ".txt";
				System.IO.File.WriteAllText (trialTxtName, currentRecallObject); //includes name and the kws_threshold 

				//make a .LST file with all the names of the objects in the trial
				string trialLstName = exp.sessionDirectory + "audio/" + fileName + ".lst";
				System.IO.File.WriteAllText (trialLstName, trialLstContents);


				//disable position selection
				exp.environmentController.myPositionSelector.EnableObjectSelectorVisuals (false);
				trialLogger.LogRecallChoiceStarted (false, 1);

				//check audio response
				//UnityEngine.Debug.Log ("CHECKING SPHINX RESPONSE: " + totalTrialNumber + " and  " + currentRecallNumber);
				//int sphinxNum = totalTrialNumber;
				//recallAnswers [randomOrderIndex] = exp.sphinxTest.CheckAudioResponse (sphinxNum, currentRecallNumber, currentRecallObject, kws_threshold);

				trialLogger.LogSphinxEvent ();

				trialLogger.LogInstructionEvent ();

				if (i <= exp.objectController.RecallObjectList.Count - 1) {
					//jitter if it's not the last object to be shown
					yield return StartCoroutine (exp.WaitForJitter (Config_CoinTask.randomPALJitterMin, Config_CoinTask.randomPALJitterMax));
					yield return StartCoroutine (exp.WaitForISI (Config_CoinTask.isiTime));
					Debug.Log ("waited for jitter and ISI time");
				}
				#if !UNITY_EDITOR_OSX
				keywords.Clear ();
				#endif
				Debug.Log ("moving to the position");
			} 

	
			//temporal retrieval
			trialLogger.LogTemporalRetrievalStarted ();
			yield return StartCoroutine (RunTemporalRetrieval ());
			trialLogger.LogTemporalRetrievalEnded ();
//			RunTemporalRetrieval();
			trialLogger.LogRecallPhaseStarted (false);

			yield return StartCoroutine (ShowFeedback (randomSpecialObjectOrder, chosenPositions, isFoil, recallAnswers));

		}

		//perform reconnection check
		if (EyetrackerManager.shouldCheckHead) {
			Debug.Log ("initiating reconnection");
			yield return StartCoroutine (InitiateEyetrackerReconnection ());
		}
		totalTrialNumber++;

		//increment subject's trial count
#if !UNITY_WEBPLAYER
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();
#endif
	}

	int currTrialNum = 0;

	private void KeywordRecognizer_OnPhraseRecognized (PhraseRecognizedEventArgs args)
	{
		System.Action keywordAction;
		// if the keyword recognized is in our dictionary, call that Action.
		if (keywords.TryGetValue (args.text, out keywordAction)) {
			keywordAction.Invoke ();
		}
	}

	IEnumerator RunTemporalRetrieval ()
	{
		//temporal retrieval
		Debug.Log ("number of objects this list: " + exp.objectController.CurrentTrialSpecialObjects.Count.ToString ());
		//only use the TH1 shuffling as it doesn't contain any foil objects
		List<GameObject> temporalList = new List<GameObject> ();
		for (int j = 0; j < exp.objectController.CurrentTrialSpecialObjects.Count; j++) {
			temporalList.Add (exp.objectController.CurrentTrialSpecialObjects [j]);
		}
		List<GameObject> tupleList = new List<GameObject> ();
		//create a tuple list that's unique
		tupleList = CreateTuples (temporalList);
			
		int tupleCount = tupleList.Count / 2;
		Debug.Log ("there are " + tupleCount.ToString () + " tuples");
		List<int> randomTemporalOrder = UsefulFunctions.GetRandomIndexOrder (tupleCount);

		for (int i = 0; i < tupleCount; i++) {
			int indexA = 2 * randomTemporalOrder [i];
			Debug.Log ("index A: " + indexA.ToString ());
			GameObject optionA = tupleList [indexA];
			int indexB = (2 * randomTemporalOrder [i]) + 1;
			Debug.Log ("index B: " + indexB.ToString ());
			GameObject optionB = tupleList [indexB];
			Debug.Log ("option A : " + optionA.name + " and Option B: " + optionB.name);
			GameObject optionACopy = Instantiate (optionA, Vector3.zero, optionA.transform.rotation) as GameObject;
			string optionAName = optionA.GetComponent<SpawnableObject> ().ShortenedGermanName;
			optionACopy.name = optionAName + " UICopy";
			optionACopy.transform.parent = exp.cameraController.UICamera.transform; //make this copy follow camera/head movement. mainly for VR.
			//set layer of object & children to PlayerUI
			optionACopy.GetComponent<SpawnableObject> ().SetLayer ("PlayerUI");

			GameObject optionBCopy = Instantiate (optionB, Vector3.zero, optionB.transform.rotation) as GameObject;
			string optionBName = optionB.GetComponent<SpawnableObject> ().ShortenedGermanName;
			optionBCopy.name = optionBName + " UICopy";
			optionBCopy.transform.parent = exp.cameraController.UICamera.transform; //make this copy follow camera/head movement. mainly for VR.
			//set layer of object & children to PlayerUI
			optionBCopy.GetComponent<SpawnableObject> ().SetLayer ("PlayerUI");

			Debug.Log ("starting the questionUI coroutine now");

			//do a coin toss to decide whether to flip locations of the options
			int halfChance = Random.Range (0, 2);
			if (halfChance == 0) {
				yield return StartCoroutine (exp.uiController.temporalRetrievalUI.Play (optionACopy, optionBCopy, optionAName, optionBName));
			} else {
				yield return StartCoroutine (exp.uiController.temporalRetrievalUI.Play (optionBCopy, optionACopy, optionBName, optionAName));
			}
			yield return StartCoroutine (exp.WaitForActionButton ());

			int correctAnswerIndex = -1;

			if (indexA > indexB) {
				Debug.Log (optionA.name + " is more recent");
				correctAnswerIndex = 0;
			} else {
				Debug.Log (optionB.name + " is more recent");
				correctAnswerIndex = 1;
			}

			//flip the correct answer because we flipped when we asked the question
			if (halfChance == 1) {
				correctAnswerIndex = 1 - correctAnswerIndex;
				Debug.Log ("got flipped");
				exp.uiController.temporalRetrievalUI.flippedList.Add (1);
			} else {
				Debug.Log ("not flipped");
				exp.uiController.temporalRetrievalUI.flippedList.Add (0);
			}

			int selectedAnswerIndex = exp.uiController.temporalRetrievalUI.myAnswerSelector.GetAnswerPosition ();
			Debug.Log ("selected: " + selectedAnswerIndex.ToString () + " and correct: " + correctAnswerIndex.ToString ());
			if (selectedAnswerIndex != correctAnswerIndex) {
				Debug.Log ("mark as incorrect answer");
				exp.uiController.temporalRetrievalUI.scoreList.Add (-1);
			} else {
				Debug.Log ("correct answer");
				exp.uiController.temporalRetrievalUI.scoreList.Add (1);
			}
			exp.uiController.temporalRetrievalUI.answerList.Add (correctAnswerIndex);
			Debug.Log ("stored as correct answer: " + correctAnswerIndex.ToString ());
			//temporarily stop and destroy the spawned objects
			exp.uiController.temporalRetrievalUI.Stop ();
		}

		yield return null;
	}

	List<GameObject> CreateTuples (List<GameObject> objList)
	{
		List<GameObject> tupleList = new List<GameObject> ();
		for (int i = 0; i < objList.Count; i++) {
			for (int j = i + 1; j < objList.Count; j++) {
				tupleList.Add (objList [i]);
				tupleList.Add (objList [j]);
			}
		}
		for (int k = 0; k < tupleList.Count; k++) {
			Debug.Log (k.ToString () + ": " + tupleList [k].name);
		}
		return tupleList;
	}

	IEnumerator ShowFeedback (List<int> specialObjectOrder, List<Vector3> chosenPositions)
	{
		trialLogger.LogFeedback (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, true);

		memoryScore = 0;
		temporalScore = 0;
		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		List<GameObject> specialObjectListRecallOrder = new List<GameObject> ();

		List<int> objectScores = new List<int> ();

		for (int i = 0; i < specialObjectOrder.Count; i++) {

			Vector3 chosenPosition = chosenPositions [i];

			//throw bomb to selected location
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't actually want its visuals showing up as we wait

#if !(MRIVERSION)
			yield return StartCoroutine (exp.objectController.ThrowExplosive (exp.player.transform.position, chosenPosition, i));
#endif

			int randomOrderIndex = specialObjectOrder [i];

			//turn on each special object & scale up for better visibility
			GameObject specialObj = exp.objectController.CurrentTrialSpecialObjects [randomOrderIndex];
			specialObjectListRecallOrder.Add (specialObj);

			SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject> ();
			specialSpawnable.TurnVisible (true);
			specialSpawnable.Scale (2.0f);
			UsefulFunctions.FaceObject (specialObj, exp.player.gameObject, false);
			
			//create an indicator for each special object
			float indicatorHeight = exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.position.y;
			Vector3 correctPosition = new Vector3 (specialObj.transform.position.x, indicatorHeight, specialObj.transform.position.z);
			GameObject correctPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.CorrectPositionIndicator, correctPosition, exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.rotation) as GameObject;
			correctPositionIndicator.GetComponent<SpawnableObject> ().SetNameID (correctPositionIndicator.transform, i);
			CorrectPositionIndicators.Add (correctPositionIndicator); 

			//create an indicator for each chosen position
			//spawn the indicator at the height of the original indicator
			exp.environmentController.myPositionSelector.EnableSelection (true); //turn on selector for spawning indicator
			float chosenIndicatorHeight = exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y;
			Vector3 chosenIndicatorPosition = new Vector3 (chosenPosition.x, chosenIndicatorHeight, chosenPosition.z);
			GameObject chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;

			chosenPositionIndicator.GetComponent<SpawnableObject> ().SetNameID (chosenPositionIndicator.transform, i);
			chosenPositionIndicator.GetComponent<VisibilityToggler> ().TurnVisible (true);


			ChosenPositionIndicators.Add (chosenPositionIndicator);
			
			//calculate the memory points and display them
			exp.environmentController.myPositionSelector.PositionSelector.transform.position = chosenPosition;

			int points = exp.scoreController.CalculateMemoryPoints (specialObj.transform.position);//, areYouSureResponses[i] );

			//change chosen indicator color to reflect right or wrong
			ChosenIndicatorController chosenIndicatorController = chosenPositionIndicator.GetComponent<ChosenIndicatorController> ();
			Color chosenPositionColor = chosenIndicatorController.RightColor;
			if (points > 0) {
				chosenIndicatorController.ChangeToRightColor ();
			} else if (points <= 0) {
				chosenIndicatorController.ChangeToWrongColor ();
				chosenPositionColor = chosenIndicatorController.WrongColor;
			}


			//connect the chosen and correct indicators via a line
			SetConnectingLines (correctPositionIndicator, chosenPosition, chosenPositionColor);


			CorrectPositionIndicatorController correctPosController = correctPositionIndicator.GetComponent<CorrectPositionIndicatorController> ();

			correctPosController.SetPointsText (points);
			memoryScore += points;

			objectScores.Add (points);


			//WAIT BEFORE NEXT FEEDBACK
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't want its visuals showing up as we wait
#if !(MRIVERSION)
			yield return new WaitForSeconds (Config_CoinTask.feedbackTimeBetweenObjects);
#endif
		}
		
		//disable original selector
		exp.environmentController.myPositionSelector.EnableSelection (false);

#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxFeedbackTime));
#else
		//wait for selection button press
		exp.uiController.instructionPanel.SetActive (true);
		exp.currInstructions.SetTextPanelOn ();
		yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.pressToContinue, false, true, false, Config_CoinTask.minDefaultInstructionTime));
		exp.currInstructions.SetTextPanelOff ();
		exp.uiController.instructionPanel.SetActive (false);
#endif

		currTrialNum++;


		trialLogger.LogInstructionEvent ();


		//temporal feedback
		trialLogger.LogTemporalFeedbackStarted ();
		yield return StartCoroutine (exp.uiController.temporalRetrievalUI.ShowAnswer ());
		trialLogger.LogTemporalFeedbackEnded ();

		trialLogger.LogScoreScreenStarted (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, true);
		exp.uiController.scoreRecapUI.Play (currTrialNum, timeBonus + memoryScore + temporalScore, Config_CoinTask.GetTotalNumTrials (), objectScores, specialObjectListRecallOrder, timeBonus, temporalScore, trialTimer.GetSecondsFloat ());

#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxScoreScreenTime));
#else
		yield return StartCoroutine (exp.WaitForActionButton ());
#endif

		exp.uiController.scoreRecapUI.Stop ();
		trialLogger.LogScoreScreenStarted (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, false);


		//delete all indicators & special objects
		DestroyGameObjectList (CorrectPositionIndicators);
		DestroyGameObjectList (ChosenPositionIndicators);
		DestroyGameObjectList (exp.objectController.CurrentTrialSpecialObjects);

		trialLogger.LogFeedback (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, false);

		yield return 0;
	}


	IEnumerator ShowFeedback (List<int> specialObjectOrder, List<Vector3> chosenPositions, List<bool>isFoil, List<int>recallAnswers)
	{
		trialLogger.LogFeedback (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, true);

		int correctRecallCues = 0; 
		int totalRecallCues = 0;
		int consecutiveScore = 0;
		memoryScore = 0;
		temporalScore = 0;

		List<GameObject> CorrectPositionIndicators = new List<GameObject> ();
		List<GameObject> ChosenPositionIndicators = new List<GameObject> ();
		List<GameObject> specialObjectListRecallOrder = new List<GameObject> ();

		List<int> objectScores = new List<int> ();

		for (int i = 0; i < specialObjectOrder.Count; i++) {

			Vector3 chosenPosition = chosenPositions [i];
			chosenPosition = new Vector3 (chosenPosition.x, 1.2f, chosenPosition.z); //to make sure explosive lands on the ground
			//throw bomb to selected location
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't actually want its visuals showing up as we wait

			#if !(MRIVERSION)
			yield return StartCoroutine (exp.objectController.ThrowExplosive (exp.player.transform.position, chosenPosition, i));
			#endif

			int randomOrderIndex = specialObjectOrder [i];

			//turn on each special object & scale up for better visibility
			GameObject specialObj = exp.objectController.RecallObjectList [randomOrderIndex];
			specialObjectListRecallOrder.Add (specialObj);

			SpawnableObject specialSpawnable = specialObj.GetComponent<SpawnableObject> ();
			int currentRecallAnswer = recallAnswers [randomOrderIndex];
			specialSpawnable.TurnVisible (true);
			specialSpawnable.Scale (2.0f);
			UsefulFunctions.FaceObject (specialObj, exp.player.gameObject, false);

			//create an indicator for each special object
			//			float indicatorHeight = exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.position.y;
			//			Vector3 correctPosition = new Vector3 (specialObj.transform.position.x, indicatorHeight, specialObj.transform.position.z);


			//create an indicator for each chosen position
			//spawn the indicator at the height of the original indicator
			//			if(!isFoil[randomOrderIndex])
			//				exp.environmentController.myPositionSelector.EnableVisibility (true); //turn on selector for spawning indicator
			float chosenIndicatorHeight = exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.position.y;
			Vector3 chosenIndicatorPosition = new Vector3 (chosenPosition.x, chosenIndicatorHeight, chosenPosition.z);

			//			GameObject chosenPositionIndicator;
			//			if(currentRecallType==2)
			//			chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.PositionSelectorVisuals, chosenIndicatorPosition, exp.environmentController.myPositionSelector.PositionSelectorVisuals.transform.rotation) as GameObject;
			//			else

			GameObject correctPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.CorrectPositionIndicator, new Vector3 (chosenIndicatorPosition.x, 4.82f, chosenIndicatorPosition.z), exp.environmentController.myPositionSelector.CorrectPositionIndicator.transform.rotation) as GameObject;
			correctPositionIndicator.GetComponent<SpawnableObject> ().SetNameID (correctPositionIndicator.transform, i);
			CorrectPositionIndicators.Add (correctPositionIndicator); 

			GameObject chosenPositionIndicator; 

			if (!isFoil [randomOrderIndex])
				chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.ObjectRecallIndicator, chosenIndicatorPosition, exp.environmentController.myPositionSelector.ObjectRecallIndicator.transform.rotation) as GameObject;
			else
				chosenPositionIndicator = Instantiate (exp.environmentController.myPositionSelector.FoilRecallIndicator, new Vector3 (chosenIndicatorPosition.x, 5.61f, chosenIndicatorPosition.z), exp.environmentController.myPositionSelector.ObjectRecallIndicator.transform.rotation) as GameObject;


			chosenPositionIndicator.GetComponent<SpawnableObject> ().SetNameID (chosenPositionIndicator.transform, i);
			chosenPositionIndicator.GetComponent<VisibilityToggler> ().TurnVisible (true);


			ChosenPositionIndicators.Add (chosenPositionIndicator);

			CorrectPositionIndicatorController correctIndicatorController = correctPositionIndicator.GetComponent<CorrectPositionIndicatorController> ();

			ChosenIndicatorController chosenIndicatorController = chosenPositionIndicator.GetComponent<ChosenIndicatorController> ();

			int points = 0;
			//increment total cues
			totalRecallCues++;
			if (currentRecallAnswer == 1) {
				Debug.Log ("changing to green");
				chosenIndicatorController.ChangeToRightColor ();
				//	correctIndicatorController.ChangeToRightColor();
				trialLogger.LogCorrectAnswer ();
				points = 200;
			} else if (currentRecallAnswer == 0) {
				Debug.Log ("changing to red");
				points = 0;
				chosenIndicatorController.ChangeToWrongColor ();
				//correctIndicatorController.ChangeToWrongColor();
				trialLogger.LogWrongAnswer ();
				//chosenPositionColor = correctIndicatorController.ChangeToWrongColor();
			}

			CorrectPositionIndicatorController correctPosController = correctPositionIndicator.GetComponent<CorrectPositionIndicatorController> ();

			correctPosController.SetPointsText (points);
			memoryScore += points;

			objectScores.Add (points);


			//WAIT BEFORE NEXT FEEDBACK
			exp.environmentController.myPositionSelector.EnableSelection (false); //turn off selector -- don't want its visuals showing up as we wait
			#if !(MRIVERSION)
			yield return new WaitForSeconds (Config_CoinTask.feedbackTimeBetweenObjects);
			#endif
		}

		//disable original selector
		exp.environmentController.myPositionSelector.EnableVisibility (false);

		#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxFeedbackTime));
		#else
		//wait for selection button press

		exp.currInstructions.SetTextPanelOn ();
		yield return StartCoroutine (exp.ShowSingleInstruction (exp.currInstructions.pressToContinue, false, true, false, Config_CoinTask.minDefaultInstructionTime));
		exp.currInstructions.SetTextPanelOff ();
		#endif

		currTrialNum++;


		//temporal feedback
		trialLogger.LogTemporalFeedbackStarted ();
		yield return StartCoroutine (exp.uiController.temporalRetrievalUI.ShowAnswer ());
		trialLogger.LogTemporalFeedbackEnded ();

		trialLogger.LogInstructionEvent ();
		trialLogger.LogScoreScreenStarted (true);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, true);
		exp.uiController.scoreRecapUI.Play (currTrialNum, timeBonus + memoryScore + temporalScore, Config_CoinTask.GetTotalNumTrials (), objectScores, specialObjectListRecallOrder, timeBonus, temporalScore, trialTimer.GetSecondsFloat ());

		#if MRIVERSION
		yield return StartCoroutine(WaitForMRITimeout(Config_CoinTask.maxScoreScreenTime));
		#else
		yield return StartCoroutine (exp.WaitForActionButton ());
		#endif

		exp.uiController.scoreRecapUI.Stop ();
		trialLogger.LogScoreScreenStarted (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.SCORESCREEN, false);


		//delete all indicators & special objects
		DestroyGameObjectList (CorrectPositionIndicators);
		DestroyGameObjectList (ChosenPositionIndicators);
		DestroyGameObjectList (exp.objectController.CurrentTrialSpecialObjects);
		DestroyGameObjectList (exp.objectController.RecallObjectList);
		DestroyGameObjectList (exp.objectController.FoilObjects);
		//reset num foil objects 
		exp.objectController.CurrentTrialFoilObjects = 0;

		trialLogger.LogFeedback (false);
		TCPServer.Instance.SetState (TCP_Config.DefineStates.FEEDBACK, false);

		yield return 0;
	}

	

	void SetConnectingLines (GameObject correctPositionIndicator, Vector3 chosenPosition, Color chosenIndicatorColor)
	{//, EnvironmentPositionSelector.SelectionRadiusType chosenRadiusSize ){
		correctPositionIndicator.GetComponent<CorrectPositionIndicatorController> ().SetLineTarget (chosenPosition, chosenIndicatorColor);
	}

	void DestroyGameObjectList (List<GameObject> listOfGameObjects)
	{
		int numObjects = listOfGameObjects.Count;
		for (int i = 0; i < numObjects; i++) {
			Destroy (listOfGameObjects [i]);
		}
		listOfGameObjects.Clear ();
	}

	public void LogAnswerSelectorPositionChanged (Config_CoinTask.MemoryState memoryState)
	{
		if (exp.uiController.doYouRememberUI.isPlaying) {
			trialLogger.LogAnswerPositionMoved (memoryState, true);
		} 
	}

	IEnumerator InitiateEyetrackerReconnection ()
	{
		yield return StartCoroutine (exp.eyeManager.StartReconnection ());
		yield return null;
	}

	public IEnumerator WaitForPlayerToLookAt (GameObject treasureChest)
	{
		//LOG waiting for player to look at the object

		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		//wait for player to look at the gameobject
		yield return StartCoroutine (Experiment_CoinTask.Instance.player.controls.PlayerLookingAt (treasureChest));

		Debug.Log ("the player has looked at");
		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;


		//LOG end of waiting for player to look at
	}

	public IEnumerator WaitForPlayerRotationToTreasure (GameObject treasureChest)
	{
		trialLogger.LogPlayerChestRotation (true);

		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		
		yield return StartCoroutine (exp.player.controls.RotateTowardSpecialObject (treasureChest));

		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;

		trialLogger.LogPlayerChestRotation (false);
	}

	//GETS CALLED FROM DEFAULTITEM.CS WHEN CHEST OPENS ON COLLISION WITH PLAYER.
	public IEnumerator WaitForTreasurePause (GameObject specialObject)
	{

		//lock the avatar controls
		exp.player.controls.ShouldLockControls = true;
		exp.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		yield return new WaitForSeconds (Config_CoinTask.pauseAtTreasureTime);
		
		//unlock the avatar controls
		Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;

		//turn the special object invisible
		if (specialObject != null) {
			specialObject.GetComponent<SpawnableObject> ().TurnVisible (false);
		}


		//only after the pause should we increment the number of coins collected...
		//...because the trial controller waits for this to move on to the next part of the trial.
		Debug.Log ("INCREMENT CHEST COUNT");
		IncrementNumDefaultObjectsCollected ();
	}
}
