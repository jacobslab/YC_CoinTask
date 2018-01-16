using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerUI : MonoBehaviour {
	public Transform objATransform;
	public Transform objBTransform;
	public TextMesh objAText;
	public TextMesh objBText;
	public GameObject yourAnswerHighlighter;
	public GameObject correctHighlighter;

	public Color correctColor;
	public Color wrongColor;

	float yTransformCorrect = 0.61f;
	float yTransformWrong = 0.95f;
	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetCorrectColor()
	{
		yourAnswerHighlighter.transform.GetChild(1).GetComponent<SpriteRenderer> ().color = correctColor;
		yourAnswerHighlighter.transform.GetChild (0).transform.localPosition = new Vector3 (yourAnswerHighlighter.transform.GetChild (0).transform.localPosition.x, yTransformCorrect, yourAnswerHighlighter.transform.GetChild (0).transform.localPosition.z);
	}

	public void SetWrongColor()
	{
		yourAnswerHighlighter.transform.GetChild(1).GetComponent<SpriteRenderer> ().color = wrongColor;
		yourAnswerHighlighter.transform.GetChild (0).transform.localPosition = new Vector3 (yourAnswerHighlighter.transform.GetChild (0).transform.localPosition.x, yTransformWrong, yourAnswerHighlighter.transform.GetChild (0).transform.localPosition.z);
	}

	public void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
