using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tobii.Research;
using Tobii.Research.CodeExamples;
using UnityEngine;

public class EyetrackerManager : MonoBehaviour {
    private IEyeTracker _eyeTracker;
    private EyetrackerLogTrack eyeLogTrack;
    private Queue<GazeDataEventArgs> _queue = new Queue<GazeDataEventArgs>();
    private bool canPumpData = false;
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
        }
        else
        {
            Debug.Log("Selected eye tracker with serial number {0}" + _eyeTracker.SerialNumber);
        }

        StartCoroutine(InitiateEyetracker());
    }
    // Use this for initialization
    void Start () {
		
	}

    IEnumerator InitiateEyetracker()
    {
        //check to see if there is any eyetracker
        if (_eyeTracker != null)
        {
            UnityEngine.Debug.Log("eyetracker is not null; performing calibration");
            //perform calibration
			yield return StartCoroutine(ScreenBasedCalibration_Calibrate.Execute(_eyeTracker));
            canPumpData = true;
        }
        yield return null;
    }

 
void Update()
{
        if(canPumpData)
            PumpGazeData();
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
    // Do something with gaze data
    eyeLogTrack.LogGazeData(new Vector3(e.LeftEye.GazeOrigin.PositionInUserCoordinates.X, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Y, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Z));
    /*
     Debug.Log(string.Format(
         "Got gaze data with {0} left eye origin at point ({1}, {2}, {3}) in the user coordinate system.",
         e.LeftEye.GazeOrigin.Validity,
         e.LeftEye.GazeOrigin.PositionInUserCoordinates.X,
        e.LeftEye.GazeOrigin.PositionInUserCoordinates.Y,
         e.LeftEye.GazeOrigin.PositionInUserCoordinates.Z));
         */
}

}
