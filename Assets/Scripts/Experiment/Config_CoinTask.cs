//#define MRI_VERSION
//#define STANDARD_VERSION

using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class Config_CoinTask : MonoBehaviour {

	public enum Version
	{
		THR1,
		TH2,
		THR3,
		THR_RECORD,
		THR_STIM,
		MRI
	}
#if MRIVERSION
	public static Version BuildVersion = Version.MRI;
#else
	public static Version BuildVersion = Version.THR3;
#endif

	public static string VersionNumber = "2.100";


#if MRIVERSION
	public static bool isSyncbox = false;
	public static bool isSYS3 = false;
#endif

#if SYS1
	public static bool isSyncbox = true;
	public static bool isSYS3 = false;
#endif

#if SYS3
	public static bool isSyncbox= false;
	public static bool isSYS3 = true;
#endif

#if SYS3COMBO
	public static bool isSyncbox = true;
	public static bool isSYS3 = true;
#endif
	#if DEMO
	public static bool isSyncbox = false;
	public static bool isSYS3 = false;
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

	public static float randomPALJitterMin = 0.5f;
	public static float randomPALJitterMax = 0.75f;

	public static float isiTime = 1.0f;

	public static int selectedMic=0;
	public static float micLoudThreshold = 0.1f;

	//recall
	#if MRIVERSION
	public static int recallTime=2;
	#else
	public static int recallTime=6;
	#endif

	//trophies
	public static int bronzeThreshold=3; //must give 3 consecutive correct within a single trial
	public static int silverThreshold=3; // must have 80% or more recall rate for 3 trials
	public static int goldThreshold=3; // must have 100% recall rate for 3 trials
	public static float trophyDisplayTime=2f; //time trophy is initially displayed on-screen when won
	public static float trophyTransformTime=2f; //time trophy takes to transform from center to top-right corner
	public static float trophyRedeemTime=2f; //time trophy takes to be redeemed by moving from top-right to block score transform
	
#if MRIVERSION
	public static int numTestTrials = 8; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT
	
	
	//practice settings
	public static int numTrialsPract = 3;
	public static bool isPractice = false;
	public static int[] numSpecialObjectsPract = {2,2,3};
//	public static int numTwoItemTrials = 20;
//	public static int numThreeItemTrials = 20;
	public static int numThreeItemTrials = 4;
	public static int numFourItemTrials = 4;
	public static int numTrialsPerBlock = 6;
	
	//FEEDBACK SETTINGS:
	public static float feedbackTimeBetweenObjects = 0.5f;

	public static float MRIFixationTime = 30.0f;

#else
	public static int numTestTrials = 30; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT


	//practice settings
	public static int numTrialsPract = 1;
	public static bool isPractice = false;
	public static int numSpecialObjectsPract = 2;
	//public static int numTwoItemTrials = 6;

	public static int numThreeItemTrials = 15;
	public static int numFourItemTrials = 15;
//
//	public static int numFiveItemTrials = 6;
//	public static int numSixItemTrials = 6;
	public static int numTrialsPerBlock = 6;


	//FEEDBACK SETTINGS:
	public static float feedbackTimeBetweenObjects = 0.5f;

#endif

	public enum MemoryState{
		yes,
		maybe,
		no
	}


	//OBJECTS
	public static int numDefaultObjects = 6;

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

	public void SetNewMic()
	{
		selectedMic = Experiment_CoinTask.Instance.trialController.micTest.micDrops.value;
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
