using UnityEngine;
using System.Collections;

public class AvatarControls : MonoBehaviour{

	Experiment_CoinTask exp  { get { return Experiment_CoinTask.Instance; } }


	public bool ShouldLockControls = false;



	//NEW STUFF NOT PHYSICS BASED -- DIDN'T WORK FOR FRAME RATE INDEPENDENCE, ALSO MESSES UP COLLISION DETECTION. DON'T USE.
	/*

	//MOVING
	float absMaxMoveSpeed = 1.0f;
	float moveAccMultiplier = 2.0f; //gets multiplied by Time.deltaTime later for a slower, framerate independent acceleration
	
	float currentMoveSpeed = 0.0f;

	//TURNING
	float absMaxRotSpeed = 1.5f;
	float rotAccMultiplier = 2.3f;

	float currentRotSpeed = 0.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; // TODO: on collision, don't allow a change in angular velocity?
				
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}
	
	void GetInput(){
		//moving
		float verticalAxisInput = Input.GetAxis ("Vertical");

		if(verticalAxisInput > 0.01f){
			MoveForward();
		}
		else if(verticalAxisInput < -0.01f){
			MoveBackward();
		}
		else{
			StopMoving();
		}

		//turning
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		
		if(horizontalAxisInput > 0.01f){
			TurnRight();
		}
		else if(horizontalAxisInput < -0.01f){
			TurnLeft();
		}
		else{
			StopTurning();
		}
	}

	//MOVING
	void MoveBackward(){
		if(currentMoveSpeed > 0){
			currentMoveSpeed = 0;
		}

		if(currentMoveSpeed > -absMaxMoveSpeed){
			currentMoveSpeed -= moveAccMultiplier*Time.deltaTime;
		}
		
		if(currentMoveSpeed < -absMaxMoveSpeed){
			currentMoveSpeed = -absMaxMoveSpeed;
		}
		
		Move ();

	}
	
	void MoveForward(){
		if(currentMoveSpeed < 0){
			currentMoveSpeed = 0;
		}

		if(currentMoveSpeed < absMaxMoveSpeed){
			currentMoveSpeed += moveAccMultiplier*Time.deltaTime;
		}
		
		if(currentMoveSpeed > absMaxMoveSpeed){
			currentMoveSpeed = absMaxMoveSpeed;
		}
		
		Move ();

	}

	void Move(){
		
		transform.position += ( Vector3.forward * currentMoveSpeed );
		
	}
	
	void StopMoving(){
		float moveIncrement = 2f * moveAccMultiplier * Time.deltaTime;
		
		if(currentMoveSpeed < 0){
			currentMoveSpeed += moveIncrement;
		}
		else if(currentMoveSpeed > 0){
			currentMoveSpeed -= moveIncrement;
		}
		
		if(currentMoveSpeed >= -moveIncrement && currentMoveSpeed < moveIncrement){
			currentMoveSpeed = 0;
		}
		
		Move();
	}



	//TURNING 

	void TurnLeft(){
		if(currentRotSpeed > 0){
			currentRotSpeed = 0;
		}
		
		if(currentRotSpeed > -absMaxRotSpeed){
			currentRotSpeed -= rotAccMultiplier*Time.deltaTime;
		}
		
		if(currentRotSpeed < -absMaxRotSpeed){
			currentRotSpeed = -absMaxMoveSpeed;
		}
		
		TurnNew ();
		
	}
	
	void TurnRight(){
		if(currentRotSpeed < 0){
			currentRotSpeed = 0;
		}
		
		if(currentRotSpeed < absMaxRotSpeed){
			currentRotSpeed += rotAccMultiplier*Time.deltaTime;
		}
		
		if(currentRotSpeed > absMaxRotSpeed){
			currentRotSpeed = absMaxRotSpeed;
		}
		
		TurnNew ();
		
	}

	void TurnNew(){
		transform.RotateAround( transform.position, Vector3.up, currentRotSpeed );
	}

	void StopTurning(){
		float rotIncrement = 2f * rotAccMultiplier * Time.deltaTime;
		
		if(currentRotSpeed < 0){
			currentRotSpeed += rotIncrement;
		}
		else if(currentRotSpeed > 0){
			currentRotSpeed -= rotIncrement;
		}
		
		if(currentRotSpeed >= -rotIncrement && currentRotSpeed < rotIncrement){
			currentRotSpeed = 0;
		}
		
		TurnNew ();
	}
*/

