using UnityEngine;
using System.Collections;

public class EnvironmentGrid : MonoBehaviour {

	int numRows = 6;
	int numCols = 10;
	public int Rows { get { return numRows; } }
	public int Columns { get { return numCols; } }

	GridSpotType[,] myGrid;

	//TODO: move to GridItem.cs?
	public enum GridSpotType{
		empty,
		defaultGridItem,
		specialGridItem
	}

	// Use this for initialization
	void Start () {
		InitGrid ();	
	}

	void InitGrid(){
		myGrid = new GridSpotType[numRows, numCols];

		GameObject[] gridItems = GameObject.FindGameObjectsWithTag ("DefaultGridItem");

		for(int i = 0; i < gridItems.Length; i++){
			GridItem currItem = gridItems[i].GetComponent<GridItem>();
			myGrid[currItem.rowIndex, currItem.colIndex] = GridSpotType.defaultGridItem;
		}

		PlaceRandomSpecialObjects (Config_CoinTask.numSpecialObjectsEasy);
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
		return myGrid[rowIndex, colIndex];
	}

	public void RemoveGridItem(int rowIndex, int colIndex){
		myGrid [rowIndex, colIndex] = GridSpotType.empty;
	}


}
