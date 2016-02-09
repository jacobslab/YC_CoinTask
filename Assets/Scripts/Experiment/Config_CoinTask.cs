using UnityEngine;
using System.Collections;

public class Config_CoinTask : MonoBehaviour {

	public enum Version
	{
		TH1,
		TH2,
		TH3,
		Pilot_001,	
		Pilot_002,		
		FMRI_001	
	}

	public static Version BuildVersion = Version.TH3;
	public static string VersionNumber = "2.03";

	public static bool isSyncbox = false;
	public static bool isSystem2 = false;

	//REPLAY
	public static int replayPadding = 6;

	//JUICE
	public static bool isJuice = true;
	public static bool isSoundtrack = false; //WON'T PLAY IF ISJUICE IS FALSE.


	//BOX SWAP MINIGAME VARIABLES
	public static float boxMoveTime = 0.5f;
	public static Vector3 boxAcceleration = Physics.gravity * 3.0f;

	//stimulation variables
	/*public static bool shouldDoStim;	//TODO
	public static int stimFrequency;	//TODO
	public static float stimDuration;	//TODO
	public static bool shouldDoBreak;	//TODO*/
	
	//test session variables
	//doTestSession (not implemented in the panda3d version )

	public static int numTestTrials = 40; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT
	
	//practice settings
	public static int numTrialsPract = 1;
	public static bool doPracticeTrial = false;
	public static int numSpecialObjectsPract = 2;
	public static int numTwoItemTrials = 20;
	public static int numThreeItemTrials = 20;
	public static int numTrialsPerBlock = 8;


	//SPECIFIC COIN TASK VARIABLES:
	public static float randomJitterMin = 0.0f;
	public static float randomJitterMax = 0.2f;

	//FEEDBACK SETTINGS:
	public static float feedbackTimeBetweenObjects = 0.5f;

	public enum MemoryState{
		yes,
		maybe,
		no
	}


	//OBJECTS
	public static int numDefaultObjects = 4;

	public static float selectionDiameter = 26.0f;

	public static float objectToWallBuffer = 13.0f; //half of the selection diameter.
	public static float objectToObjectBuffer { get { return CalculateObjectToObjectBuffer(); } } //calculated base on min time to drive between objects!
	public static float specialObjectBufferMult = 0.0f; //the distance the object controller will try to keep between special objects. should be a multiple of objectToObjectBuffer

	public static float minDriveTimeBetweenObjects = 0.5f; //half a second driving between objects


	public static float rotateToSpecialObjectTime = 0.5f;
	public static float pauseAtTreasureTime = 1.5f;


	public static string initialInstructions1 = "Welcome to Treasure Island!" + 
		"\n\nYou are going on a treasure hunt." + 
			"\n\nUse the joystick to control your movement." + 
			"\n\nDrive into treasure chests to open them. Remember where each object is located!";

	public static string initialInstructions2 = "When you are asked about remembering an object's location, you must answer [ YES ], [ MAYBE ], or [ NO ], and choose a location." +
			"\n\nYou will win points for correct locations and you will lose points for incorrect locations.";

	public static string initialInstructions3 = "TIPS FOR MAXIMIZING YOUR SCORE" + 
		"\n\nGet a time bonus by driving to the chests quickly." +
		"\n\nIf you are more than 75% sure, you should select [YES]." +
		"\n\nIf you are at least 50% sure, you should select [MAYBE]." +
		"\n\nOtherwise, should should select [NO]." +
		"\n\nPress (X) to begin!";
		/*	"\n\nIf you are more than 50% sure of an object's location, you should say you remember." + 
			"\n\nIf you say you are very sure, you should be at least 75% accurate." + 
			"\n\nPress (X) to begin!";*/
	
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

			//float playerMaxSpeed = driveSpeed;
			buffer = driveSpeed * minDriveTimeBetweenObjects; //d = vt

			buffer += Experiment_CoinTask.Instance.objectController.GetMaxDefaultObjectColliderBoundXZ ();

			//Debug.Log ("BUFFER: " + buffer);
			if(buffer < Config_CoinTask.selectionDiameter){
				buffer = Config_CoinTask.selectionDiameter;
			}
		}

		return buffer;
	}

}
