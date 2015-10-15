using UnityEngine;
using System.Collections;

public class ChosenIndicatorController : MonoBehaviour {

	public Color RightColor;
	public Color WrongColor;

	ColorChanger colorChanger;

	void Awake(){
		colorChanger = GetComponent<ColorChanger> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeToRightColor(){
		colorChanger.ChangeColor (RightColor);
	}

	public void ChangeToWrongColor(){
		colorChanger.ChangeColor (WrongColor);
	}
}
