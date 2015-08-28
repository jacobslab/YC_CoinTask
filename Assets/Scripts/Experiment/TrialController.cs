using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	bool isPracticeTrial = false;
	int numRealTrials = 0; //used for logging trial ID's
	

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
			Trial one = new Trial(false);		//stim
			Trial two = new Trial(false);		//non-stim
			practiceBlock = new Block(one, two);
		}

		for(int i = 0; i < Config_CoinTask.numBlocks/2; i++){
			Trial one = new Trial(true);			//stim
			Trial two = new Trial(false);			//non-stim
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
		exp.avatar.transform.position = trial.avatarPosition001;
		exp.avatar.transform.rotation = trial.avatarRotation001;


		GameObject newObject = exp.objectController.SpawnRandomObjectXY (trial.objectPosition);
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
		ExperimentSettings_CoinTask.currentSubject.IncrementTrial (); 
	}
}
