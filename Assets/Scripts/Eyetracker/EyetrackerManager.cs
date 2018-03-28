using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class EyetrackerManager : MonoBehaviour {


	[DllImport ("tobii_eyetracker")]
	private static extern int connect_eyetracker();
	[DllImport ("tobii_eyetracker")]
	private static extern IntPtr return_serial_number();
	[DllImport ("tobii_eyetracker")]
	private static extern int check_for_gaze_data();
	[DllImport ("tobii_eyetracker")]
	private static extern int get_left_gaze_origin_validity();
	[DllImport ("tobii_eyetracker")]
	private static extern int get_right_gaze_origin_validity();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_left_gaze_point_display_x();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_left_gaze_point_display_y();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_right_gaze_point_display_x();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_right_gaze_point_display_y();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_left_pupil_diameter();
	[DllImport ("tobii_eyetracker")]
	private static extern float get_right_pupil_diameter();


	//private IEyeTracker _eyeTracker;
    private EyetrackerLogTrack eyeLogTrack;
    public CommandExecution commExec;
	public Canvas myCanvas;
	public RawImage leftEye;
	public RawImage rightEye;
    public Vector2 leftEditPos;
    public Vector2 rightEditPos;
//    private Queue<GazeDataEventArgs> _queue = new Queue<GazeDataEventArgs>();
    private bool canPumpData = false;

	public bl_ProgressBar reconnectionProgress;

	float invalidOriginTimer=0f;
    float validOriginTimer = 0f;
	private bool shouldReconnect=false;
	public CanvasGroup reconnectionGroup;

	private bool viewLeft=false;
	private bool viewRight=false;
	int eyeTrackerConnectResult=-1;
	public static bool shouldCheckHead = false;
	string eyeTracker_sn="";
    void Awake()
    {
//        var trackers = EyeTrackingOperations.FindAllEyeTrackers();
        eyeLogTrack = GetComponent<EyetrackerLogTrack>();
//        foreach (IEyeTracker eyeTracker in trackers)
//        {
//            Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", eyeTracker.Address, eyeTracker.DeviceName, eyeTracker.Model, eyeTracker.SerialNumber, eyeTracker.FirmwareVersion));
//        }
//        _eyeTracker = trackers.FirstOrDefault(s => (s.DeviceCapabilities & Capabilities.HasGazeData) != 0);
		eyeTrackerConnectResult=connect_eyetracker();
		UnityEngine.Debug.Log ("eyetracker connect result: " + eyeTrackerConnectResult.ToString ());
		if (eyeTrackerConnectResult!=1)
        {
			UnityEngine.Debug.Log("No screen based eye tracker detected!");
			reconnectionGroup.alpha = 0f;
			//myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
        }
        else
        {

			//myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
			IntPtr sn = return_serial_number();
			eyeTracker_sn = Marshal.PtrToStringAnsi (sn);
			UnityEngine.Debug.Log("Selected eye tracker with serial number {0}" + eyeTracker_sn);
        }

        StartCoroutine(InitiateEyetracker());
    }
    // Use this for initialization
    void Start () {
		reconnectionGroup.alpha = 0f;
		Vector2 left, right;
	//	RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector3(-56f,-10f,-10f), myCanvas.worldCamera, out left);
	//	RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector3(-42f,-10f,-8f), myCanvas.worldCamera, out right);

		leftEye.transform.position = myCanvas.transform.TransformPoint(new Vector3(138f,-80f,680f));
		rightEye.transform.position = myCanvas.transform.TransformPoint(new Vector3(108f, -60f, 680f));
		UnityEngine.Debug.Log ("transforming");
		reconnectionProgress.Value = validOriginTimer;
		reconnectionProgress.MaxValue = Config_CoinTask.maxValidOriginTime;
    }

    IEnumerator InitiateEyetracker()
    {
        //check to see if there is any eyetracker
        if (eyeTrackerConnectResult==1)
        {
            UnityEngine.Debug.Log("eyetracker is not null; performing calibration");
            //perform calibration
			CommandExecution.ExecuteTobiiEyetracker(eyeTracker_sn,"usercalibration");
            canPumpData = true;
        }
        yield return null;
    }

 
void Update()
{
       

//        if (canPumpData)
//            PumpGazeData();

		if(Input.GetKeyDown(KeyCode.L))
		{
			viewLeft = true;
		}
		if (Input.GetKeyUp (KeyCode.L)) {
			viewLeft = false;
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			viewRight = true;
		}
		if (Input.GetKeyUp (KeyCode.R))
			viewRight = false;

}
  
	public IEnumerator StartReconnection()
	{
		UnityEngine.Debug.Log ("starting reconnection");
		shouldReconnect = true;
		reconnectionGroup.alpha = 1f;
		//wait until reconnection is complete
		while (shouldReconnect) {
			yield return 0;
		}
		yield return null;
	}

//void OnEnable()
//{
//    if (_eyeTracker != null)
//    {
//        Debug.Log("Calling OnEnable with eyetracker: " + _eyeTracker.DeviceName);
//        _eyeTracker.GazeDataReceived += EnqueueEyeData;
//    }
//}
//
//void OnDisable()
//{
//    if (_eyeTracker != null)
//    {
//        _eyeTracker.GazeDataReceived -= EnqueueEyeData;
//    }
//}
//
//void OnDestroy()
//{
//    EyeTrackingOperations.Terminate();
//}

// This method will be called on a thread belonging to the SDK, and can not safely change values
// that will be read from the main thread.
//private void EnqueueEyeData(object sender, GazeDataEventArgs e)
//{
//    lock (_queue)
//    {
//        _queue.Enqueue(e);
//    }
//}
//
//private GazeDataEventArgs GetNextGazeData()
//{
//    lock (_queue)
//    {
//        return _queue.Count > 0 ? _queue.Dequeue() : null;
//    }
//}

private void PumpGazeData()
{
		int status = check_for_gaze_data ();
		if (status == 1)
			HandleGazeData ();
}

// This method will be called on the main Unity thread
private void HandleGazeData()
{

		//GAZE ORIGIN / RECENTERING HEAD TRACKING

		int left_validity = get_left_gaze_origin_validity ();
		int right_validity = get_right_gaze_origin_validity ();
		//if either left or right origin is invalid, increase timer
		if (!shouldReconnect && (left_validity == 1 ||right_validity == 1)) {
			if (invalidOriginTimer < Config_CoinTask.minInvalidOriginTime) {
				invalidOriginTimer += Time.deltaTime;
				UnityEngine.Debug.Log ("invalid origin timer: " + invalidOriginTimer.ToString ());
			} else
				shouldCheckHead = true; //if the timer is above min threshold, set reconnection check for next trial

		} else {
			//if both origins 
			//Debug.Log ("resetting");
			//invalidOriginTimer = 0f;
			//shouldCheckHead = false;
				
		}
       
		if (shouldReconnect) {
			UnityEngine.Debug.Log ("waiting for reconnect to be complete");
			if (validOriginTimer < Config_CoinTask.maxValidOriginTime)
            {
				reconnectionProgress.Value = validOriginTimer;
				if (left_validity ==1 && right_validity==1)
                {
                    validOriginTimer += Time.deltaTime;
					UnityEngine.Debug.Log("valid origin timer: " + validOriginTimer.ToString());
                }
            }
            else
            {
                shouldCheckHead = false;
                invalidOriginTimer = 0f;
				reconnectionProgress.Value = 0f;
                reconnectionGroup.alpha = 0f;
                validOriginTimer = 0f;
                shouldReconnect = false;
				UnityEngine.Debug.Log("finishing reconnection");
            }
		}
		//// GAZE POINT TRACKING 

		if (left_validity==1 || viewLeft) {
			Vector3 leftPos = new Vector3 (get_left_gaze_point_display_x(), get_left_gaze_point_display_y());
			eyeLogTrack.LogDisplayData (leftPos, "LEFT");
			Vector2 left;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(leftPos.x * Screen.width, -leftPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out left);
			leftEye.transform.position = myCanvas.transform.TransformPoint(left);
			eyeLogTrack.LogGazeData (leftEye.transform.position, "LEFT");
		}

		if (right_validity==1 || viewRight) {
			Vector3 rightPos = new Vector3 (get_right_gaze_point_display_x(), get_right_gaze_point_display_y());
			eyeLogTrack.LogDisplayData (rightPos, "RIGHT");
			Vector2 right;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(rightPos.x * Screen.width, -rightPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out right);
			rightEye.transform.position = myCanvas.transform.TransformPoint (right);
			eyeLogTrack.LogGazeData (rightEye.transform.position, "RIGHT");
		}
		if(left_validity==1)
			eyeLogTrack.LogPupilData (get_left_pupil_diameter(), "LEFT");
		if(right_validity==1)
			eyeLogTrack.LogPupilData (get_right_pupil_diameter(), "RIGHT");

        
      
    }

}
