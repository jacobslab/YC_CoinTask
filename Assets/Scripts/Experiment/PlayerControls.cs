using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour{

	Experiment_CoinTask exp  { get { return Experiment_CoinTask.Instance; } }


	public bool ShouldLockControls = false;

	bool isSmoothMoving = false;

	public Transform TiltableTransform;
	public Transform towerPositionTransform1;
	public Transform towerPositionTransform2;
	public Transform startPositionTransform1;
	public Transform startPositionTransform2;

	float RotationSpeed = 60.0f;
	
	float maxTimeToMove = 3.75f; //seconds to move across the furthest field distance
	float minTimeToMove = 1.5f; //seconds to move across the closest field distance
	float furthestTravelDist; //distance between far start pos and close start tower; set in start
	float closestTravelDist; //distance between close start pos and close start tower; set in start



	// Use this for initialization
	void Start () {
		//when in replay, we don't want physics collision interfering with anything
		if(ExperimentSettings_CoinTask.isReplay){
			GetComponent<Collider>().enabled = false;
		}
		else{
			GetComponent<Collider>().enabled = true;
		}

		furthestTravelDist = (startPositionTransform1.position - towerPositionTransform2.position).magnitude; //close start, far tower
		closestTravelDist = (startPositionTransform1.position - towerPositionTransform1.position).magnitude; //close start, close tower

	}
	
	// Update is called once per frame
	void Update () {

		if (exp.currentState == Experiment_CoinTask.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY; // TODO: on collision, don't allow a change in angular velocity?

				//sets velocities
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				SetTilt(0.0f, 1.0f);
			}
		}
	}

	
	//based on amount difference of y rotation, tilt in z axis
	void SetTilt(float amountTurned, float turnTime){
		if (Config_CoinTask.isAvatarTilting) {
			float turnRate = 0.0f;
			if (turnTime != 0.0f) {
				turnRate = amountTurned / turnTime;
			}

			float tiltAngle = turnRate * Config_CoinTask.turnAngleMult;

			tiltAngle *= -1; //tilt in opposite direction of the difference
			TiltableTransform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, tiltAngle);	
		}
	}

	void GetInput()
	{
		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");
		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*Config_CoinTask.driveSpeed; //since we are setting velocity based on input, no need for time.delta time component

		}
		else{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}

		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			float percent = horizontalAxisInput / 1.0f;
			Turn (percent * RotationSpeed * Time.deltaTime); //framerate independent!
		} else {
			SetTilt(0.0f, 1.0f);
		}

	}

	void Move( float amount ){
		transform.position += transform.forward * amount;
	}
	
	void Turn( float amount ){
		transform.RotateAround (transform.position, Vector3.up, amount );
		SetTilt (amount, Time.deltaTime);
	}
	

	public IEnumerator SmoothMoveTo(Vector3 targetPosition, Quaternion targetRotation){

		SetTilt (0.0f, 1.0f);

		//notify tilting that we're smoothly moving, and thus should not tilt
		isSmoothMoving = true;

		//stop collisions
		GetComponent<Collider> ().enabled = false;


		Quaternion origRotation = transform.rotation;
		Vector3 origPosition = transform.position;

		float travelDistance = (origPosition - targetPosition).magnitude;


		float timeToTravel = GetTimeToTravel (travelDistance);//travelDistance / smoothMoveSpeed;


		float tElapsed = 0.0f;
		float epsilon = 0.01f;

		//DEBUG
		float totalTimeElapsed = 0.0f;

		float angleDiffY = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y);
		float angleDiffX = Mathf.Abs(transform.rotation.eulerAngles.x - targetRotation.eulerAngles.x);
		bool arePositionsCloseEnough = UsefulFunctions.CheckVectorsCloseEnough(transform.position, targetPosition, epsilon);
		//while ( ( angleDiffY >= epsilon ) || ( angleDiffX >= epsilon ) || (!arePositionsCloseEnough) ){
		while(tElapsed < timeToTravel){
			totalTimeElapsed += Time.deltaTime;

			//tElapsed += (Time.deltaTime * moveAndRotateRate);

			tElapsed += Time.deltaTime;

			float percentageTime = tElapsed / timeToTravel;

			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, targetRotation, percentageTime); //SLERP ALWAYS TAKES THE SHORTEST PATH.
			transform.position = Vector3.Lerp(origPosition, targetPosition, percentageTime);

			//calculate new differences
			angleDiffY = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y);
			angleDiffX = Mathf.Abs(transform.rotation.eulerAngles.x - targetRotation.eulerAngles.x);
			arePositionsCloseEnough = UsefulFunctions.CheckVectorsCloseEnough(transform.position, targetPosition, epsilon);
			yield return 0;
		}
		
		Debug.Log ("TOTAL TIME ELAPSED FOR SMOOTH MOVE: " + totalTimeElapsed);

		transform.rotation = targetRotation;
		transform.position = targetPosition;

		//enable collisions again
		GetComponent<Collider> ().enabled = true;

		yield return 0;
	}

	float GetTimeToTravel(float distanceFromTarget){
		//on the very first trial, you may not have explored very far!
		//Then you get sent back to a home base, and not a tower -- which is much closer.
		if (distanceFromTarget < closestTravelDist) {

			float percentDistanceDifference = distanceFromTarget / closestTravelDist;
			float timeToTravel = percentDistanceDifference * minTimeToMove; //do a linear relationship here

			return timeToTravel;
		}
		else {
			float minMaxDistanceDifference = furthestTravelDist - closestTravelDist;
			float percentDistanceDifference = (distanceFromTarget - closestTravelDist) / minMaxDistanceDifference;
		
			float minMaxTimeDifference = maxTimeToMove - minTimeToMove;
			float timeToTravel = minTimeToMove + percentDistanceDifference * minMaxTimeDifference;
		
			return timeToTravel;
		}
	}

	public IEnumerator RotateTowardSpecialObject(GameObject target){
		Quaternion origRotation = transform.rotation;
		Vector3 targetPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		transform.LookAt(targetPosition);
		Quaternion desiredRotation = transform.rotation;
		
		
		//rotate to look at target
		transform.rotation = origRotation;
		
		float ELAPSEDTIME = 0.0f;
		
		float rotateRate = 1.0f / Config_CoinTask.rotateToSpecialObjectTime;
		float tElapsed = 0.0f;
		float rotationEpsilon = 0.01f;
		while (Mathf.Abs(transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) >= rotationEpsilon){

			float currRotY = transform.rotation.eulerAngles.y;

			tElapsed += (Time.deltaTime * rotateRate);
			ELAPSEDTIME += Time.deltaTime;

			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, desiredRotation, tElapsed); //SLERP ALWAYS TAKES THE SHORTEST PATH.

			float angleRotated = transform.rotation.eulerAngles.y - currRotY;
			SetTilt(angleRotated, Time.deltaTime);

			yield return 0;
		}
		
		
		
		transform.rotation = desiredRotation;
		
		Debug.Log ("TIME ELAPSED WHILE ROTATING: " + ELAPSEDTIME);
	}

	//returns the angle between the facing angle of the player and an XZ position
	public float GetYAngleBetweenFacingDirAndObjectXZ ( Vector2 objectPos ){

		Quaternion origRotation = transform.rotation;
		Vector3 origPosition = transform.position;

		float origYRot = origRotation.eulerAngles.y;

		//transform.RotateAround(transform.position, Vector3.up, -origYRot);
		transform.position = new Vector3( objectPos.x, origPosition.y, objectPos.y );
		transform.RotateAround(origPosition, Vector3.up, -origYRot);

		Vector3 rotatedObjPos = transform.position;


		//put player back in orig position
		transform.position = origPosition;

		transform.LookAt (rotatedObjPos);


		float yAngle = transform.rotation.eulerAngles.y;

		if(yAngle > 180.0f){
			yAngle = 360.0f - yAngle; //looking for shortest angle no matter the angle
			yAngle *= -1; //give it a signed value
		}

		transform.rotation = origRotation;

		return yAngle;

	}


	
}
