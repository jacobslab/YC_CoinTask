using UnityEngine;
using System.Collections;

public class CorrectPositionIndicatorController : MonoBehaviour {

	LineRenderer toTargetLine;
	float lineHeight = 0.3f;
	Vector3 startPos { get { return GetStartPos(); } }
	LineRendererLogTrack lineLogTrack;

	TextMesh pointsScoredText;

	void Awake () {
		toTargetLine = GetComponent<LineRenderer> ();
		lineLogTrack = GetComponent<LineRendererLogTrack> ();

		//start off with them both at the start pos.
		toTargetLine.SetPosition(0, startPos);
		toTargetLine.SetPosition(1, startPos);

		//log start pos as the beginning and end of the line.
		lineLogTrack.LogPoint (startPos, 0);
		lineLogTrack.LogPoint (startPos, 1);
	}

	void Start(){
		pointsScoredText = GetComponentInChildren<TextMesh> ();
		pointsScoredText.name += GetComponent<SpawnableObject> ().IDstring; //if it has an ID, it can log itself and be replayed without issue! ...maybe.
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetLineTarget(Vector3 targetPos){

		if (toTargetLine == null) {
			toTargetLine = GetComponent<LineRenderer> ();
		}

		Vector3 endPos = new Vector3 (targetPos.x, transform.position.y + lineHeight, targetPos.z); //use the current object's height
		toTargetLine.SetPosition(1, endPos);

		//log whenever the points change/are set.
		lineLogTrack.LogPoint (startPos, 0);
		lineLogTrack.LogPoint (endPos, 1);
	}

	Vector3 GetStartPos(){
		return new Vector3 (transform.position.x, transform.position.y + lineHeight, transform.position.z);
	}
}
