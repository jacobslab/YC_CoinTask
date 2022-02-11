﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoPlay : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
	
//	MovieTexture movie;
	AudioSource movieAudio;

	private VideoPlayer vid;
	public CanvasGroup group;
	public AudioSource wavesAudio;
	void Awake(){
		group.alpha = 0.0f;
	}
	
	// Use this for initialization
	void Start () {
		vid = GetComponent<VideoPlayer> ();
		movieAudio = GetComponent<AudioSource> ();
	}

	public void EnableVideo()
	{
		vid.enabled = true;
		movieAudio.enabled = true;
	}

	public void DisableVideo()
	{
		vid.enabled = false;
		movieAudio.enabled = false;
	}

	public bool CheckIfVideoIsValid()
    {
		return ((vid!=null)?true:false);
    }

	bool isMoviePaused = false;
	void Update () {
		if (vid.clip!= null) {
			if (vid.isPlaying) {
				wavesAudio.volume = 0f;
				if (Input.GetAxis (Config_CoinTask.ActionButtonName) > 0.2f) { //skip movie!
					Debug.Log ("skip movie");
					Stop ();
				}
				if (TrialController.isPaused) {
					Pause ();
				}
			} else {
				wavesAudio.volume = 0.13f;
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
		if (vid.clip != null) {
			Debug.Log("playing instruction video");
			yield return StartCoroutine (AskIfShouldPlay());

			if (shouldPlay) {
				group.alpha = 1.0f;
			
				vid.Stop ();
				movieAudio.Play ();
				vid.Play ();
				yield return new WaitForSeconds (1f);
				while (vid.isPlaying || isMoviePaused) {
					yield return 0;
				}
				
				isMoviePaused = false;
				vid.Stop ();
				movieAudio.Stop ();
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
                Debug.Log("pressed Y");
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
		if(vid.clip != null){
			vid.Pause();
			movieAudio.Pause ();
			isMoviePaused = true;
		} 
		else {
			Debug.Log("No movie attached! Can't pause.");
		}
	}
	
	void UnPause(){
		if(vid.clip != null){
			vid.Play ();
			movieAudio.UnPause ();
			isMoviePaused = false;
		} 
		else {
			Debug.Log("No movie attached! Can't unpause.");
		}
	}
	
	void Stop(){
		if(vid.clip != null){
			isMoviePaused = false;
			movieAudio.Stop ();
			vid.Stop ();
		} 
		else {
			Debug.Log("No movie attached! Can't stop.");
		}
	}
	
}
