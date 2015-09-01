using UnityEngine;
using System.Collections;

public class TileHighlighter : MonoBehaviour {

	[HideInInspector] public bool isHighlighted = false;
	[HideInInspector]  Material myMaterial;
	float highlightAlphaLow = 0.5f;
	float highlightAlphaHigh = 1.0f;
	float origMatAlpha;

	Color origColor;

	void Awake(){
		myMaterial = GetComponent<Renderer>().material;
		origMatAlpha = myMaterial.color.a;
		origColor = myMaterial.color;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSpecialColor(Color newColor){
		myMaterial.color = newColor;
	}

	public void ResetColor(){
		myMaterial.color = origColor;
	}

	public void HighlightHigh(){
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, highlightAlphaHigh);
		isHighlighted = true;
	}

	public void HighlightLow(){
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, highlightAlphaLow);
		isHighlighted = true;
	}

	public void UnHighlight(){
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, origMatAlpha);
		isHighlighted = false;
	}
}
