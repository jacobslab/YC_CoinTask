﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tobii.Research;
using Tobii.Research.CodeExamples;
using UnityEngine;
using UnityEngine.UI;
public class EyetrackerManager : MonoBehaviour {
    private IEyeTracker _eyeTracker;
    private EyetrackerLogTrack eyeLogTrack;
    public CommandExecution commExec;
	public Canvas myCanvas;
	public RawImage leftEye;
	public RawImage rightEye;
    public Vector2 leftEditPos;
    public Vector2 rightEditPos;
    private Queue<GazeDataEventArgs> _queue = new Queue<GazeDataEventArgs>();
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
        var trackers = EyeTrackingOperations.FindAllEyeTrackers();
        eyeLogTrack = GetComponent<EyetrackerLogTrack>();
        foreach (IEyeTracker eyeTracker in trackers)
        {
            Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", eyeTracker.Address, eyeTracker.DeviceName, eyeTracker.Model, eyeTracker.SerialNumber, eyeTracker.FirmwareVersion));
        }
        _eyeTracker = trackers.FirstOrDefault(s => (s.DeviceCapabilities & Capabilities.HasGazeData) != 0);
        if (_eyeTracker == null)
        {
            Debug.Log("No screen based eye tracker detected!");
			reconnectionGroup.alpha = 0f;
			//myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
        }
        else
        {

			//myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
            Debug.Log("Selected eye tracker with serial number {0}" + _eyeTracker.SerialNumber);
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
        if (_eyeTracker != null)
        {
            UnityEngine.Debug.Log("eyetracker is not null; performing calibration");
            //perform calibration
			CommandExecution.ExecuteTobiiEyetracker(_eyeTracker.SerialNumber,"usercalibration");
            canPumpData = true;
        }
        yield return null;
    }

 
void Update()
{
       

        if (canPumpData)
            PumpGazeData();

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
		Debug.Log ("starting reconnection");
		shouldReconnect = true;
		reconnectionGroup.alpha = 1f;
		//wait until reconnection is complete
		while (shouldReconnect) {
			yield return 0;
		}
		yield return null;
	}

void OnEnable()
{
    if (_eyeTracker != null)
    {
        Debug.Log("Calling OnEnable with eyetracker: " + _eyeTracker.DeviceName);
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

private void PumpGazeData()
{
    var next = GetNextGazeData();
    while (next != null)
    {
        HandleGazeData(next);
        next = GetNextGazeData();
    }
}

// This method will be called on the main Unity thread
private void HandleGazeData(GazeDataEventArgs e)
{

		//GAZE ORIGIN / RECENTERING HEAD TRACKING

		//if either left or right origin is invalid, increase timer
		if (!shouldReconnect && (e.LeftEye.GazeOrigin.Validity == Validity.Invalid || e.RightEye.GazeOrigin.Validity == Validity.Invalid)) {
			if (invalidOriginTimer < Config_CoinTask.minInvalidOriginTime) {
				invalidOriginTimer += Time.deltaTime;
				Debug.Log ("invalid origin timer: " + invalidOriginTimer.ToString ());
			} else
				shouldCheckHead = true; //if the timer is above min threshold, set reconnection check for next trial

		} else {
			//if both origins 
			//Debug.Log ("resetting");
			//invalidOriginTimer = 0f;
			//shouldCheckHead = false;
				
		}
       
		if (shouldReconnect) {
			Debug.Log ("waiting for reconnect to be complete");
			if (validOriginTimer < Config_CoinTask.maxValidOriginTime)
            {
				reconnectionProgress.Value = validOriginTimer;
                if (e.LeftEye.GazeOrigin.Validity == Validity.Valid && e.RightEye.GazeOrigin.Validity == Validity.Valid)
                {
                    validOriginTimer += Time.deltaTime;
                    Debug.Log("valid origin timer: " + validOriginTimer.ToString());
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
                Debug.Log("finishing reconnection");
            }
		}
		//// GAZE POINT TRACKING 

		if (e.LeftEye.GazePoint.Validity == Validity.Valid || viewLeft) {
			Vector2 leftPos = new Vector2 (e.LeftEye.GazePoint.PositionOnDisplayArea.X, e.LeftEye.GazePoint.PositionOnDisplayArea.Y);
			eyeLogTrack.LogDisplayData (leftPos, "LEFT");
			Vector2 left;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(leftPos.x * Screen.width, -leftPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out left);
			leftEye.transform.position = myCanvas.transform.TransformPoint(left);
			eyeLogTrack.LogGazeData (leftEye.transform.position, "LEFT");
		}

		if (e.RightEye.GazePoint.Validity == Validity.Valid || viewRight) {
			Vector2 rightPos = new Vector2 (e.RightEye.GazePoint.PositionOnDisplayArea.X, e.RightEye.GazePoint.PositionOnDisplayArea.Y);
			eyeLogTrack.LogDisplayData (rightPos, "RIGHT");
			Vector2 right;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(rightPos.x * Screen.width, -rightPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out right);
			rightEye.transform.position = myCanvas.transform.TransformPoint (right);
			eyeLogTrack.LogGazeData (rightEye.transform.position, "RIGHT");
		}
		if(e.LeftEye.Pupil.Validity == Validity.Valid)
			eyeLogTrack.LogPupilData (e.LeftEye.Pupil.PupilDiameter, "LEFT");
		if(e.RightEye.Pupil.Validity == Validity.Valid)
			eyeLogTrack.LogPupilData (e.RightEye.Pupil.PupilDiameter, "RIGHT");

        
      
    }

}
