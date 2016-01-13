using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TCP_Config : MonoBehaviour {

	public static float numSecondsBeforeAlignment = 10.0f;

	public static string HostIPAddress = "192.168.137.200"; //"169.254.50.2" for Mac Pro Desktop.
	public static int ConnectionPort = 8888; //8001 for Mac Pro Desktop communication


	public static char MSG_START = '{';
	public static char MSG_END = '}';

	public static string ExpName = "TH1";
	public static string SubjectName = ExperimentSettings_CoinTask.currentSubject.name;

	public static float ClockAlignInterval = 60.0f; //this should happen about once a minute

	public enum EventType {
		SUBJECTID,
		EXPNAME,
		VERSION,
		INFO,
		CONTROL,
		DEFINE,
		SESSION,
		PRACTICE,
		TRIAL,
		PHASE,
		DISPLAYON,
		DISPLAYOFF,
		HEARTBEAT,
		ALIGNCLOCK,
		ABORT,
		SYNC,
		SYNCNP,
		SYNCED,
		STATE,
		EXIT
	}

	public enum SessionType{
		CLOSED_STIME,
		OPEN_STIM,
		NO_STIM
	}

	public static List<string> GetDefineList(){
		List<string> defineList = new List<string> ();
		//fill in how you see fit!

		defineList.Add ("TREASURE_1_APPEAR");
		defineList.Add ("TREASURE_1_OPEN");
		defineList.Add ("TREASURE_1_DISAPPEAR");
		defineList.Add ("TREASURE_2_APPEAR");
		defineList.Add ("TREASURE_2_OPEN");
		defineList.Add ("TREASURE_2_DISAPPEAR");
		defineList.Add ("TREASURE_3_APPEAR");
		defineList.Add ("TREASURE_3_OPEN");
		defineList.Add ("TREASURE_3_DISAPPEAR");
		defineList.Add ("TREASURE_4_APPEAR");
		defineList.Add ("TREASURE_4_OPEN");
		defineList.Add ("TREASURE_4_DISAPPEAR");

		defineList.Add ("NAVIGATION_STARTED");
		defineList.Add ("NAVIGATION_OVER");

		//recall cue -- "do you remember?"
		defineList.Add ("RECALLCUE_1_STARTED");
		defineList.Add ("RECALLCUE_1_OVER");
		defineList.Add ("RECALLCUE_2_STARTED");
		defineList.Add ("RECALLCUE_2_OVER");
		defineList.Add ("RECALLCUE_3_STARTED");
		defineList.Add ("RECALLCUE_3_OVER");
		defineList.Add ("RECALLCUE_4_STARTED");
		defineList.Add ("RECALLCUE_4_OVER");

		//choosing a location
		defineList.Add ("RECALLCHOOSE_1_STARTED");
		defineList.Add ("RECALLCHOOSE_1_OVER");
		defineList.Add ("RECALLCHOOSE_2_STARTED");
		defineList.Add ("RECALLCHOOSE_2_OVER");
		defineList.Add ("RECALLCHOOSE_3_STARTED");
		defineList.Add ("RECALLCHOOSE_3_OVER");
		defineList.Add ("RECALLCHOOSE_4_STARTED");
		defineList.Add ("RECALLCHOOSE_4_OVER");

		defineList.Add ("FEEDBACK_STARTED");
		defineList.Add ("FEEDBACK_OVER");

		defineList.Add ("SCORESCREEN_STARTED");
		defineList.Add ("SCORESCREEN_OVER");

		defineList.Add ("BLOCKSCREEN_STARTED");
		defineList.Add ("BLOCKSCREEN_OVER");

		return defineList;
	}
	
}
