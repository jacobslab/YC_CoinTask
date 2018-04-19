using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using Tobii.Research;
#endif
public class EyetrackerManager : MonoBehaviour {

    public string filepath = "";
	public LayerMask layerMask;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    [DllImport ("tobii_eyetracker")]
	private static extern int connect_eyetracker();
	[DllImport ("tobii_eyetracker")]
	private static extern IntPtr return_serial_number();

	[DllImport ("tobii_eyetracker")]
	private static extern void release_all_eyetrackers();

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

	int eyeTrackerConnectResult=-1;
    string eyeTracker_sn="";
#else
    private IEyeTracker _eyeTracker;
    private Queue<GazeDataEventArgs> _queue = new Queue<GazeDataEventArgs>();

#endif

    //
    private EyetrackerLogTrack eyeLogTrack;
    public CommandExecution commExec;
	public Canvas myCanvas;
	public RawImage leftEye;
	public RawImage rightEye;
    public Vector2 leftEditPos;
    public Vector2 rightEditPos;
//    
    private bool canPumpData = false;

	public bl_ProgressBar reconnectionProgress;

	float invalidOriginTimer=0f;
    float validOriginTimer = 0f;
	private bool shouldReconnect=false;
	public CanvasGroup reconnectionGroup;

	private bool viewLeft=false;
	private bool viewRight=false;
	public static bool shouldCheckHead = false;
	
