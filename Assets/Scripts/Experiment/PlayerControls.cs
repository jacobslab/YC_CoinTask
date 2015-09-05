using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour{

	Experiment_CoinTask exp  { get { return Experiment_CoinTask.Instance; } }


	public bool ShouldLockControls = false;


	public Transform TiltableTransform;
	public Transform towerPositionTransform;

	public Vector3 TrialStartPos;
	public float TrialStartRotY;

	float RotationSpeed = 1.5f;
	Quaternion lastRotation;

	float toTowerTime = 2.0f;



	// Use this for initialization
	void Start () {
		//when in replay, we don't want physics collision interfering with anything
		if(ExperimentSettings_CoinTask.isReplay){
			GetComponent<Collider>().enabled = false;
		}
		else{
			GetComponent<Collider>().enabled = true;
		}
		lastRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		if (exp.currentState == Experiment_CoinTask.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; // TODO: on collision, don't allow a change in angular velocity?

				if (Config_CoinTask.isAvatarTilting) {
					SetTilt ();
					lastRotation = transform.rotation;
				}

				//sets velocities
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}

	void FixedUpdate(){

	}
	
	//based on amount difference of y rotation, tilt in z axis
	void SetTilt(){
		float rotationDifference = transform.rotation.eulerAngles.y - lastRotation.eulerAngles.y;

		float percentTilt = rotationDifference / Config_CoinTask.maxAngleDifference;
		float tiltAngle = percentTilt * Config_CoinTask.maxTiltAngle;
		if (percentTilt > 1.0f) {
			tiltAngle = Config_CoinTask.maxTiltAngle;
		}

		tiltAngle *= -1; //tilt in opposite direction of the difference
		TiltableTransform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, tiltAngle);
	}

	void GetInput()
	{
		//VERTICAL
		float verticalAxisInput = Input.GetAxis ("Vertical");
		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*Config_CoinTask.driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input																								//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.

		}
		else{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}

		//HORIZONTAL
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			//GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
			float percent = horizontalAxisInput / 1.0f;
			Turn (percent * RotationSpeed);
			//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
		}
		else {
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
		}

	}

	void Move( float amount ){
		transform.position += transform.forward * amount;
	}
	
	void Turn( float amount ){
		transform.RotateAround (transform.position, Vector3.up, amount );
	}

	public void SmoothMoveToTower(){

		StartCoroutine( SmoothMoveTo (towerPositionTransform.position, towerPositionTransform.rotation, toTowerTime ));
		
	}

	public void SmoothMoveToPos(Vector3 position, Quaternion rotation){
		int a = 0;

		StartCoroutine( SmoothMoveTo (position, rotation, toTowerTime) );

	}

	IEnumerator SmoothMoveTo(Vector3 targetPosition, Quaternion targetRotation,  float totalTime){

		Debug.Log("Started Smooth Moving");

		Quaternion origRotation = transform.rotation;
		Vector3 origPosition = transform.position;
		
		float moveAndRotateRate = 1.0f / totalTime;
		float tElapsed = 0.0f;
		float epsilon = 0.01f;

		float angleDiff = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y);
		bool arePositionsCloseEnough = CheckPositionsCloseEnough(transform.position, targetPosition, epsilon);
		while ( ( angleDiff >= epsilon ) || (!arePositionsCloseEnough) ){
			Debug.Log("AHHHHH");
			lastRotation = transform.rotation; //set last rotation before rotating!
			
			tElapsed += (Time.deltaTime * moveAndRotateRate);
			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, targetRotation, tElapsed); //SLERP ALWAYS TAKES THE SHORTEST PATH.
			transform.position = Vector3.Lerp(origPosition, targetPosition, tElapsed);

			//calculate new differences
			angleDiff = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y);
			arePositionsCloseEnough = CheckPositionsCloseEnough(transform.position, targetPosition, epsilon);
			yield return 0;
		}
		
		
		transform.rotation = targetRotation;
		transform.position = targetPosition;
		
		yield return 0;
	}
	
	bool CheckPositionsCloseEnough(Vector3 position1, Vector3 position2, float epsilon){
		float xDiff = Mathf.Abs (position1.x - position2.x);
		float yDiff = Mathf.Abs (position1.y - position2.y);
		float zDiff = Mathf.Abs (position1.z - position2.z);
		
		if (xDiff < epsilon && yDiff < epsilon && zDiff < epsilon) {
			return true;
		}
		else {
			return false;
		}
	}

	public IEnumerator RotateTowardSpecialObject(GameObject target){
		//set tilt to zero
		lastRotation = transform.rotation;
		SetTilt ();

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
		//bool hasSetTilt = false;
		while (Mathf.Abs(transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) >= rotationEpsilon){
			
			lastRotation = transform.rotation; //set last rotation before rotating!
			
			tElapsed += (Time.deltaTime * rotateRate);
			ELAPSEDTIME += Time.deltaTime;
			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, desiredRotation, tElapsed); //SLERP ALWAYS TAKES THE SHORTEST PATH.
			
			//if(!hasSetTilt && Config_CoinTask.isAvatarTilting){
			//	SetTilt(); //should be a constant speed - only set this once
			//	hasSetTilt = true;
			//}
			
			yield return 0;
		}
		
		
		
		transform.rotation = desiredRotation;
		
		Debug.Log ("TIME ELAPSED WHILE ROTATING: " + ELAPSEDTIME);
	}


	
}
