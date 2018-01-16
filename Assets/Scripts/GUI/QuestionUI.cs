using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	List<GameObject> temporalObjects;
	public List<int> answerList;
	public List<int> scoreList;
	public List<int> flippedList;

	public AnswerUI answerUI;

	float objectScaleMult = 5.0f; //what the appropriate object scale is when in this UI
	int origObjNameSize;

	// Use this for initialization
	void Start () {
		Enable (false);
		origObjNameSize = ObjectNameTextMesh.fontSize;
		answerList = new List<int> ();
		scoreList = new List<int> ();
		flippedList = new List<int> ();
		ResetTemporalObjects ();
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

		myAnswerSelector.SetupAnswerSelector (optionAName, optionBName);

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

	void ResetTemporalObjects()
	{
		temporalObjects = new List<GameObject> ();
	}

	public void Stop(){
		isPlaying = false;

		if (selectedObject != null) {
			Destroy(selectedObject);
		}
		//turn them off as we'll need to reactivate them for feedback
		if (selectableObjectA != null) {
			selectableObjectA.SetActive (false);
			temporalObjects.Add (selectableObjectA);
		}
		if (selectableObjectB != null) {
			selectableObjectB.SetActive (false);
			temporalObjects.Add (selectableObjectB);
		}
		
		Enable (false);
	}

	public IEnumerator ShowAnswer()
	{

		answerUI.Enable (true);
		//we will retrieve the stored lists in groups of two in the same order we had questioned them in
		for (int i = 0; i < temporalObjects.Count / 2; i++) {
			answerUI.objAText.text = temporalObjects [i*2].GetComponent<SpawnableObject>().GermanName;
			//answerUI.objATransform = temporalObjects [i].transform;
			temporalObjects [i*2].SetActive (true);

			answerUI.objBText.text = temporalObjects [(i*2)+1].GetComponent<SpawnableObject>().GermanName;
			//answerUI.objBTransform = temporalObjects [i].transform;
			temporalObjects [(i*2)+1].SetActive (true);
			if (flippedList [i] == 0)
				Debug.Log ("NOT FLIPPED");
			else
				Debug.Log ("flipped");
			if (answerList [i] == 0) {
				answerUI.correctHighlighter.transform.position = answerUI.objATransform.position;
				
				if (scoreList [i] == 1) {
					exp.trialController.temporalScore += 200;
					Debug.Log ("mark and score as CORRECT ANSWER");
					answerUI.SetCorrectColor ();
					Debug.Log ("marked your CORRECT answer as A");
					answerUI.yourAnswerHighlighter.transform.position = answerUI.objATransform.position;
				} else { 
					Debug.Log ("mark and score as INCORRECT ANSWER");
					answerUI.SetWrongColor ();

					Debug.Log ("marked your WRONG answer as B");
					answerUI.yourAnswerHighlighter.transform.position = answerUI.objBTransform.position;
				}
			}
				else {
					answerUI.correctHighlighter.transform.position = answerUI.objBTransform.position;
				
				if (scoreList [i] == 1) {
					exp.trialController.temporalScore += 200;
					Debug.Log ("mark and score as CORRECT ANSWER");
					answerUI.SetCorrectColor ();
					Debug.Log ("marked your CORRECT answer as B");
					answerUI.yourAnswerHighlighter.transform.position = answerUI.objBTransform.position;
				} else { 
					Debug.Log ("mark and score as INCORRECT ANSWER");
					answerUI.SetWrongColor ();
						Debug.Log ("marked your WRONG answer as A NOT FLIPPED");
						answerUI.yourAnswerHighlighter.transform.position = answerUI.objATransform.position;
				}
			}


			
			yield return new WaitForSeconds (3f);
			Destroy(temporalObjects [i*2]);
			Destroy (temporalObjects [(i*2) + 1]);
				
		}
		answerUI.Enable (false);
		answerList.Clear ();
		temporalObjects.Clear ();
		scoreList.Clear ();
		flippedList.Clear ();
		yield return null;
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}

}