    void Awake()
    {
        filepath = Application.dataPath;
        eyeLogTrack = GetComponent<EyetrackerLogTrack>();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        eyeTrackerConnectResult=connect_eyetracker();
	//	eyeTrackerConnectResult=1;
#else

                var trackers = EyeTrackingOperations.FindAllEyeTrackers();

                foreach (IEyeTracker eyeTracker in trackers)
                {
                    UnityEngine.Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", eyeTracker.Address, eyeTracker.DeviceName, eyeTracker.Model, eyeTracker.SerialNumber, eyeTracker.FirmwareVersion));
                }
                _eyeTracker = trackers.FirstOrDefault(s => (s.DeviceCapabilities & Capabilities.HasGazeData) != 0);
#endif
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        UnityEngine.Debug.Log ("eyetracker connect result: " + eyeTrackerConnectResult.ToString ());
		if (eyeTrackerConnectResult!=1)
#else
        if(_eyeTracker==null)
#endif
        {
			UnityEngine.Debug.Log("No screen based eye tracker detected!");
			reconnectionGroup.alpha = 0f;
			//myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
        }
        else
        {

            //myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            UnityEngine.Debug.Log("Selected eyetracker with serial number {0} " + _eyeTracker.SerialNumber);
			#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			IntPtr sn = return_serial_number();
			eyeTracker_sn = Marshal.PtrToStringAnsi (sn);
			UnityEngine.Debug.Log("Selected eye tracker with serial number {0}" + eyeTracker_sn);
#endif
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
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (eyeTrackerConnectResult==1)
        {
            UnityEngine.Debug.Log("eyetracker is not null; performing calibration");
            //perform calibration
			//CommandExecution.ExecuteTobiiEyetracker(eyeTracker_sn,"usercalibration",filepath);
            canPumpData = true;
			StartCoroutine ("InitiatePumpingData");
        }
		//else
		//	CommandExecution.ExecuteTobiiEyetracker("","usercalibration",filepath);
#else
        if (_eyeTracker != null)
        {
            UnityEngine.Debug.Log("eyetracker is not null;calibration disabled temporarily");
            //perform calibration
           // CommandExecution.ExecuteTobiiEyetracker(_eyeTracker.SerialNumber, "usercalibration",filepath);
            canPumpData = true;
        }
       // else
      //      CommandExecution.ExecuteTobiiEyetracker("", "usercalibration", filepath);
            

#endif
        yield return null;
    }
	IEnumerator InitiatePumpingData()
	{
		while (canPumpData) {
			PumpGazeData ();
			yield return 0;
		}
	}
 
void Update()
{

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (canPumpData)
                    PumpGazeData();
#endif
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
	#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
void OnEnable()
{
    if (_eyeTracker != null)
    {
        UnityEngine.Debug.Log("Calling OnEnable with eyetracker: " + _eyeTracker.DeviceName);
        _eyeTracker.GazeDataReceived += EnqueueEyeData;
    }
}

void OnDisable()
{
    if (_eyeTracker != null)
    {
        _eyeTracker.GazeDataReceived -= EnqueueEyeData;
    }
}

    void OnDestroy()
{
    EyeTrackingOperations.Terminate();
}

// This method will be called on a thread belonging to the SDK, and can not safely change values
// that will be read from the main thread.
private void EnqueueEyeData(object sender, GazeDataEventArgs e)
{
    lock (_queue)
    {
        _queue.Enqueue(e);
    }
}

private GazeDataEventArgs GetNextGazeData()
{
    lock (_queue)
    {
        return _queue.Count > 0 ? _queue.Dequeue() : null;
    }
}
	#endif

private void PumpGazeData()
{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        int status = check_for_gaze_data ();
		if (status == 1)
			HandleGazeData ();
#else
        var next = GetNextGazeData();
        while (next != null)
        {
            HandleGazeData(next);
            next = GetNextGazeData();
        }
#endif
    }

    // This method will be called on the main Unity thread
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
private void HandleGazeData()
{
		UnityEngine.Debug.Log("handling gaze data");

		//GAZE ORIGIN / RECENTERING HEAD TRACKING
		int left_validity = get_left_gaze_origin_validity ();
		int right_validity = get_right_gaze_origin_validity ();
		//if either left or right origin is invalid, increase timer
		if (!shouldReconnect && (left_validity == 1 ||right_validity == 1)) {
			if (invalidOriginTimer < Config_CoinTask.minInvalidOriginTime) {
				invalidOriginTimer += Time.deltaTime;
//				UnityEngine.Debug.Log ("invalid origin timer: " + invalidOriginTimer.ToString ());
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
//					UnityEngine.Debug.Log("valid origin timer: " + validOriginTimer.ToString());
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
			Vector3 leftPos = new Vector3 (get_left_gaze_point_display_x(), (1f-get_left_gaze_point_display_y()));
//			UnityEngine.Debug.Log ("left pos : " + leftPos.ToString ());
			eyeLogTrack.LogDisplayData (leftPos, "LEFT");
			Vector2 left;
			Ray ray;
			RectTransformUtility.ScreenPointToLocalPointInRectangle (myCanvas.transform as RectTransform, new Vector2 (leftPos.x * Screen.width, -leftPos.y * Screen.height) + new Vector2 (0f, Screen.height), myCanvas.worldCamera, out left);
			leftEye.transform.position = myCanvas.transform.TransformPoint (left);
			ray = Camera.main.ViewportPointToRay (new Vector3 (leftPos.x, leftPos.y, Camera.main.nearClipPlane));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f, layerMask.value)) {
				eyeLogTrack.LogGazeObject (hit.collider.gameObject.name);
				eyeLogTrack.LogVirtualPointData (hit.point, "LEFT");
			}
		}

		if (right_validity==1 || viewRight) {
			Vector3 rightPos = new Vector3 (get_right_gaze_point_display_x(), (1f-get_right_gaze_point_display_y()));
//			UnityEngine.Debug.Log ("right pos : " + rightPos.ToString ());
			eyeLogTrack.LogDisplayData (rightPos, "RIGHT");
			Ray ray; 
			Vector2 right;
			RectTransformUtility.ScreenPointToLocalPointInRectangle (myCanvas.transform as RectTransform, new Vector2 (rightPos.x * Screen.width, -rightPos.y * Screen.height) + new Vector2 (0f, Screen.height), myCanvas.worldCamera, out right);
			rightEye.transform.position = myCanvas.transform.TransformPoint (right);
			ray = Camera.main.ViewportPointToRay (new Vector3 (rightPos.x, rightPos.y, Camera.main.nearClipPlane));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f, layerMask.value)) {
				eyeLogTrack.LogGazeObject (hit.collider.gameObject.name);
				eyeLogTrack.LogVirtualPointData (hit.point, "RIGHT");
			}
		}
		if (left_validity == 1) {
			eyeLogTrack.LogPupilData (get_left_pupil_diameter (), "LEFT");
//			UnityEngine.Debug.Log ("left pupil : " + get_left_pupil_diameter().ToString());
		}
		if (right_validity == 1) {
			eyeLogTrack.LogPupilData (get_right_pupil_diameter (), "RIGHT");

//			UnityEngine.Debug.Log ("right pupil : " + get_right_pupil_diameter ().ToString ());
		}

        
      
    }
