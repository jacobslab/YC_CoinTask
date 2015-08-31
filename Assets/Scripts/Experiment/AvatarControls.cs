using UnityEngine;
using System.Collections;

public class AvatarControls : MonoBehaviour{

	Experiment_CoinTask exp  { get { return Experiment_CoinTask.Instance; } }


	public bool ShouldLockControls = false;


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
