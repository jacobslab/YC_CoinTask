using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class GazeTracker : MonoBehaviour {

    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
    public Vector2 LeftEyePosition { get; private set; }
    public Vector2 RightEyePosition { get; private set; }
    public bool LeftEyeClosed { get; private set; }
    public bool RightEyeClosed { get; private set; }
    public int VisualizationDistance = 10;
    private SpriteRenderer _gazeBubbleRenderer;
    public LayerMask layerMask;
    public bool userPresent = false;
    public static bool shouldReconnect = false;
    float disconnectionTimer = 0f;
    private EyetrackerLog eyetrackerLog;
    private UserPresence userPresence;
    public Text eyeText;
    public GameObject regularCircle;
    public GameObject normalizedCircle;

    // Use this for initialization
    void Start () {


        _gazeBubbleRenderer = GetComponent<SpriteRenderer>();
        eyetrackerLog = GetComponent<EyetrackerLog>();

        eyetrackerLog.LogResolution();
    }

	
	// Update is called once per frame
	void Update () {
        
        var gazePoint = TobiiAPI.GetGazePoint();
        userPresence = TobiiAPI.GetUserPresence();
        userPresent = userPresence.IsUserPresent();
        if(!userPresent && !shouldReconnect)
        {
            disconnectionTimer += Time.deltaTime;
            if (disconnectionTimer > 15f)
                shouldReconnect = true;
        }
        else
        {
            disconnectionTimer = 0f;
        }
        if (gazePoint.IsRecent() && Camera.main != null)
        {
            long gazeDeviceTimestamp = gazePoint.PreciseTimestamp;
            Vector2 viewportPoint = gazePoint.Viewport;
            //Debug.Log("viewport point " + viewportPoint.ToString());
           // Debug.Log("unadjusted screen point " + gazePoint.Screen.ToString());
          //  Vector2 screenPoint = new Vector2(gazePoint.Screen.normalized.x * Screen.width,gazePoint.Screen.normalized.y*Screen.height);
            Ray ray;

           ray = Camera.main.ViewportPointToRay(new Vector3(viewportPoint.x, viewportPoint.y, Camera.main.nearClipPlane));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask.value))
            {
              //  Debug.Log("collided obj " + hit.collider.gameObject.name);
                eyetrackerLog.LogGazeObject(hit.collider.gameObject.name, gazeDeviceTimestamp);
                eyetrackerLog.LogVirtualPointData(hit.point,gazeDeviceTimestamp);
            }
          //  Debug.Log("screen point " + gazePoint.Screen.ToString());
            //transform.position = ProjectToPlaneInWorld(gazePoint);
            eyetrackerLog.LogEyetrackerScreenPosition(gazePoint.Screen, gazeDeviceTimestamp);
            //Debug.Log("res " + Screen.width.ToString() + " x " + Screen.height.ToString());
            //eyeText.text = "eye " + gazePoint.Screen.ToString() + "\n eye normalized  " + gazePoint.Screen.normalized.ToString();
          //  Vector2 screenPos = new Vector2(gazePoint.Screen.x, gazePoint.Screen.y);
          //  Vector2 normalizedScreenPos = new Vector2(gazePoint.Screen.normalized.x * Screen.width, gazePoint.Screen.normalized.y * Screen.height);
           // normalizedCircle.GetComponent<RectTransform>().anchoredPosition = screenPos;
           // regularCircle.GetComponent<RectTransform>().anchoredPosition = screenPos;
            // Debug.Log("eye " + gazePoint.Screen.ToString());
            // Debug.Log("eye normalized " + gazePoint.Screen.normalized.ToString());
            //Vector2 screenPos = new Vector2(gazePoint.Screen.normalized.x * Screen.width, gazePoint.Screen.normalized.y * Screen.height);
            // _gazeBubbleRenderer.GetComponent<RectTransform>().anchoredPosition = screenPos;
        }

       // LeftEyeClosed = RightEyeClosed = TobiiAPI.GetUserPresence().IsUserPresent() && (Time.unscaledTime - gazePoint.Timestamp) > 0.15f || !gazePoint.IsRecent();
    }

    private Vector3 ProjectToPlaneInWorld(GazePoint gazePoint)
    {
        Vector3 gazeOnScreen = gazePoint.Screen;
        gazeOnScreen += (transform.forward * VisualizationDistance);
        return Camera.main.ScreenToWorldPoint(gazeOnScreen);
    }
}
