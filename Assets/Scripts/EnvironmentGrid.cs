using UnityEngine;
using System.Collections;

public class EnvironmentGrid : MonoBehaviour {

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
			else{ //otherwise mark the grid as having a grid item
				myGrid [currItem.rowIndex, currItem.colIndex] = GridSpotType.defaultGridItem;
			}
		}

		PlaceRandomSpecialObjects (Config_CoinTask.numSpecialObjectsEasy);


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

	//don't pass too many in here, or it could take a while.
	void PlaceRandomSpecialObjects(int numSpecialObjects){

		for (int i = 0; i < numSpecialObjects; i++) {
			int randomRow = Random.Range (0, numRows);
			int randomCol = Random.Range (0, numCols);

			//if number of special objects exceeds the number of free spots, we'll get stuck.
			//...so exit the loop instead.
			if(i > numRows * numCols){
				break;
			}

			while( myGrid[randomRow, randomCol] == GridSpotType.specialGridItem){
				randomRow = Random.Range (0, numRows);
				randomCol = Random.Range (0, numCols);
			}

			myGrid [randomRow, randomCol] = GridSpotType.specialGridItem;
			Debug.Log("special item grid location: row" + randomRow + ",col" + randomCol);
		}
	}

	public GridSpotType GetGridSpotType(int rowIndex, int colIndex){
		return myGrid [rowIndex, colIndex];
	}

	public Tile GetGridTile(int rowIndex, int colIndex){
		return tileGrid [rowIndex, colIndex];
	}

	public void RemoveGridItem(int rowIndex, int colIndex){
		myGrid [rowIndex, colIndex] = GridSpotType.empty;
	}


}
