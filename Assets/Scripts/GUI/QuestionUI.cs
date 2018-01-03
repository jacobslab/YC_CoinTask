using UnityEngine;
using System.Collections;

public class QuestionUI : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh ObjectNameTextMesh;
	public TextMesh ObjectNameTextMesh_Right;
	public GameObject Answers;
	public ParticleSystem ObjectParticles;
	public AudioSource ObjectSound;
	public Transform ObjectPositionTransform;


	public Transform ObjectAPositionTransform;
	public Transform ObjectBPositionTransform;

	public bool isPlaying = false;

	public AnswerSelector myAnswerSelector;

	GameObject selectedObject = null;

	GameObject selectableObjectA=null;
	GameObject selectableObjectB=null;

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

		Enable (true);
		Answers.gameObject.SetActive (false);
		myAnswerSelector.SetShouldCheckForInput (false);

		PlayObjectJuice ();

		selectedObject = objectToSelect;
		selectedObject.transform.position = ObjectPositionTransform.position;
		SpawnableObject selectedObjectSpawnable = selectedObject.GetComponent<SpawnableObject> ();
		selectedObjectSpawnable.TurnVisible (true);
		selectedObjectSpawnable.SetShadowCasting (false); //turn off shadows, they look weird in this case.
		selectedObjectSpawnable.Scale (objectScaleMult);

		SetObjectNameText(objectName);

		UsefulFunctions.FaceObject (objectToSelect, exp.player.gameObject, false); //make UI copy face the player

		Answers.gameObject.SetActive (true);
		myAnswerSelector.SetShouldCheckForInput (true);

		yield return 0;
	}

	public IEnumerator Play(GameObject optionA, GameObject optionB, string optionAName, string optionBName){
		isPlaying = true;

		Debug.Log ("should enable children");
		Enable (true);
		Answers.gameObject.SetActive (false);
		myAnswerSelector.SetShouldCheckForInput (false);

		PlayObjectJuice ();

		selectableObjectA = optionA;
		selectableObjectA.transform.position = ObjectAPositionTransform.position;
		SpawnableObject selectedObjectSpawnableA = selectableObjectA.GetComponent<SpawnableObject> ();
		selectedObjectSpawnableA.TurnVisible (true);
		selectedObjectSpawnableA.SetShadowCasting (false); //turn off shadows, they look weird in this case.
		selectedObjectSpawnableA.Scale (objectScaleMult);
		SetObjectNameText(ObjectNameTextMesh,optionAName);
		UsefulFunctions.FaceObject (selectableObjectA, exp.player.gameObject, false); //make UI copy face the player

		selectableObjectB = optionB;
		selectableObjectB.transform.position = ObjectBPositionTransform.position;
		SpawnableObject selectedObjectSpawnableB = selectableObjectB.GetComponent<SpawnableObject> ();
		selectedObjectSpawnableB.TurnVisible (true);
		selectedObjectSpawnableB.SetShadowCasting (false); //turn off shadows, they look weird in this case.
		selectedObjectSpawnableB.Scale (objectScaleMult);
		SetObjectNameText(ObjectNameTextMesh_Right,optionBName);
		UsefulFunctions.FaceObject (selectableObjectB, exp.player.gameObject, false); //make UI copy face the player

		Answers.gameObject.SetActive (true);
		myAnswerSelector.SetShouldCheckForInput (true);

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

	void SetObjectNameText(TextMesh ObjectNameTextMesh,string name){
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
		if (selectableObjectA != null)
			Destroy (selectableObjectA);
		if (selectableObjectB != null)
			Destroy (selectableObjectB);
		
		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}

}
