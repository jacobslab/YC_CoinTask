using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tobii.Research;
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
            yield return StartCoroutine(Calibrate(_eyeTracker));
            canPumpData = true;
        }
        yield return null;
    }

    private static IEnumerator Calibrate(IEyeTracker eyeTracker)
    {
        // Create a calibration object.
        var calibration = new ScreenBasedCalibration(eyeTracker);
        UnityEngine.Debug.Log("entering calibration mode");
        // Enter calibration mode.
        calibration.EnterCalibrationMode();
        // Define the points on screen we should calibrate at.
        // The coordinates are normalized, i.e. (0.0f, 0.0f) is the upper left corner and (1.0f, 1.0f) is the lower right corner.
        var pointsToCalibrate = new NormalizedPoint2D[] {
                new NormalizedPoint2D(0.5f, 0.5f),
                new NormalizedPoint2D(0.1f, 0.1f),
                new NormalizedPoint2D(0.1f, 0.9f),
                new NormalizedPoint2D(0.9f, 0.1f),
                new NormalizedPoint2D(0.9f, 0.9f),
            };
        // Collect data.
        foreach (var point in pointsToCalibrate)
        {
            // Show an image on screen where you want to calibrate.
            Debug.Log(string.Format("Show point on screen at ({0}, {1})", point.X, point.Y));
            // Wait a little for user to focus.
            yield return new WaitForSeconds(.7f);
            // Collect data.
            CalibrationStatus status = calibration.CollectData(point);
            if (status != CalibrationStatus.Success)
            {
                // Try again if it didn't go well the first time.
                // Not all eye tracker models will fail at this point, but instead fail on ComputeAndApply.
                calibration.CollectData(point);
            }
        }
        // Compute and apply the calibration.
        CalibrationResult calibrationResult = calibration.ComputeAndApply();
        Debug.Log(string.Format("Compute and apply returned {0} and collected at {1} points.",
            calibrationResult.Status, calibrationResult.CalibrationPoints.Count));
        // Analyze the data and maybe remove points that weren't good.
        calibration.DiscardData(new NormalizedPoint2D(0.1f, 0.1f));
        // Redo collection at the discarded point.
        Debug.Log(string.Format("Show point on screen at ({0}, {1})", 0.1f, 0.1f));
        calibration.CollectData(new NormalizedPoint2D(0.1f, 0.1f));
        // Compute and apply again.
        calibrationResult = calibration.ComputeAndApply();
        Debug.Log(string.Format("Second compute and apply returned {0} and collected at {1} points.",
            calibrationResult.Status, calibrationResult.CalibrationPoints.Count));
        // See that you're happy with the result.
        // The calibration is done. Leave calibration mode.
        calibration.LeaveCalibrationMode();
    }
    // <EndExample>

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
