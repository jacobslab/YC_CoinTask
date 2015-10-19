using UnityEngine;
using System.Collections;

public class Config_CoinTask : MonoBehaviour {

	//BOX SWAP MINIGAME VARIABLES
	public static float boxMoveTime = 1.0f;



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

	public static int numTestTrials = 32; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT
	public static Vector3 trialBlockDistribution = new Vector3 (2, 4, 2); //instead of 1,2,1 because there must be TWICE as many trials in order to counterbalance them
	
	//practice settings
	//public static int numEasyLearningTrialsPract = 1;	//TODO
	//public static int numHardLearningTrialsPract = 1;	//TODO
	public static int numTrialsPract = 1;
	public static bool doPracticeTrial = false;
	public static int numSpecialObjectsPract = 2;


//SPECIFIC COIN TASK VARIABLES:

	public static float randomJitterMin = 0.0f;
	public static float randomJitterMax = 0.2f;



	//DEFAULT OBJECTS
	/*public static int numDefaultObjectsEasy = 15;
	public static int numDefaultObjectsMedium = 15;
	public static int numDefaultObjectsHard = 15;*/
	public static int numDefaultObjects = 4;

	//THERE MUST ALWAYS BE FEWER SPECIAL OBJECTS THAN DEFAULT OBJECTS ------> now randomizing 1-3 special.
	/*public static int numSpecialObjectsEasy = 2;
	public static int numSpecialObjectsMedium = 4;
	public static int numSpecialObjectsHard = 6;*/

	public static float selectionDiameter = 20.0f;

	public static float objectToWallBuffer = 10.0f; //half of the selection diameter.
	public static float objectToObjectBuffer { get { return CalculateObjectToObjectBuffer(); } } //calculated base on min time to drive between objects!
	public static float specialObjectBufferMult = 0.0f; //the distance the object controller will try to keep between special objects. should be a multiple of objectToObjectBuffer

	public static float minDriveTimeBetweenObjects = 0.5f; //half a second driving between objects


	public static float rotateToSpecialObjectTime = 0.5f;
	public static float pauseAtTreasureTime = 1.5f;


	public static string initialInstructions1 = "Welcome to Treasure Island!" + 
		"\n\nYou are going on a treasure hunt." + 
			"\n\nUse the joystick to control your movement." + 
			"\n\nDrive into treasure chests to open them. Remember where each object is located!";

	public static string initialInstructions2 = "TIPS FOR MAXIMIZING YOUR SCORE" + 
		"\n\nGet a time bonus by driving to the chests quickly." + 
			"\n\nIf you are more than 50% sure of an object's location, you should say you remember." + 
			"\n\nIf you say you are very sure, you should be at least 75% accurate." + 
			"\n\nPress (X) to begin!";


	public static float minObjselectionUITime = 0.5f;
	public static float minInitialInstructionsTime = 0.0f; //TODO: change back to 5.0f
	public static float minDefaultInstructionTime = 0.0f; //time each learning trial instruction should be displayed for
	public static float minScoreMapTime = 0.0f;


	//tilt variables
	public static bool isAvatarTilting = true;
	public static float turnAngleMult = 0.07f;

	//drive variables
	public static float driveSpeed = 22;

	//object buffer variables

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start(){

	}

	public static int GetTotalNumTrials(){
		if (!doPracticeTrial) {
			return numTestTrials;
		} 
		else {
			return numTestTrials + numTrialsPract;
		}
	}

	public static float CalculateObjectToObjectBuffer(){
		float buffer = 0;

		if (Experiment_CoinTask.Instance != null) {

			float playerMaxSpeed = driveSpeed;
			buffer = driveSpeed * minDriveTimeBetweenObjects; //d = vt

			buffer += Experiment_CoinTask.Instance.objectController.GetMaxDefaultObjectColliderBoundXZ ();

			//Debug.Log ("BUFFER: " + buffer);

		}

		return buffer;
	}

}
