using UnityEngine;
using System.Collections;

public class BoxMover : MonoBehaviour {

	float moveTime = 1.0f;

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

	public void Move(Vector3 targetPos){
		switch (myMoveType){
			case MoveType.moveStraight:
				StartCoroutine(MoveStraight(targetPos));
				break;
			case MoveType.moveOverArc:
				StartCoroutine(MoveArc(targetPos));
				break;
			case MoveType.moveUnderArc:
			//TODO: GET RID OF MOVE UNDER ARC? OR IMPLEMENT IT?
				StartCoroutine(MoveArc(targetPos));
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

	IEnumerator MoveArc(Vector3 targetPos){
		Vector3 origPos = transform.position;

		Vector3 totalDistance = targetPos - transform.position;
		
		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration*moveTime*moveTime) ) / moveTime;

		float currentTime = 0.0f;
		while( currentTime < moveTime ){
			currentTime += Time.deltaTime;
			transform.position = origPos + (initVelocity * currentTime) + ( acceleration * currentTime * currentTime );

			yield return 0;
		}

		transform.position = targetPos;
	}
}
