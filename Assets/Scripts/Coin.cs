using UnityEngine;
using System.Collections;

public class Coin : GridItem {

	AudioSource coinSound;
	bool shouldDie = false;

	// Use this for initialization
	void Start () {
		coinSound = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && coinSound.isPlaying == false){
			Destroy(gameObject); //once sound has finished playing, destroy the coin
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player") {
			//turn invisible, play sound
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			coinSound.Play();
			shouldDie = true;
		}
	}

}
