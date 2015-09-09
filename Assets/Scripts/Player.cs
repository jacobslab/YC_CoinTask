using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public PlayerControls controls;
	//public TileSelector tileSelector;
	public EnvironmentPositionSelector positionSelector;
	public GameObject visuals;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnOnVisuals(bool isVisible){
		visuals.SetActive (isVisible);
	}
	
}
