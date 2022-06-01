using UnityEngine;
using System.Collections;

public class QuestionUI : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh ObjectNameTextMesh;
	public GameObject Answers;
	public ParticleSystem ObjectParticles;
	public AudioSource ObjectSound;
	public Transform ObjectPositionTransform;

	public bool isPlaying = false;

	public AnswerSelector myAnswerSelector;

	GameObject selectedObject = null;

	float objectScaleMult = 5.0f; //what the appropriate object scale is when in this UI
	int origObjNameSize;

	// Use this for initialization
	void Start () {
		Enable (false);
		origObjNameSize = ObjectNameTextMesh.fontSize;
	}

	//no object
	public IEnumerator Play(){
		isPlaying = true;

		Enable (true);

		Answers.gameObject.SetActive (false);
		myAnswerSelector.SetShouldCheckForInput(false);

		Answers.gameObject.SetActive (true);
		myAnswerSelector.SetShouldCheckForInput (true);

		yield return 0;
	}

	//show an object
	public IEnumerator Play(GameObject objectToSelect, string objectName){
		isPlaying = true;
		UnityEngine.Debug.Log("PLAY:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		Enable(true);
		PlayerMotion.ControlPause = true;
		Yes.isenabled = true;
		Maybe.isenabled = true;
		No.isenabled = true;
		Answers.gameObject.SetActive (false);
		myAnswerSelector.SetShouldCheckForInput (false);
		UnityEngine.Debug.Log("AFTER PLAY:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		PlayObjectJuice();
		UnityEngine.Debug.Log("AFTER PLAY 2:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		selectedObject = objectToSelect;
		selectedObject.transform.position = ObjectPositionTransform.position;
		SpawnableObject selectedObjectSpawnable = selectedObject.GetComponent<SpawnableObject> ();
		selectedObjectSpawnable.TurnVisible (true);
		selectedObjectSpawnable.SetShadowCasting (false); //turn off shadows, they look weird in this case.
		selectedObjectSpawnable.Scale (objectScaleMult);
		UnityEngine.Debug.Log("AFTER PLAY 3:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		SetObjectNameText(objectName);

		UsefulFunctions.FaceObject (objectToSelect, exp.player.gameObject, false); //make UI copy face the player
		UnityEngine.Debug.Log("AFTER PLAY 4:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		Answers.gameObject.SetActive (true);
		myAnswerSelector.SetShouldCheckForInput (true);
		UnityEngine.Debug.Log("AFTER PLAY 5:   fijfowiioqrweffewfwenfiwefwieofiwefriwefiefiefoiwefoiwefoiweoiqrwqioiweoiqweoiqioqiqoiqoqoqwoqpqfgurefuiewfowef");

		yield return 0;
	}

	void PlayObjectJuice(){
		JuiceController.PlayParticles (ObjectParticles);
		AudioController.PlayAudio (ObjectSound);
	}

	void SetObjectNameText(string name){
		ObjectNameTextMesh.text = name;
		int nameLength = name.Length;
		int maxLengthBeforeResize = 13;//15 = max num chars counted in editor... a rough length estimate.
		int fontSizeDecreasePerChar = 50;
		int minFontSize = 230;
		if(nameLength > maxLengthBeforeResize){
			int numOver = nameLength - maxLengthBeforeResize;
			ObjectNameTextMesh.fontSize -= fontSizeDecreasePerChar*numOver;
			if(ObjectNameTextMesh.fontSize < minFontSize){
				ObjectNameTextMesh.fontSize = minFontSize;
			}
		}
		else{
			ResetObjectTextSize();
		}
	}

	void ResetObjectTextSize(){
		ObjectNameTextMesh.fontSize = origObjNameSize;
	}

	public void Stop(){
		isPlaying = false;

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
