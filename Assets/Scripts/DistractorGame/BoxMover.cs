using UnityEngine;
using System.Collections;

public class BoxMover : MonoBehaviour {

	float moveTime = Config_CoinTask.boxMoveTime;

	int boxLocationIndex = 0;
	public int BoxLocationIndex { get { return boxLocationIndex; } }

	public enum MoveType{
		moveOverArc,
		moveUnderArc,
		moveStraight
	}

	public MoveType myMoveType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetBoxLocationIndex (int newBoxLocationIndex){
		boxLocationIndex = newBoxLocationIndex;
		//SetMoveType ();
	}

	public void SetMoveType(MoveType newMoveType){
		myMoveType = newMoveType;
		
		Debug.Log ("SET BOX MOVE TYPE: " + myMoveType);
	}

	public void Move(Vector3 targetPos){
		switch (myMoveType){
			case MoveType.moveStraight:
				StartCoroutine(MoveStraight(targetPos));
				break;
			case MoveType.moveOverArc:
				StartCoroutine(MoveArc(targetPos, 1));
				break;
			case MoveType.moveUnderArc:
				StartCoroutine(MoveArc(targetPos, -1));
				break;
		}
	}

	IEnumerator MoveStraight(Vector3 targetPos){
		float currentTime = 0.0f;

		Vector3 origPos = transform.position;

		while(currentTime < moveTime){
			currentTime += Time.deltaTime;

			transform.position = Vector3.Lerp(origPos, targetPos, currentTime);

			yield return 0;
		}

		transform.position = targetPos;
	}

	IEnumerator MoveArc(Vector3 targetPos, int arcDirection){
		Vector3 origPos = transform.position;

		Vector3 totalDistance = targetPos - transform.position;
		
		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration*moveTime*moveTime) ) / moveTime;

		if(arcDirection > 0){
			initVelocity = new Vector3 (initVelocity.x, -initVelocity.y, initVelocity.z);
			acceleration *= -1;
		}

		float currentTime = 0.0f;
		while( currentTime < moveTime ){
			currentTime += Time.deltaTime;
			transform.position = origPos + (initVelocity * currentTime) + ( acceleration * currentTime * currentTime );

			yield return 0;
		}

		transform.position = targetPos;
	}

}
