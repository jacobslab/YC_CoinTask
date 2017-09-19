using UnityEngine;
using System.Collections;

//Default item, aka a treasure chest.
public class DefaultItem : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

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

	private GameObject specialObject;
	bool isExecutingPlayerCollision = false;
	//bool shouldDie = false;

	public TextMesh specialObjectText;

	void Awake(){
		InitTreasureState ();
	}

	void InitTreasureState(){
		switch (exp.trialController.NumDefaultObjectsCollected) {
		case 0:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, true,1);
			break;
		case 1:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, true,2);
			break;
		case 2:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, true,3);
			break;
		case 3:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, true,4);
			break;
		}
	}

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
		if (collision.gameObject.tag == "Player" && (tag == "DefaultObject" || tag == "DefaultSpecialObject") && !isExecutingPlayerCollision) {
			if (!ExperimentSettings_CoinTask.isOculus) {
				GetComponent<TreasureChestLogTrack> ().LogPlayerChestCollision ();

				isExecutingPlayerCollision = true;
				StartCoroutine (RunCollision ());

			} else {
				StartCoroutine ("WaitForObjectLookAt");
			}
		}
	}

	IEnumerator WaitForObjectLookAt()
	{
		yield return StartCoroutine (Experiment_CoinTask.Instance.trialController.WaitForPlayerToLookAt (gameObject));
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
		//shouldDie = true;
		
		PlayJuice (false);
	
		yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForTreasurePause(null));

		/*while(SpecialParticles.isPlaying && DefaultParticles.isPlaying){
			yield return 0;
		}*/

		Destroy(gameObject); //once audio & particles have finished playing, destroy the item!
	}

	void PlayJuice(bool isSpecial){
		if (Config_CoinTask.isJuice) {
			if(isSpecial){
				JuiceController.PlayParticles (SpecialParticles);
				AudioController.PlayAudio(specialCollisionSound);
			}
			else{
				JuiceController.PlayParticles (DefaultParticles);
				AudioController.PlayAudio(defaultCollisionSound);
			}
		}
	}

	public IEnumerator TurnSpecialObjectInvisible()
	{
		if (specialObject != null) {
			specialObject.GetComponent<SpawnableObject> ().TurnVisible (false);
		}
		yield return null;
	}

	 public IEnumerator QuickSpawnSpecialObject()
	{
		specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObject(specialObjectSpawnPoint.position);
		yield return StartCoroutine(TurnSpecialObjectInvisible());
		Destroy(gameObject);

	}

	IEnumerator SpawnSpecialObject(Vector3 specialSpawnPos){
		Experiment_CoinTask.Instance.scoreController.AddSpecialPoints();
		
		//TODO: spawn with default objects, show on collision???
		
		GameObject specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObject(specialSpawnPos);
		
		//set special object text
		SetSpecialObjectText (specialObject.GetComponent<SpawnableObject> ().GetDisplayName ());

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

		if (GetIsSpecial ()) {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.TREASURE_OPEN_SPECIAL, true);
		} else {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.TREASURE_OPEN_EMPTY, true);
		}

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
		
		//Quaternion origRotation = top.rotation;


		float angleChange = 8.0f;
		float directionMult = 1.0f;

		if (angleToOpen < 0) {
			directionMult = -1.0f;
		}

		if (Config_CoinTask.isJuice) {
			//animate if juice!
			while (directionMult*angleToOpen > 0) {
				if(!TrialController.isPaused){
					top.RotateAround (pivotPos, -directionMult * transform.right, angleChange);
					angleToOpen -= directionMult * angleChange;
				}
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

	void OnDestroy(){
		EndTreasureState ();
	}

	void EndTreasureState(){
		if (GetIsSpecial ()) {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.TREASURE_OPEN_SPECIAL, false);
		} else {
			TCPServer.Instance.SetState (TCP_Config.DefineStates.TREASURE_OPEN_EMPTY, false);
		}
		UnityEngine.Debug.Log ("NUM DEFAULT: " + exp.trialController.NumDefaultObjectsCollected);
		switch (exp.trialController.NumDefaultObjectsCollected) {
		case 1:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, false,1);
			break;
		case 2:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, false,2);
			break;
		case 3:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, false,3);
			break;
		case 4:
			TCPServer.Instance.SetStateWithNum (TCP_Config.DefineStates.TREASURE, false,4);
			break;
		}
	}
}
