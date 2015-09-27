using UnityEngine;
using System.Collections;

public class SelectObjectUI : LogTrack {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh ObjectNameTextMesh;
	public TextMesh PressButtonTextMesh;
	public ParticleSystem ObjectParticles;
	public AudioSource ObjectSound;
	public Transform ObjectPositionTransform;

	GameObject selectedObject = null;

	float objectScaleMult = 5.0f; //what the appropriate object scale is when in this UI

	// Use this for initialization
	void Start () {
		Enable (false);
	}

	public IEnumerator Play(GameObject objectToSelect){
		Enable (true);
		PressButtonTextMesh.gameObject.SetActive (false);

		ObjectParticles.Stop ();
		ObjectParticles.Play ();

		ObjectSound.Stop ();
		ObjectSound.Play ();
		Debug.Log ("PLAYING OBJECT SOUND? " + ObjectSound.isPlaying);

		selectedObject = objectToSelect;
		selectedObject.transform.position = ObjectPositionTransform.position;
		SpawnableObject selectedObjectSpawnable = selectedObject.GetComponent<SpawnableObject> ();
		selectedObjectSpawnable.TurnVisible (true);
		selectedObjectSpawnable.SetShadowCasting (false); //turn off shadows, they look weird in this case.
		selectedObjectSpawnable.Scale (objectScaleMult);

		ObjectNameTextMesh.text = selectedObjectSpawnable.GetName ();

		UsefulFunctions.FaceObject( objectToSelect, exp.player.gameObject, false ); //make UI copy face the player

		yield return new WaitForSeconds (Config_CoinTask.minObjselectionUITime);

		PressButtonTextMesh.gameObject.SetActive (true);

		yield return 0;
	}

	public void Stop(){
		if (selectedObject != null) {
			Destroy(selectedObject);
		}
		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}

}
