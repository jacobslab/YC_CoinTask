using System.Collections;
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
    public Text leftPosText;
    public Text rightPosText;
    public Vector2 leftEditPos;
    public Vector2 rightEditPos;
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
			myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
        }
        else
        {

			myCanvas.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
            Debug.Log("Selected eye tracker with serial number {0}" + _eyeTracker.SerialNumber);
        }

        StartCoroutine(InitiateEyetracker());
    }
    // Use this for initialization
    void Start () {
        leftPosText.text = Vector3.zero.ToString();
        rightPosText.text = Vector3.zero.ToString();
		Vector2 left, right;
	//	RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector3(-56f,-10f,-10f), myCanvas.worldCamera, out left);
	//	RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector3(-42f,-10f,-8f), myCanvas.worldCamera, out right);

		leftEye.transform.position = myCanvas.transform.TransformPoint(new Vector3(138f,-80f,680f));
		rightEye.transform.position = myCanvas.transform.TransformPoint(new Vector3(108f, -60f, 680f));
		UnityEngine.Debug.Log ("transforming");
    }

    IEnumerator InitiateEyetracker()
    {
        //check to see if there is any eyetracker
        if (_eyeTracker != null)
        {
            UnityEngine.Debug.Log("eyetracker is not null; performing calibration");
            //perform calibration
			CommandExecution.ExecuteCommand(_eyeTracker.SerialNumber,"usercalibration");
            canPumpData = true;
        }
        yield return null;
    }

 
void Update()
{
       

        if (canPumpData)
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
        // eyeLogTrack.LogGazeData(new Vector3(e.LeftEye.GazeOrigin.PositionInUserCoordinates.X, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Y, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Z));




        ///GAZE ORIGIN / RECENTERING HEAD TRACKING
        //   Vector3 leftPos = new Vector3(e.LeftEye.GazeOrigin.PositionInUserCoordinates.X, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Y, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Z);
        //   Vector3 rightPos = new Vector3(e.RightEye.GazeOrigin.PositionInUserCoordinates.X, e.RightEye.GazeOrigin.PositionInUserCoordinates.Y, e.RightEye.GazeOrigin.PositionInUserCoordinates.Z);



        //// GAZE POINT TRACKING 
        Vector3 leftPos = new Vector3(e.LeftEye.GazePoint.PositionOnDisplayArea.X, e.LeftEye.GazePoint.PositionOnDisplayArea.Y);
        Vector3 rightPos = new Vector3(e.RightEye.GazePoint.PositionOnDisplayArea.X, e.RightEye.GazePoint.PositionOnDisplayArea.Y);

        Vector2 left, right;
        leftPosText.text = leftEditPos.ToString();
        rightPosText.text = rightEditPos.ToString();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(leftPos.x * Screen.width, -leftPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out left);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, new Vector2(rightPos.x * Screen.width, -rightPos.y * Screen.height) + new Vector2(0f, Screen.height), myCanvas.worldCamera, out right);

        leftEye.transform.position = myCanvas.transform.TransformPoint(left);
        rightEye.transform.position = myCanvas.transform.TransformPoint(right);
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
