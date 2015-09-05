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

		exp.player.TurnOnVisuals (true);

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
		exp.player.TurnOnVisuals (false);
		exp.player.controls.SmoothMoveToTower ();

		//show instructions for location selection 
		yield return StartCoroutine (exp.ShowSingleInstruction ("You will now be shown the environment as a grid. Use the joystick to select the location of the requested object.", true, true, Config_CoinTask.minDefaultInstructionTime));

		//ask player to locate each object individually on the grid
		int memoryScore = 0;
		for (int i = 0; i < exp.objectController.CurrentTrialSpecialObjects.Count; i++) {
			//turn on grid visibility
			exp.environmentController.myGrid.TurnOnTileVisibility(true);
			//enable grid selection
			exp.player.tileSelector.Enable ();

			//show instructions for location selection -- TODO: use the image of the object instead?
			SpawnableObject specialObj = exp.objectController.CurrentTrialSpecialObjects [i];
			string specialItemName = specialObj.GetName();
			//show instructions and wait for selection button press
			yield return StartCoroutine (exp.ShowSingleInstruction ("Select the location of the " + specialItemName + ".", false, true, Config_CoinTask.minDefaultInstructionTime));

			Tile correctTile = exp.environmentController.myGrid.GetGridTile(specialObj.GetComponent<GridItem>().rowIndex, specialObj.GetComponent<GridItem>().colIndex);
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
			Vector2 chosenTileGridPos = exp.player.tileSelector.selectedTile.GetComponent<GridItem>().GetGridIndices();
			Vector2 correctTileGridPos = correctTile.GetComponent<GridItem>().GetGridIndices();
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






/*		GameObject newObject = exp.objectController.SpawnRandomObjectXY (trial.objectPosition);
		SpawnableObject newSpawnedObject = newObject.GetComponent<SpawnableObject> ();
		string newObjectName = newObject.GetComponent<SpawnableObject>().GetName();

		//override player input -- going to be driven to the object
		exp.avatar.ShouldLockControls = true;

		//show instruction for "press the button to be driven to the OBJECT_NAME".
		yield return StartCoroutine(exp.ShowSingleInstruction("Press the button to be driven to the " + newObjectName + ".", true, true, Config_CoinTask.learningTrialInstructionTime));
		
		//drive the player to the object
		newSpawnedObject.TurnVisible (false); //important function to turn off the object without setting it inactive -- because we want to keep logging on
		yield return new WaitForSeconds(Config_CoinTask.pauseBeforeSpinTime);
		newSpawnedObject.TurnVisible (true);
		yield return exp.avatar.StartCoroutine(exp.avatar.DriveToTargetObject(newObject));
		yield return new WaitForSeconds (Config_CoinTask.waitAtObjTime); //wait at object
		
		//show instruction for "you will now be driven to the OBJECT_NAME from another location.
		yield return StartCoroutine(exp.ShowSingleInstruction("You will now be driven to the " + newObjectName + 
		                                                      "\n from another location.", true, false, Config_CoinTask.learningTrialInstructionTime));
		
		//override player input (already done above)
		//move player to second location
		//drive player to object
		exp.avatar.transform.position = trial.avatarPosition002;
		exp.avatar.transform.rotation = trial.avatarRotation002;

		newSpawnedObject.TurnVisible (false);
		yield return new WaitForSeconds(Config_CoinTask.pauseBeforeSpinTime);
		newSpawnedObject.TurnVisible (true);
		yield return exp.avatar.StartCoroutine(exp.avatar.DriveToTargetObject(newObject));
		yield return new WaitForSeconds (Config_CoinTask.waitAtObjTime); //wait at object


		//HIDE OBJECT

		//turn off visuals of object
		newSpawnedObject.TurnVisible (false);

		//show instruction for "the OBJECT_NAME is now hidden. you will now drive to the OBJECT_NAME on your own."
		//+"Press the button to continue, and then drive to the locaiton of the cactus and press the button when you are in the correct location."
		yield return StartCoroutine(exp.ShowSingleInstruction("The " + newObjectName + " is now hidden. " +
														"\nYou will now drive to the " + newObjectName + " on your own.", true, false, Config_CoinTask.minTestTrialInstructionTime1));
		//after some time, add more to the instruction...
		yield return StartCoroutine(exp.ShowSingleInstruction("The " + newObjectName + " is now hidden. " +
		                                                      "\nYou will now drive to the " + newObjectName + " on your own." +
		                                                      "\n" + "\nPress the button to continue, and then drive to the location of the "+ newObjectName +
		                                                      "and press the button when you are in the correct location.", true, true, Config_CoinTask.minTestTrialInstructionTime2));
		
		//show black text across top of screen: "press the button at the location of the OBJECT_NAME"
		//exp.inGameInstructionsController.DisplayText("press the button at the location of the " + newObjectName);
		exp.instructionsController.DisplayText("press the button at the location of the " + newObjectName);
		
		//move player to random location (location 3) & rotation
		exp.avatar.transform.position = trial.avatarPosition003;
		exp.avatar.transform.rotation = trial.avatarRotation003;

		//enable player movement
		//wait for player to press the button, then move on
		exp.avatar.ShouldLockControls = false;
		yield return StartCoroutine(exp.WaitForActionButton());


		//show overhead view of player's position vs. desired object position
		//with text: "Nice job. You earned X points. Press the button to continue."
		exp.environmentMap.SetAvatarVisualPosition(exp.avatar.transform.position);
		exp.environmentMap.SetObjectVisualPosition(newObject.transform.position);
		exp.environmentMap.TurnOn();

		//calculate points
		int pointsReceived = exp.scoreController.CalculatePoints(newObject);

		//add points
		ExperimentSettings_CoinTask.currentSubject.AddScore(pointsReceived);

		//show point text
		yield return StartCoroutine(exp.ShowSingleInstruction("Nice job! You earned " + pointsReceived.ToString() + " points. Press the button to continue. " + 
		                                                      "\n" + "\n Overall Score: " + exp.scoreController.score,false, true, Config_CoinTask.minScoreMapTime));

		//turn off the environment map
		exp.environmentMap.TurnOff();
		
		GameObject.Destroy(newObject); 

		 //increment subject's trial count
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial ();*/
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
