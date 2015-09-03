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


	//used for precise joystick control
	public enum DirectionType{
		colUp,
		colDown,
		rowUp,
		rowDown,
		none
	}
	
	DirectionType lastSelectionDirection = DirectionType.none;


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

	

	bool CheckWithinBounds(float value, float min, float max){
		if (value > min && value < max) {
			return true;
		}

		return false;
	}

	bool GetChangedRowColInput(){
		int origRow = selectedRow;
		int origCol = selectedCol;
		
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");


		if (ExperimentSettings_CoinTask.isJoystickInput) {
			bool horizInMaxBounds = CheckWithinBounds(horizontalInput, 0.0f, 1.0f);
			bool vertInMaxBounds = CheckWithinBounds(verticalInput, 0.0f, 1.0f);
			bool horizInMinBounds = CheckWithinBounds(horizontalInput, -1.0f, 0.0f);
			bool vertInMinBounds = CheckWithinBounds(verticalInput, -1.0f, 0.0f);

			//both in max bounds
			if (horizInMaxBounds && vertInMaxBounds && lastSelectionDirection != DirectionType.colUp) {
				if (selectedCol < numCols - 1) {
					selectedCol += 1;
					lastSelectionDirection = DirectionType.colUp;
				}
			} 
			//both in min bounds
			else if (horizInMinBounds && vertInMinBounds && lastSelectionDirection != DirectionType.colDown) {
				if (selectedCol > 0) {
					selectedCol -= 1;
					lastSelectionDirection = DirectionType.colDown;
				}
			}
			//horiz in max bounds, vert in min bounds
			else if (horizInMaxBounds && vertInMinBounds && lastSelectionDirection != DirectionType.rowDown) {
				if (selectedRow > 0) {
					selectedRow -= 1;
					lastSelectionDirection = DirectionType.rowDown;
				}
			} 
			//horiz in min bounds, vert in max bounds
			else if (horizInMinBounds && vertInMaxBounds && lastSelectionDirection != DirectionType.rowUp) {
				if (selectedRow < numRows - 1) {
					selectedRow += 1;
					lastSelectionDirection = DirectionType.rowUp;
				}
			} 
			else if(!horizInMinBounds && !horizInMaxBounds && !vertInMinBounds && !vertInMaxBounds) {
				lastSelectionDirection = DirectionType.none;
			}

		}

		//works well for keyboard
		else {
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				if (selectedRow > 0) {
					selectedRow -= 1;
				}
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				if (selectedRow < numRows - 1) {
					selectedRow += 1;
				}
			} 
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				if (selectedCol > 0) {
					selectedCol -= 1;
				}
			} 
			else if (Input.GetKeyDown(KeyCode.UpArrow)) {
				if (selectedCol < numCols - 1) {
					selectedCol += 1;
				}
			}
		}

		if (selectedRow != origRow || selectedCol != origCol) {
			return true;
		}
		
		return false;
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

}
