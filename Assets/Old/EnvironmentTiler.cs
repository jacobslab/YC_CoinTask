using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EnvironmentTiler : MonoBehaviour {

	EnvironmentController EnvController { get { return Experiment_CoinTask.Instance.environmentController; } }

	public bool shouldTile = false;

	public GameObject myObject;
	public float distanceBetweenObjects;
	
	public Transform tileParent;


	// Use this for initialization
	void Start () {
		if (shouldTile) {
			Tile ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Tile(){
		/*int rows = EnvController.myGrid.Rows;
		int columns = EnvController.myGrid.Columns;
		Vector3 currentPosition = new Vector3( transform.position.x, myObject.transform.position.y, transform.position.z );

		for (int i = 0; i < rows; i++) { //i for rows
			for(int j = 0; j < columns; j++){ //j for columns
				currentPosition = new Vector3 (transform.position.x + (j*distanceBetweenObjects), myObject.transform.position.y, transform.position.z + (i*distanceBetweenObjects) );
				GameObject newTile = Instantiate(myObject, currentPosition, myObject.transform.rotation) as GameObject;
				newTile.transform.parent = tileParent;

				DefaultItem gridItem = newTile.GetComponent<DefaultItem>();
				if(gridItem){
					gridItem.rowIndex = i;
					gridItem.colIndex = j;
				}
				else{
					Debug.Log("no grid item attached!");
				}
			}
		}*/
	}
}
