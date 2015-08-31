using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour {

	public GameObject[] floorGridSingular;

	public ArcGenerator myArc;
	EnvironmentGrid grid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }
	int selectedRow = 0;
	int selectedCol = 0;
	Tile selectedTile;

	int numRows { get { return Experiment_CoinTask.Instance.environmentController.myGrid.Rows; } }
	int numCols { get { return Experiment_CoinTask.Instance.environmentController.myGrid.Columns; } }

	// Use this for initialization
	void Start () {

		if(numRows > 0 && numCols > 0){
			SelectTile();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(GetChangedRowColInput()){
			SelectTile();
		}
	}

	void SelectTile(){


		if(selectedTile != null){
			selectedTile.myHighlighter.UnHighlight();
		}
		selectedTile = grid.GetGridTile( selectedRow, selectedCol );
		selectedTile.myHighlighter.Highlight();

		myArc.GenerateArc(selectedTile.transform.position);
	}

	bool GetChangedRowColInput(){
		int origRow = selectedRow;
		int origCol = selectedCol;

		if(Input.GetKeyDown(KeyCode.RightArrow)){
			if(selectedRow > 0){
				selectedRow -= 1;
			}
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow)){
			if(selectedRow < numRows - 1){
				selectedRow += 1;
			}
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow)){
			if(selectedCol > 0){
				selectedCol -= 1;
			}
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow)){
			if(selectedCol < numCols - 1){
				selectedCol += 1;
				Debug.Log(selectedCol);
			}
		}

		if(selectedRow != origRow || selectedCol != origCol){
			return true;
		}

		return false;
	}
}
