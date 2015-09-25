using UnityEngine;
using System.Collections;

//Default item, aka a treasure chest.
public class DefaultItem : MonoBehaviour {

	//opening variables
	public Transform pivotA;
	public Transform pivotB;
	public Transform top;
	public Transform specialObjectSpawnPoint;
	
	
	float angleToOpen = 150.0f; //degrees


	public ParticleSystem DefaultParticles;
	public ParticleSystem SpecialParticles;
	
	public AudioSource defaultCollisionSound;
	public AudioSource specialCollisionSound;
	bool shouldDie = false;

	public TextMesh specialObjectText;

	// Use this for initialization
	void Start () {
		if (specialObjectText != null) {
			specialObjectText.text = "";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && (defaultCollisionSound.isPlaying == false && specialCollisionSound.isPlaying == false)){
			Destroy(gameObject); //once audio has finished playing, destroy the item!
		}
	}

	void OnTriggerEnter(Collider hitCollider){
		if (hitCollider.gameObject.tag == "Player" && (tag == "DefaultItem" || tag == "DefaultSpecialItem") ) {

			//open the object
			StartCoroutine( Open(Experiment_CoinTask.Instance.player.gameObject) ); //TODO: move particle systems to chest???


			//if it was a special spot and this is the default object...
			//...we should spawn the special object!
			if (tag == "DefaultSpecialItem") {

				StartCoroutine(SpawnSpecialObject(specialObjectSpawnPoint.position));

			}
			else{
				shouldDie = true;

				Experiment_CoinTask.Instance.scoreController.AddDefaultPoints();

				GetComponent<Collider>().enabled = false;

				DefaultParticles.Play();
				defaultCollisionSound.Play();
				Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();
			}
		}
	}

	IEnumerator SpawnSpecialObject(Vector3 specialSpawnPos){
		Experiment_CoinTask.Instance.scoreController.AddSpecialPoints();
		
		//TODO: spawn with default objects, show on collision???
		
		GameObject specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObject(specialSpawnPos);
		
		//set special object text
		SetSpecialObjectText (specialObject.GetComponent<SpawnableObject> ().GetName ());
		
		//RotateTextTowardPlayer();

		SpecialParticles.Stop(); //reset the particles just in case.
		SpecialParticles.Play();
		specialCollisionSound.Play ();

		//tell the trial controller to wait for the animation
		yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForSpecialAnimation(specialObject));

		//should destroy the chest after the special object time
		Destroy(gameObject);
	}

	public void SetSpecialObjectText(string text){
		specialObjectText.text = text;
		GetComponent<TreasureChestLogTrack> ().LogTreasureLabel (specialObjectText.text);
	}

	/*void RotateTextTowardPlayer(){
		GameObject player = Experiment_CoinTask.Instance.player.gameObject;
		Vector3 lookAtPos = new Vector3 (player.transform.position.x, specialObjectText.transform.position.y, player.transform.position.z);
		specialObjectText.transform.LookAt(lookAtPos);
		specialObjectText.transform.RotateAround(transform.position, Vector3.up, 180.0f); //text faces opposite, so flip 180 degrees to actually face the player
	}*/

	//open. most likely a treasure chest. could also be something like a giftbox.
	public IEnumerator Open(GameObject opener){
		float distOpenerToPivotA = (pivotA.position - opener.transform.position).magnitude;
		float distOpenerToPivotB = (pivotB.position - opener.transform.position).magnitude;
		
		Vector3 pivotPos = transform.position;
		string closePivotName = ""; //actually want to use the closer pivot as our opener reference for Logging
		if (distOpenerToPivotA > distOpenerToPivotB) { //use the further away pivot
			pivotPos = pivotA.position;
			closePivotName = pivotB.name;
		} 
		else {
			pivotPos = pivotB.position;
			closePivotName = pivotA.name;
			angleToOpen = -angleToOpen;
		}
		
		GetComponent<TreasureChestLogTrack> ().LogOpening (closePivotName); 
		
		Quaternion origRotation = top.rotation;
		top.RotateAround(pivotPos, -transform.right, angleToOpen); //rotate to get the desired rotation
		Quaternion desiredRotation = top.transform.rotation;
		
		yield return 0;
	}
}
