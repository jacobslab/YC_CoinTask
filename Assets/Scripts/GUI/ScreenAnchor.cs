using UnityEngine;
using System.Collections;

public class ScreenAnchor : MonoBehaviour {
	public Camera anchorCamera;
	public Vector2 pixelOffset;

	public enum AnchorPoint{
		topLeft,
		topRight,
		center,
		bottomLeft,
		bottomRight
	}

	AnchorPoint anchor = AnchorPoint.topLeft;

	// Use this for initialization
	void Start () {
		SetScreenPosition ();
	}

	void SetScreenPosition(){
		Vector3 desiredScreenPos = Vector3.zero;

		switch (anchor) {
			case AnchorPoint.topLeft:
				desiredScreenPos = new Vector3(0.0f, anchorCamera.pixelHeight, 0.0f) + (Vector3)pixelOffset;
			break;
			case AnchorPoint.topRight:
				desiredScreenPos = new Vector3(anchorCamera.pixelWidth, anchorCamera.pixelHeight, 0.0f) + (Vector3)pixelOffset;
			break;
			case AnchorPoint.center:
				desiredScreenPos = new Vector3(anchorCamera.pixelWidth/2, anchorCamera.pixelHeight/2, 0.0f) + (Vector3)pixelOffset;
			break;
			case AnchorPoint.bottomLeft:
				desiredScreenPos = new Vector3(0.0f, 0.0f, 0.0f) + (Vector3)pixelOffset;
			break;
			case AnchorPoint.bottomRight:
				desiredScreenPos = new Vector3(anchorCamera.pixelWidth, 0.0f, 0.0f) + (Vector3)pixelOffset;
			break;
		}

		transform.position = anchorCamera.ScreenToWorldPoint(desiredScreenPos) + Vector3.forward*transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