	public Transform TiltableTransform;

	float RotationSpeed = 0.75f;
	Quaternion lastRotation;



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

		if (rotationDifference != 0) {
			int a = 0;
		}

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
	

	public IEnumerator DriveToTargetObject(GameObject target){

		Quaternion origRotation = transform.rotation;
		Vector3 targetPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		transform.LookAt(targetPosition);
		Quaternion desiredRotation = transform.rotation;


		//rotate to look at target
		transform.rotation = origRotation;

		float ELAPSEDTIME = 0.0f;

		float rotateRate = 1.0f / Config_CoinTask.spinTime;
		float tElapsed = 0.0f;
		float rotationEpsilon = 0.01f;
		bool hasSetTilt = false;
		while (Mathf.Abs(transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) >= rotationEpsilon){

			lastRotation = transform.rotation; //set last rotation before rotating!

			tElapsed += (Time.deltaTime * rotateRate);
			ELAPSEDTIME += Time.deltaTime;
			//will spherically interpolate the rotation for config.spinTime seconds
			transform.rotation = Quaternion.Slerp(origRotation, desiredRotation, tElapsed); //SLERP ALWAYS TAKES THE SHORTEST PATH.

			if(!hasSetTilt && Config_CoinTask.isAvatarTilting){
				SetTilt(); //should be a constant speed - only set this once
				hasSetTilt = true;
			}

			yield return 0;
		}

		//set tilt back to zero
		lastRotation = transform.rotation;
		SetTilt ();


		transform.rotation = desiredRotation;

		Debug.Log ("TIME ELAPSED WHILE ROTATING: " + ELAPSEDTIME);


		//move to desired location
		Vector3 desiredPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		Vector3 origPosition = transform.position;


		ELAPSEDTIME = 0.0f;

		float moveRate = 1.0f / Config_CoinTask.driveTime;
		tElapsed = 0.0f;
		float positionEpsilon = 0.01f;
		//stop when you have collided with something
		while(!CheckXZPositionsCloseEnough(transform.position, desiredPosition, positionEpsilon)){

			tElapsed += (Time.deltaTime * moveRate);
			ELAPSEDTIME += Time.deltaTime;
			//will linearly interpolate the position for config.driveTime seconds
			transform.position = Vector3.Lerp(origPosition, desiredPosition, tElapsed);
			

			yield return 0;
		}
		transform.position = desiredPosition;

		Debug.Log ("TIME ELAPSED WHILE DRIVING: " + ELAPSEDTIME);

	}

	bool CheckXZPositionsCloseEnough(Vector3 position1, Vector3 position2, float epsilon){
		float xDiff = Mathf.Abs (position1.x - position2.x);
		float zDiff = Mathf.Abs (position1.z - position2.z);

		if (xDiff < epsilon && zDiff < epsilon) {
			return true;
		}
		else {
			return false;
		}
	}

	public void RotateTowards(Vector3 position){
		position = new Vector3(position.x, transform.position.y, position.z); //set the y coordinate to the avatar's -- should still look straight ahead!
		
		transform.LookAt(position);
	}

