using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {

	public int rowIndex;
	public int colIndex;

	EnvironmentGrid envGrid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		if (envGrid.GetGridSpotType (rowIndex, colIndex) == EnvironmentGrid.GridSpotType.specialGridItem && tag == "DefaultGridItem") {
			//if it was a special spot and thsi is the default object...
			//...we should spawn the special object!
			Experiment_CoinTask.Instance.objectController.SpawnRandomObjectXY(transform.position);
		}
		else {
			envGrid.RemoveGridItem (rowIndex, colIndex);
		}
	}
}
