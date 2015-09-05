using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	[HideInInspector] public TileHighlighter myHighlighter;
	GridItem MyGridItem;
	public Vector2 GridIndices { get { return GetGridIndices(); } }

	void Awake(){
		MyGridItem = GetComponent<GridItem>();
		myHighlighter = GetComponent<TileHighlighter>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Vector2 GetGridIndices(){
		return new Vector2 (MyGridItem.rowIndex, MyGridItem.colIndex);
	}
}