	//TODO: USE avatarToObjectDistance in CONFIG
	public Vector3 SetLearningLocation001(Vector3 objectPosition){
		int numPositioningAttempts = 0;

		Vector3 newPos = objectPosition;
		bool isNewPosWithinWalls = false;
		while ( ( !isNewPosWithinWalls && numPositioningAttempts < 15 ) || newPos == objectPosition) { //15 is arbitrary!
			newPos = exp.environmentController.GetRandomPositionWithinWallsXZ (Config_CoinTask.bufferBetweenObjectsAndWall);
			Vector3 dir = Vector3.Normalize(newPos - objectPosition);
			newPos = objectPosition + Config_CoinTask.avatarToObjectDistance*dir;
			numPositioningAttempts++;
			isNewPosWithinWalls = exp.environmentController.CheckWithinWalls (newPos, Config_CoinTask.bufferBetweenObjectsAndWall);

			//Debug.Log("num positioning attempts: " + numPositioningAttempts);
		}

		newPos = new Vector3 (newPos.x, transform.position.y, newPos.z);
		transform.position = newPos;

		//Vector3 newPosNoHeight = new Vector3 (newPos.x, 0, newPos.z);
		//Vector3 objectPosNoHeight = new Vector3 (objectPosition.x, 0, objectPosition.z);
		//Debug.Log ("AVATAR POSITION 001 DISTANCE FROM OBJ: " + (newPosNoHeight - objectPosNoHeight).magnitude);

		return newPos;
	}

	public Vector3 SetLearningLocation002(Vector3 objectPosition, Vector3 lastAvatarPos){
		//Vector3 avObjDiff = lastAvatarPos - objectPosition;

		int numPositioningAttempts = 0;


		//float distance = 0;
		Vector3 newPos = objectPosition;
		bool isNewPosWithinWalls = false;
		while ( !isNewPosWithinWalls && numPositioningAttempts < 15 ) { //15 is arbitrary!
			numPositioningAttempts++;

			objectPosition = new Vector3(objectPosition.x, transform.position.y, objectPosition.z);
			transform.position = objectPosition; //put avatar in object position
			RotateTowards (lastAvatarPos);

			float randomRotation = Random.Range (Config_CoinTask.minDegreeBetweenLearningTrials, Config_CoinTask.maxDegreeBetweenLearningTrials);
			int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
			
			if (shouldBeNegative == 1) {
				randomRotation *= -1;
			}

			transform.RotateAround (transform.position, Vector3.up, randomRotation); //rotate to face the line along which the avatar must be placed next!


			newPos = objectPosition + Config_CoinTask.avatarToObjectDistance*transform.forward;
			numPositioningAttempts++;
			isNewPosWithinWalls = exp.environmentController.CheckWithinWalls (newPos, Config_CoinTask.bufferBetweenObjectsAndWall);

		}

		newPos = new Vector3 (newPos.x, transform.position.y, newPos.z);
		transform.position = newPos;

		Vector3 newPosNoHeight = new Vector3 (newPos.x, 0, newPos.z);
		Vector3 objectPosNoHeight = new Vector3 (objectPosition.x, 0, objectPosition.z);
		//Debug.Log ("AVATAR POSITION 002 DISTANCE FROM OBJ: " + (newPosNoHeight - objectPosNoHeight).magnitude);

		return newPos;
	}

	//assumes the avatar is in the correct location currently
	public Quaternion SetYRotationAwayFrom(Vector3 objectPosition, float minDegree, float maxDegree){
		float randomYRotation = Random.Range (minDegree, maxDegree);
		RotateTowards (objectPosition);

		int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
		if (shouldBeNegative == 1) {
			randomYRotation *= -1;
		}

		transform.RotateAround (transform.position, Vector3.up, randomYRotation);

		//Debug.Log("Random Avatar rotation: " + randomYRotation);

		return transform.rotation;
	}

	public float GenerateRandomRotationY(){
		float randomYRotation = Random.Range (0.0f, 360.0f);
		
		return randomYRotation;
	}
	
	//only in y axis
	public Quaternion SetRandomRotationY(){
		float randomYRotation = GenerateRandomRotationY ();
		
		transform.RotateAround(transform.position, Vector3.up, randomYRotation);
		
		return transform.rotation;
	}
	
}
