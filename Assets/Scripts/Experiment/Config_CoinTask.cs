using UnityEngine;
using System.Collections;

public class Config_CoinTask : MonoBehaviour {

//GENERIC EXPERIMENT VARIABLES

	/*
	number of sessions
	heading offsets (min, max)
	object to use vs obj to use
	one object version? (true/false)
	autodrive to destination
	wait at object time
	drive time
	drive speed
	spin time
	visible during spin
	pause before spin time
	should maximize learning angle
	minimum degree between learning trials
	do massed objects
	randomize test order
	randomize learn order
	object buffer variables
	distance between various objects (wallbuffer)
	object buffer ( distance objects in a block must be from each other)
	avatar object buffer
	stimulation variables (default to not doing stimulation)
	doStim
	stimFreq
	stimDuration
	doBreak
	Test session variables
	doTestSession (not yet implemented???)
	Practice variables
	doPracticeBlock
	*/

	//session information
	//public static int numSessions;

	//stimulation variables
	public static bool shouldDoStim;	//TODO
	public static int stimFrequency;	//TODO
	public static float stimDuration;	//TODO
	public static bool shouldDoBreak;	//TODO
	
	//test session variables
	//doTestSession (not implemented in the panda3d version )
	
	//practice variables
	public static bool doPracticeBlock = true;
	
	//the following are set in INIT depending on isOneObjectVersion
	public static int numBlocks = 2; 				//each block is two items
	public static int numEasyLearningTrials;	//per item, trials with object visible the entire time //TODO
	public static int numHardLearningTrials;	//per item, trials with object initially visible	//TODO
	public static int numTestTrials = 4;			//per item, trials with object never visible
	
	//practice settings
	public static int numEasyLearningTrialsPract = 1;	//TODO
	public static int numHardLearningTrialsPract = 1;	//TODO
	public static int numTestTrialsPract = 2;



//SPECIFIC COIN TASK VARIABLES:
	public static int numSpecialObjectsEasy = 3;
	public static int numSpecialObjectsMedium = 4;
	public static int numSpecialObjectsHard = 5;











	public static string initialInstructions = "In this game, you will learn the location of an object in a grassy field. Your job is to drive to the location of the object and press the joystick button." + 
		"\n" + "\nAt first the object is visible, but later it is hidden, so you must remember its location." + 
			"\n" + "\nWe will now show you what the environment looks like. Press the button to continue."; 

	public static float minInitialInstructionsTime = 5.0f;
	public static float learningTrialInstructionTime = 1.0f; //time each learning trial instruction should be displayed for
	public static float minTestTrialInstructionTime1 = 4.0f;
	public static float minTestTrialInstructionTime2 = 1.0f;
	public static float minScoreMapTime = 0.0f;

	public static bool isAvatarTilting = true;
	public static float maxTiltAngle = 15.0f; //max angle to tilt
	public static float maxAngleDifference = 5.0f; //maximum turning angle difference to use in tilt calculation --> currTiltAngle/maxTiltAngle = currentAngleDifference/maxAngleDifference

	public static bool isOneObjectVersion = false;	//TODO

	//drive variables
	public static bool shouldAutodrive = false;	//TODO
	public static float pauseBeforeSpinTime = 2;	//TODO
	public static bool isVisibleDuringSpin = true;	//TODO
	public static bool do360Spin;	//TODO
	public static float spinTime = 1;
	public static float driveTime = 3;
	public static float driveSpeed = 22;
	public static float waitAtObjTime = 1;

	public static float avatarToObjectDistance = 0; //The distance you will start from the object will be determine by the driveSpeed and driveTime.

	//trial and object spawning variables
	public static bool shouldMaximizeLearningAngle = true;
	public static int minDegreeBetweenLearningTrials = 20;
	public static int maxDegreeBetweenLearningTrials = 50;
	//public static bool shouldDoMassedObjects;
	//public static bool shouldRandomizeTestOrder;
	//public static bool shouldRandomizeLearnOrder;
	public static float headingOffsetMin = 30; //offset from object
	public static float headingOffsetMax = 60; //offset from object

	//object buffer variables
	public static float bufferBetweenObjects = 20; // for each block
	public static float bufferBetweenObjectsAndWall = 20;
	public static float bufferBetweenObjectsAndAvatar = 20;

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);

		float speedUnitsPerSecond = Config_CoinTask.driveSpeed; //GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*Config.driveSpeed
		avatarToObjectDistance = speedUnitsPerSecond * driveTime; // dX = v*t
		Debug.Log("AVATAR TO OBJ CONFIG DIST: " + avatarToObjectDistance);
	}

	void Start(){

	}

	public static int GetTotalNumTrials(){
		if (!doPracticeBlock) {
			return numTestTrials;
		} 
		else {
			return numTestTrials + numTestTrialsPract;
		}
	}

	public static void Init(){ //called in experiment.cs
		/*if (!isOneObjectVersion) {
			initialInstructions = "In this game, you will learn the locations of two objects in a grassy field.  For each period of driving, you will first be shown the name of an object." + 
			"\n" + "\nYou should then drive to the location of that object and press the joystick button. At first the objects are visible, but later they are hidden, so you must remember their location." +
			"\n" + "\n\nWe will now show you what the environment looks like. Press the button to continue."
			numBlocks = 8; 				//each block is two items
			numEasyLearningTrials = 1;	//per item, trials with object visible the entire time
			numHardLearningTrials = 2;	//per item, trials with object initially visible
			numTestTrials = 3;			//per itme, trials with object never visible
			
			//practice settings
			doPracticeBlock = true;
			numEasyLearningTrialsPract = 1;
			numHardLearningTrialsPract = 1;
			numTestTrialsPract = 2;
			
		}
		else{
			if(do360Spin){
				//TODO: set instructions here?
			}
			else{
				
			}

			//objectToUse = 'useAll';
			numBlocks = 24;            // each block is two items
			numEasyLearningTrials = 0; // per item, trials with object visible entire time
			numHardLearningTrials = 2; // per item, trials with object initially visible
			numTestTrials = 1;         // per item, trials with object never visible
					
			// practice settings
			doPracticeBlock = true;
			numEasyLearningTrialsPract = 0;
			numHardLearningTrialsPract = 2;
			numTestTrialsPract = 1;
		}
		else if(shouldAutodrive){
			initialInstructions = "In this game, you will learn the locations of two objects in a grassy field.  For each period of driving, you will first be shown the name of an object." + 
			"\n" + "\nYou should then drive to the location of that object and press the joystick button. At first the objects are visible, but later they are hidden, so you must remember their location." +
			"\n" + "\n\nWe will now show you what the environment looks like. Press the button to continue."
		}
		 */
	}

}
