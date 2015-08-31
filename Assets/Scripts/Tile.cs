using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	[HideInInspector] public TileHighlighter myHighlighter;

	void Awake(){
		myHighlighter = GetComponent<TileHighlighter>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
