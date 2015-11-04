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

	bool isOpen = false;
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
		
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Player" && (tag == "DefaultObject" || tag == "DefaultSpecialObject") && !isOpen ) {

			StartCoroutine(RunCollision());

		}
	}

	IEnumerator RunCollision(){

		yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForPlayerRotationToTreasure(gameObject));

		//open the object
		StartCoroutine( Open(Experiment_CoinTask.Instance.player.gameObject) );

		//if it was a special spot and this is the default object...
		//...we should spawn the special object!
		if (tag == "DefaultSpecialObject") {
			
			yield return StartCoroutine(SpawnSpecialObject(specialObjectSpawnPoint.position));
			
		}
		else{
			yield return StartCoroutine(RunDefaultCollision());
		}


	}

	IEnumerator RunDefaultCollision(){
		shouldDie = true;
		
		PlayJuice (false);
	
		yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForTreasurePause(null));

		while(SpecialParticles.isPlaying && DefaultParticles.isPlaying){
			yield return 0;
		}

		Destroy(gameObject); //once audio & particles have finished playing, destroy the item!
	}

	void PlayJuice(bool isSpecial){
		if (Config_CoinTask.isJuice) {
			if(isSpecial){
				SpecialParticles.Stop(); //reset the particles just in case.
				SpecialParticles.Play();
				AudioController.PlayAudio(specialCollisionSound);
			}
			else{
				DefaultParticles.Play();
				AudioController.PlayAudio(defaultCollisionSound);
			}
		}
	}

	IEnumerator SpawnSpecialObject(Vector3 specialSpawnPos){
		Experiment_CoinTask.Instance.scoreController.AddSpecialPoints();
		
		//TODO: spawn with default objects, show on collision???
		
		GameObject specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObject(specialSpawnPos);
		
		//set special object text
		SetSpecialObjectText (specialObject.GetComponent<SpawnableObject> ().GetName ());

		PlayJuice (true);

		//tell the trial controller to wait for the animation
		yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForTreasurePause(specialObject));

		//should destroy the chest after the special object time
		Destroy(gameObject);
	}

	public void SetSpecialObjectText(string text){
		specialObjectText.text = text;
		GetComponent<TreasureChestLogTrack> ().LogTreasureLabel (specialObjectText.text);
	}

	//open. most likely a treasure chest. could also be something like a giftbox.
	public IEnumerator Open(GameObject opener){
		isOpen = true;

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

		GetComponent<TreasureChestLogTrack> ().LogOpening (closePivotName, GetIsSpecial()); 
		
		Quaternion origRotation = top.rotation;


		float angleChange = 8.0f;
		float directionMult = 1.0f;

		if (angleToOpen < 0) {
			directionMult = -1.0f;
		}

		if (Config_CoinTask.isJuice) {
			//animate if juice!
			while (directionMult*angleToOpen > 0) {
				top.RotateAround (pivotPos, -directionMult * transform.right, angleChange);
				angleToOpen -= directionMult * angleChange;
				yield return 0;
			}
		} else {
			top.RotateAround (pivotPos, transform.right, -angleToOpen);
		}
		
		yield return 0;
	}

	bool GetIsSpecial(){
		if (gameObject.tag == "DefaultSpecialObject") {
			return true;
		}
		return false;
	}
}
