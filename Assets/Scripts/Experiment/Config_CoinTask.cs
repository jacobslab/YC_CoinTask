//#define MRI_VERSION
//#define STANDARD_VERSION

using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class Config_CoinTask : MonoBehaviour {

	public enum Version
	{
		TH1,
		TH2,
		TH3,	
		MRI
	}
#if MRIVERSION
	public static Version BuildVersion = Version.MRI;
#else
	public static Version BuildVersion = Version.TH1;
#endif

	public static string VersionNumber = "2.042";

#if MRIVERSION
	public static bool isSyncbox = false;
	public static bool isSystem2 = false;
#else
	public static bool isSyncbox = false;
	public static bool isSystem2 = false;
#endif

	//#if MRIVERSION
//	public static string HorizontalAxisName = "MRI Horizontal";
	//public static string VerticalAxisName = "MRI Vertical";
	//public static string ActionButtonName = "MRI Action Button";
	//#else
	//INPUT
	public static string HorizontalAxisName = "Horizontal";
	public static string VerticalAxisName = "Vertical";
	public static string ActionButtonName = "Action Button";
//	#endif


	//fog
	public static bool isFog=true;

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

	//SPECIFIC COIN TASK VARIABLES:
	public static float randomJitterMin = 0.0f;
	public static float randomJitterMax = 0.2f;
	
#if MRIVERSION
	public static int numTestTrials = 8; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT
	
	
	//practice settings
	public static int numTrialsPract = 3;
	public static bool isPractice = true;
	public static int[] numSpecialObjectsPract = {2,2,3};
	public static int numTwoItemTrials = 20;
	public static int numThreeItemTrials = 20;
	public static int numTrialsPerBlock = 8;
	
	//FEEDBACK SETTINGS:
	public static float feedbackTimeBetweenObjects = 0.5f;

	public static float MRIFixationTime = 30.0f;

#else
	public static int numTestTrials = 40; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT


	//practice settings
	public static int numTrialsPract = 1;
	public static bool isPractice = false;
	public static int numSpecialObjectsPract = 2;
	public static int numTwoItemTrials = 20;
	public static int numThreeItemTrials = 20;
	public static int numTrialsPerBlock = 8;


	//FEEDBACK SETTINGS:
	public static float feedbackTimeBetweenObjects = 0.5f;

#endif

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

#if MRIVERSION
	public static float rotateToSpecialObjectSpeed = 45.0f;
	public static float pauseAtTreasureTime = 1.5f;

	public static float maxChestNavigationTime = 8.0f;
	public static float maxInstructionTime = 3.0f;
	public static float maxFeedbackTime = 3.0f;
	public static float maxAnswerTime = 5.0f;
	public static float maxBoxAnswerTime = 5.0f;
	public static float maxScoreScreenTime = 5.0f;
	public static float maxLocationChooseTime = 10.0f;

	public static float MRIAutoDriveTimeMult = 2.0f;
#else
	public static float rotateToSpecialObjectSpeed = 33.33f;
	public static float pauseAtTreasureTime = 1.5f;
#endif
	
	public static float minInitialInstructionsTime = 0.0f; //TODO: change back to 5.0f
	public static float minDefaultInstructionTime = 0.0f; //time each learning trial instruction should be displayed for

	//tilt variables
	#if ENABLE_VR
	public static bool isAvatarTilting = false; //don't tilt in VR!
	#else
	public static bool isAvatarTilting = true;
	#endif
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
		if (!isPractice) {
			return numTestTrials;
		} 
		else{
		#if MRIVERSION
			return numTrialsPract;
		#else
			return numTestTrials + numTrialsPract;
		#endif
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
