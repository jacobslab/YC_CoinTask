using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentGrid : MonoBehaviour {
	//move this elsewhere?
	public GameObject DefaultObject;


	int numRows = 6;
	int numCols = 10;
	public int Rows { get { return numRows; } }
	public int Columns { get { return numCols; } }

	GridSpotType[,] myGrid;
	Tile[,] tileGrid;

	int homeBaseRow = 0;
	int homeBaseCol = 0;

	//TODO: move to GridItem.cs?
	public enum GridSpotType{
		empty,
		homeBase,
		defaultGridItem,
		specialGridItem
	}

	// Use this for initialization
	void Awake () {
		InitGrids ();	
	}

	void InitGrids(){
		myGrid = new GridSpotType[numRows, numCols];
		myGrid [homeBaseRow, homeBaseCol] = GridSpotType.homeBase;

		GameObject[] gridItems = GameObject.FindGameObjectsWithTag ("DefaultGridItem");

		for (int i = 0; i < gridItems.Length; i++) {
			GridItem currItem = gridItems [i].GetComponent<GridItem> ();

			//destroy the item if it's in the home base spot
			if (GetGridSpotType(currItem.rowIndex, currItem.colIndex) == GridSpotType.homeBase) {
				Debug.Log("there's an item in home base!");
				Destroy(currItem.gameObject);
			}
			else{ //otherwise mark the grid as empty. we will put objects in the grid when we generate Trials.
				myGrid [currItem.rowIndex, currItem.colIndex] = GridSpotType.empty;
			}
		}


		InitTileGrid ();
	}

	void InitTileGrid(){
		tileGrid = new Tile[numRows, numCols];

		Tile[] gridTiles = GameObject.FindObjectsOfType<Tile> ();

		for (int i = 0; i < gridTiles.Length; i++) {
			GridItem tileItem = gridTiles[i].GetComponent<GridItem>();
			tileGrid[tileItem.rowIndex, tileItem.colIndex] = gridTiles[i];
		}
	}

	//clears grid of all objects
	public void Clear(){
		for (int i = 0; i < Rows; i++) {
			for( int j = 0; j < Columns; j++){
				if(myGrid[i, j] != GridSpotType.homeBase){ // keep homebase location
					myGrid[i,j] = GridSpotType.empty;
				}
			}
		}
	}

	//for use with pregenerated trials
	public void SetConfiguration(List<Vector2> defaultIndices, List<Vector2> specialIndices){
		for(int i = 0; i < defaultIndices.Count; i++){
			myGrid[(int)defaultIndices[i].x, (int)defaultIndices[i].y] = GridSpotType.defaultGridItem;
		}
		for (int i = 0; i < specialIndices.Count; i++) {
			myGrid[(int)specialIndices[i].x, (int)specialIndices[i].y] = GridSpotType.specialGridItem;
		}
	}

	//spawns default objects and returns a list of default object indices in the grid
	public List<Vector2> GenerateDefaultObjectConfiguraiton(int numDefaultObjects){
		List<Vector2> IndicesList = new List<Vector2> ();

		for (int i = 0; i < numDefaultObjects; i++) {
			int randomRow = Random.Range (0, numRows);
			int randomCol = Random.Range (0, numCols);

			//if number of special objects exceeds the number of free spots, we'll get stuck.
			//...so exit the loop instead.
			if(i > numRows * numCols){
				break;
			}
			
			while( myGrid[randomRow, randomCol] == GridSpotType.homeBase){ //if we hit home base, generate again...
				randomRow = Random.Range (0, numRows);
				randomCol = Random.Range (0, numCols);
			}
			IndicesList.Add(new Vector2(randomRow, randomCol));
			myGrid [randomRow, randomCol] = GridSpotType.defaultGridItem;
		}
		return IndicesList;
	}

	//spawns special objects (based on a default object configuration) and returns a list of their indices in the grid
	public List<Vector2> GenerateSpecialObjectConfiguration(List<Vector2> defaultObjectIndices, int numSpecialObjects){
		List<Vector2> specialIndices = new List<Vector2> ();

		for (int i = 0; i < numSpecialObjects; i++) {
			int randomIndex = Random.Range(0, defaultObjectIndices.Count);

			//if number of special objects exceeds the number of free spots, we'll get stuck.
			//...so exit the loop instead.
			if(i > defaultObjectIndices.Count){
				break;
			}

			Vector2 currIndices = defaultObjectIndices[i];

			myGrid [(int)currIndices.x, (int)currIndices.y] = GridSpotType.specialGridItem;
			specialIndices.Add (currIndices);

			Debug.Log("special item grid location: row" + currIndices.x + ",col" + currIndices.y);
		}

		return specialIndices;
	}

	public GridSpotType GetGridSpotType(int rowIndex, int colIndex){
		return myGrid [rowIndex, colIndex];
	}

	public Tile GetGridTile(int rowIndex, int colIndex){
		return tileGrid [rowIndex, colIndex];
	}

	public Vector3 GetGridPosition(int rowIndex, int colIndex){
		Tile tile = GetGridTile (rowIndex, colIndex);
		return tile.transform.position;
	}

	public void TurnOnTileVisibility(bool isOn){
		for(int i = 0; i < numRows; i++){
			for(int j = 0; j < numCols; j++){
				if(isOn){
					tileGrid[i,j].GetComponent<Tile>().myHighlighter.HighlightLow();
				}
				else{
					tileGrid[i,j].GetComponent<Tile>().myHighlighter.UnHighlight();
				}
			}
		}
	}

	public void RemoveGridItem(int rowIndex, int colIndex){
		myGrid [rowIndex, colIndex] = GridSpotType.empty;
	}


}
