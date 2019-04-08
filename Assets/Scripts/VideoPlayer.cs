using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayer : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
	
	VideoPlayer movie;
	AudioSource movieAudio;
	
	public CanvasGroup group;
	
	void Awake(){
		group.alpha = 0.0f;
	}

    //TODO: FIX video playing
	
	// Use this for initialization
	void Start () {
        //RawImage rim = GetComponent<RawImage>();
        //if(rim != null){
        //	if(rim.texture != null){
        //		movie =
        //	}
        //}
        movie = GetComponent<VideoPlayer>();
		movieAudio = GetComponent<AudioSource> ();
	}
	
	bool isMoviePaused = false;
	void Update () {
		if (movie != null) {
			if (!movie.isMoviePaused) {
				if (Input.GetAxis (Config_CoinTask.ActionButtonName) > 0.2f) { //skip movie!
					Debug.Log("skip movie");
					Stop ();
				}
				if (TrialController.isPaused) {
					Pause ();
				}
			}
			if (!TrialController.isPaused) {
				if (isMoviePaused) {
					UnPause ();
				}
			}
		} 
		//else {
			//Debug.Log("No movie attached! Can't update.");
		//}
	}

	bool shouldPlay = false;
	public IEnumerator Play(){
		if (movie != null) {
			Debug.Log("playing instruction video");
			yield return StartCoroutine (AskIfShouldPlay());

			if (shouldPlay) {
				group.alpha = 1.0f;
			
				movie.Stop ();
				movieAudio.Play ();
				movie.Play ();
			
				while (movie.isMoviePaused) {
					yield return 0;
				}
			
				group.alpha = 0.0f;
			}
			yield return 0;
		} 
		else {
			Debug.Log("No movie attached! Can't play.");
		}
	}

	IEnumerator AskIfShouldPlay(){
		exp.currInstructions.SetInstructionsColorful ();
		exp.currInstructions.DisplayText ("Play instruction video? (y/n)");
		Debug.Log("show instructions");
		bool isValidInput = false;
		while (!isValidInput) {
			if (Input.GetKeyUp (KeyCode.Y)) {
				isValidInput = true;
				shouldPlay = true;
			}
			else if (Input.GetKeyUp (KeyCode.N)) {
				isValidInput = true;
				shouldPlay = false;
			}
			yield return 0;
		}

		exp.currInstructions.SetInstructionsBlank ();
		exp.currInstructions.SetInstructionsTransparentOverlay ();
	}
	
	void Pause(){
		if(movie != null){
			movie.Pause();
			movieAudio.Pause ();
			isMoviePaused = true;
		} 
		else {
			Debug.Log("No movie attached! Can't pause.");
		}
	}
	
	void UnPause(){
		if(movie != null){
			movie.Play ();
			movieAudio.UnPause ();
			isMoviePaused = false;
		} 
		else {
			Debug.Log("No movie attached! Can't unpause.");
		}
	}
	
	void Stop(){
		if(movie != null){
			isMoviePaused = false;
			movie.Stop ();
		} 
		else {
			Debug.Log("No movie attached! Can't stop.");
		}
	}
	
}
