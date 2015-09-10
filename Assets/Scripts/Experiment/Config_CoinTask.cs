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

	//DEFAULT OBJECTS
	/*public static int numDefaultObjectsEasy = 15;
	public static int numDefaultObjectsMedium = 15;
	public static int numDefaultObjectsHard = 15;*/
	public static int numDefaultObjects = 10;

	//THERE MUST ALWAYS BE FEWER SPECIAL OBJECTS THAN DEFAULT OBJECTS
	public static int numSpecialObjectsEasy = 2;
	public static int numSpecialObjectsMedium = 4;
	public static int numSpecialObjectsHard = 6;

	//RADII FOR CORRECT SELECTION DURING TESTING PHASE
	public static float smallSelectionRadius = 20.0f;
	public static float bigSelectionRadius = 40.0f;
	public static EnvironmentPositionSelector.SelectionRadiusType currentSelectionRadiusType = EnvironmentPositionSelector.SelectionRadiusType.small;

	public static float objectToWallBuffer = 20.0f; //TODO: RADIUS IS ACTUALLY DIAMETER. CHANGE THAT.
	public static float objectToObjectBuffer = 10.0f;


	public static float rotateToSpecialObjectTime = 1.0f;
	public static float pauseAtSpecialObjectTime = 1.0f;


	public static string initialInstructions = "In this game, you will pick up coins in a field. In a few locations, the coin will turn into a surprise object!" + 
		"\n" + "\nAfter you have collected all of the coins, you will be asked to select the location in the field of the surprise objects." + 
			"\n" + "\nYou will get points for how well you remember the object locations and how quickly you complete the level." +
			"\n" + "\nWe will now show you what the environment looks like. Press the button to continue."; 


	public static float minInitialInstructionsTime = 0.0f; //TODO: change back to 5.0f
	public static float minDefaultInstructionTime = 0.0f; //time each learning trial instruction should be displayed for
	public static float minScoreMapTime = 0.0f;


	//tilt variables
	public static bool isAvatarTilting = true;
	public static float maxTiltAngle = 15.0f; //max angle to tilt
	public static float maxAngleDifference = 5.0f; //maximum turning angle difference to use in tilt calculation --> currTiltAngle/maxTiltAngle = currentAngleDifference/maxAngleDifference

	//drive variables
	public static float driveSpeed = 22;

	//object buffer variables

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
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

	public void CalculateObjectToObjectBuffer(){

	}

}
