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

	public Vector2 GetGridIndices(){
		return new Vector2 (rowIndex, colIndex);
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && (defaultCollisionSound.isPlaying == false && specialCollisionSound.isPlaying == false)){
			Destroy(gameObject); //once audio has finished playing, destroy the item!
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player" && tag == "DefaultGridItem") {

			shouldDie = true;

			EnvironmentGrid.GridSpotType mySpotType = envGrid.GetGridSpotType (rowIndex, colIndex);

			//Check if it's a treasure chest
			TreasureChest chest = GetComponent<TreasureChest>();
			Vector3 specialSpawnPos = transform.position;
			if(chest != null){
				StartCoroutine(chest.Open(Experiment_CoinTask.Instance.player.gameObject)); //TODO: move particle systems to chest???
				specialSpawnPos = chest.treasureSpawnPoint.position;
			}

			if (mySpotType == EnvironmentGrid.GridSpotType.specialGridItem && tag == "DefaultGridItem") {

				Experiment_CoinTask.Instance.scoreController.AddSpecialPoints();

				//if it was a special spot and this is the default object...
				//...we should spawn the special object!
				//TODO: spawn with default objects, show on collision???

				GameObject specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObjectXY(new Vector2(rowIndex, colIndex), specialSpawnPos);
				//tell the trial controller to wait for the animation
				StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForSpecialAnimation(specialObject));

				SpecialParticles.Play();
				Debug.Log(SpecialParticles.isPlaying);
				specialCollisionSound.Play ();
			}
			else{
				Experiment_CoinTask.Instance.scoreController.AddDefaultPoints();

				DefaultParticles.Play();
				defaultCollisionSound.Play();
				Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();
			}
		}
	}

	void OnDestroy(){
		envGrid.RemoveGridItem (rowIndex, colIndex);
	}
}
