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
	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetCorrectColor()
	{
		yourAnswerHighlighter.GetComponent<SpriteRenderer> ().color = correctColor;
	}

	public void SetWrongColor()
	{
		yourAnswerHighlighter.GetComponent<SpriteRenderer> ().color = wrongColor;
	}

	public void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
