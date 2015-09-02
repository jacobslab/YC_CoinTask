using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour {

	public ArcGenerator myArc;
	EnvironmentGrid grid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }
	int selectedRow = 0;
	int selectedCol = 0;
	[HideInInspector] public Tile selectedTile;

	int numRows { get { return Experiment_CoinTask.Instance.environmentController.myGrid.Rows; } }
	int numCols { get { return Experiment_CoinTask.Instance.environmentController.myGrid.Columns; } }

	bool shouldSelect = false;

	// Use this for initialization
	void Start () {
		SelectDefault ();
		Enable (shouldSelect);
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldSelect) {
			if (GetChangedRowColInput ()) {
				SelectTile ();
			}
		}
	}

	void SelectDefault(){
		selectedRow = 0;
		selectedCol = 0;
		if(numRows > 0 && numCols > 0){
			SelectTile();
		}
	}

	//enable or disable selection
	public void Enable(bool isEnabled){
		if (isEnabled) {
			shouldSelect = true;
			myArc.gameObject.SetActive(true);
			SelectDefault ();
		} 
		else {
			myArc.gameObject.SetActive(false);
			shouldSelect = false;
			if(selectedTile != null){
				selectedTile.myHighlighter.UnHighlight();
			}
		}
	}

	void SelectTile(){

		if(selectedTile != null){
			selectedTile.myHighlighter.HighlightLow();
		}
		selectedTile = grid.GetGridTile( selectedRow, selectedCol );
		selectedTile.myHighlighter.HighlightHigh();

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
