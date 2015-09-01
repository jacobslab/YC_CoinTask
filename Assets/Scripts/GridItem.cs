using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {

	public ParticleSystem DefaultParticles;
	public ParticleSystem SpecialParticles;
	
	public AudioSource defaultCollisionSound;
	public AudioSource specialCollisionSound;
	bool shouldDie = false;

	public int rowIndex;
	public int colIndex;

	EnvironmentGrid envGrid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && (defaultCollisionSound.isPlaying == false && specialCollisionSound.isPlaying == false)){
			Destroy(gameObject); //once audio has finished playing, destroy the item!
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player" && tag == "DefaultGridItem") {
			Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();

			//turn invisible, play sound
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			shouldDie = true;

			EnvironmentGrid.GridSpotType mySpotType = envGrid.GetGridSpotType (rowIndex, colIndex);
			Debug.Log(mySpotType.ToString());
			if (mySpotType == EnvironmentGrid.GridSpotType.specialGridItem && tag == "DefaultGridItem") {
				//if it was a special spot and this is the default object...
				//...we should spawn the special object!
				Experiment_CoinTask.Instance.objectController.SpawnSpecialObjectXY(transform.position);
				SpecialParticles.Play();
				specialCollisionSound.Play ();
			}
			else{
				DefaultParticles.Play();
				defaultCollisionSound.Play();
			}
		}
	}

	void OnDestroy(){
		envGrid.RemoveGridItem (rowIndex, colIndex);
	}
}
