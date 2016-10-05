using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using iView;

public class GazeFollower2D : MonoBehaviour {

	public Canvas myCanvas;
    private Ray ray;
    private RaycastHit hit;
	Transform gazeFollower;
    public LayerMask mask;
    public EyetrackerLogTrack eyetrackerLogTrack;
	void Awake(){
		gazeFollower = GetComponent<Transform> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gazeFollower != null) {
			Vector2 screenGazePos = SMIGazeController.Instance.GetSample ().averagedEye.gazePosInUnityScreenCoords ();
			//Debug.Log("SCREEN POS: " + screenGazePos);
            eyetrackerLogTrack.LogScreenGazePoint(screenGazePos);
            double leftPupilDiameter = SMIGazeController.Instance.GetSample().leftEye.pupilDiameter;
            double rightPupilDiameter= SMIGazeController.Instance.GetSample().rightEye.pupilDiameter;
            double averagedPupilDiameter = SMIGazeController.Instance.GetSample().averagedEye.pupilDiameter;
            eyetrackerLogTrack.LogPupilDiameter(leftPupilDiameter,rightPupilDiameter,averagedPupilDiameter);
            Vector3 worldGazePos = Camera.main.ScreenToWorldPoint(new Vector3 ( screenGazePos.x, screenGazePos.y, gazeFollower.position.z));
            eyetrackerLogTrack.LogWorldGazePoint(worldGazePos);
			//Debug.Log("WORLD POS: " + worldGazePos);
            ray = Camera.main.ScreenPointToRay(screenGazePos);

            Debug.DrawRay(new Vector3(screenGazePos.x, screenGazePos.y, 0f), worldGazePos, Color.red);
            if(Physics.SphereCast(ray,0.8f,out hit,100f,mask.value))
            {
                Debug.Log(hit.collider.gameObject.name);
                eyetrackerLogTrack.LogGazeObject(hit.collider.gameObject);
               // hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.green;
            }
			//gazeFollower.position = new Vector3(screenGazePos.x, screenGazePos.y, gazeFollower.position.z);
			//gazeFollower.position = worldGazePos;


			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, screenGazePos, myCanvas.worldCamera, out pos);
			gazeFollower.position = myCanvas.transform.TransformPoint(pos);

		}
	}
}
