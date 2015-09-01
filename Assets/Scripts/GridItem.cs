using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {

	AudioSource collisionSound;
	bool shouldDie = false;

	public int rowIndex;
	public int colIndex;

	EnvironmentGrid envGrid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }

	// Use this for initialization
	void Start () {
		collisionSound = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && collisionSound.isPlaying == false){
			Destroy(gameObject); //once sound has finished playing, destroy the coin
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player" && tag == "DefaultGridItem") {
			Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();

			//turn invisible, play sound
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			collisionSound.Play();
			shouldDie = true;

			EnvironmentGrid.GridSpotType mySpotType = envGrid.GetGridSpotType (rowIndex, colIndex);
			Debug.Log(mySpotType.ToString());
			if (mySpotType == EnvironmentGrid.GridSpotType.specialGridItem && tag == "DefaultGridItem") {
				//if it was a special spot and thsi is the default object...
				//...we should spawn the special object!
				Experiment_CoinTask.Instance.objectController.SpawnSpecialObjectXY(transform.position);
			}
		}
	}

	void OnDestroy(){
		envGrid.RemoveGridItem (rowIndex, colIndex);
	}
}