#else
    private void HandleGazeData(GazeDataEventArgs e)
    {

        //GAZE ORIGIN / RECENTERING HEAD TRACKING

        //if either left or right origin is invalid, increase timer
        if (!shouldReconnect && (e.LeftEye.GazeOrigin.Validity == Validity.Invalid || e.RightEye.GazeOrigin.Validity == Validity.Invalid))
        {
            if (invalidOriginTimer < Config_CoinTask.minInvalidOriginTime)
            {
                invalidOriginTimer += Time.deltaTime;
                UnityEngine.Debug.Log("invalid origin timer: " + invalidOriginTimer.ToString());
            }
            else
                shouldCheckHead = true; //if the timer is above min threshold, set reconnection check for next trial

        }
        else {
            //if both origins 
            //Debug.Log ("resetting");
            //invalidOriginTimer = 0f;
            //shouldCheckHead = false;

        }

        if (shouldReconnect)
        {
            UnityEngine.Debug.Log("waiting for reconnect to be complete");
            if (validOriginTimer < Config_CoinTask.maxValidOriginTime)
            {
                reconnectionProgress.Value = validOriginTimer;
                if (e.LeftEye.GazeOrigin.Validity == Validity.Valid && e.RightEye.GazeOrigin.Validity == Validity.Valid)
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

        if (e.LeftEye.GazePoint.Validity == Validity.Valid || viewLeft)
        {
			Vector3 leftPos = new Vector3(e.LeftEye.GazePoint.PositionOnDisplayArea.X, (1f- e.LeftEye.GazePoint.PositionOnDisplayArea.Y));
            eyeLogTrack.LogDisplayData(leftPos, "LEFT");
			Ray ray;
			Vector2 left;
			RectTransformUtility.ScreenPointToLocalPointInRectangle (myCanvas.transform as RectTransform, new Vector2 (leftPos.x * Screen.width, -leftPos.y * Screen.height) + new Vector2 (0f, Screen.height), myCanvas.worldCamera, out left);
			leftEye.transform.position = myCanvas.transform.TransformPoint (left);
			ray = Camera.main.ViewportPointToRay (new Vector3 (leftPos.x, leftPos.y, Camera.main.nearClipPlane));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f, layerMask.value)) {
				eyeLogTrack.LogGazeObject (hit.collider.gameObject.name);
				eyeLogTrack.LogVirtualPointData (hit.point, "LEFT");
			}
        }

        if (e.RightEye.GazePoint.Validity == Validity.Valid || viewRight)
        {
			Vector3 rightPos = new Vector3(e.RightEye.GazePoint.PositionOnDisplayArea.X, (1f- e.RightEye.GazePoint.PositionOnDisplayArea.Y));
            eyeLogTrack.LogDisplayData(rightPos, "RIGHT");
			Ray ray; 
			Vector2 right;
			RectTransformUtility.ScreenPointToLocalPointInRectangle (myCanvas.transform as RectTransform, new Vector2 (rightPos.x * Screen.width, -rightPos.y * Screen.height) + new Vector2 (0f, Screen.height), myCanvas.worldCamera, out right);
			rightEye.transform.position = myCanvas.transform.TransformPoint (right);
			ray = Camera.main.ViewportPointToRay (new Vector3 (rightPos.x, rightPos.y, Camera.main.nearClipPlane));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f, layerMask.value)) {
				eyeLogTrack.LogGazeObject (hit.collider.gameObject.name);
				eyeLogTrack.LogVirtualPointData (hit.point, "RIGHT");
			}
        }
        if (e.LeftEye.Pupil.Validity == Validity.Valid)
            eyeLogTrack.LogPupilData(e.LeftEye.Pupil.PupilDiameter, "LEFT");
        if (e.RightEye.Pupil.Validity == Validity.Valid)
            eyeLogTrack.LogPupilData(e.RightEye.Pupil.PupilDiameter, "RIGHT");



    }
#endif

    void OnApplicationQuit()
	{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        UnityEngine.Debug.Log ("about to release all eyetrackers");
		release_all_eyetrackers ();
		UnityEngine.Debug.Log ("released all eyetrackers");
#endif
	}

}
