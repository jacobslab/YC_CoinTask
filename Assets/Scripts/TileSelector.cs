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

	bool hasInput = false;

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
	
	bool GetChangedRowColInput(){
		int origRow = selectedRow;
		int origCol = selectedCol;
		
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");
		
		float inputEpsilon = 0.2f;
		if (horizontalInput < inputEpsilon && horizontalInput > -inputEpsilon) {
			if (verticalInput < inputEpsilon && verticalInput > -inputEpsilon) {
				hasInput = false;
			}
		}

		Debug.Log("horizontal " + horizontalInput + " , vertical " + verticalInput);

		//working on this for better joystick control
		 /* if (!hasInput) {
			if (horizontalInput > 0.5f && verticalInput > 0.5f) {
				Debug.Log ("OH HAI");
				if (selectedCol < numCols - 1) {
					selectedCol += 1;
					hasInput = true;
				}
			} else if (horizontalInput < -0.5f && verticalInput < -0.5f) {
				if (selectedCol > 0) {
					selectedCol -= 1;
					hasInput = true;
				}
			}
			if (horizontalInput > 0.5f && verticalInput < -0.5f) {
				Debug.Log ("OH HAI");
				if (selectedRow > 0) {
					selectedRow -= 1;
					hasInput = true;
				}
			} else if (horizontalInput < -0.5f && verticalInput > 0.5f) {
				if (selectedRow < numRows - 1) {
					selectedRow += 1;
					hasInput = true;
				}
			} else {
				hasInput = false;
			}
		}*/

		//works well for keyboard
		if (horizontalInput == 1.0f) {
			if (selectedRow > 0) {
				selectedRow -= 1;
				hasInput = true;
			}
		} 
		else if (horizontalInput == -1.0f) {
			if (selectedRow < numRows - 1) {
				selectedRow += 1;
				hasInput = true;
			}
		} 
		else if (verticalInput == -1.0f) {
			if (selectedCol > 0) {
				selectedCol -= 1;
				hasInput = true;
			}
		} 
		else if (verticalInput == 1.0f) {
			if (selectedCol < numCols - 1) {
				selectedCol += 1;
				hasInput = true;
			}
		}
		
		if(selectedRow != origRow || selectedCol != origCol){
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
