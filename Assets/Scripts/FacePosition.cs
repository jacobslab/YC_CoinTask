using UnityEngine;
using System.Collections;

public class FacePosition : MonoBehaviour {
	
	public Transform TargetPositionTransform;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		FaceThePosition ();
	}

	void FaceThePosition(){
		Quaternion origRot = transform.rotation;
		transform.LookAt (TargetPositionTransform);
		float yRot = transform.rotation.eulerAngles.y;

		transform.rotation = Quaternion.Euler (origRot.eulerAngles.x, yRot, origRot.eulerAngles.z);
	}
}
