using UnityEngine;
using System.Collections;

public class TileHighlighter : MonoBehaviour {

	[HideInInspector] public bool isHighlighted = false;
	[HideInInspector]  Material myMaterial;
	float highlightAlpha = 1.0f;
	float origMatAlpha;

	void Awake(){
		myMaterial = GetComponent<Renderer>().material;
		origMatAlpha = myMaterial.color.a;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Highlight(){
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, highlightAlpha);
		isHighlighted = true;
	}

	public void UnHighlight(){
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, origMatAlpha);
		isHighlighted = false;
	}
}
