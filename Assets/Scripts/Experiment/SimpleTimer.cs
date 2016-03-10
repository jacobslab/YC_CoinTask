using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour {

	public Text timerText;
	float seconds = 0;

	bool isRunning = false;
	public bool IsRunning { get { return isRunning; } } //public getter. don't want people setting isRunning outside of here.

	public bool isCountDownTimer;

	public delegate void ResetDelegate();
	public ResetDelegate myResetDelegate;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (isRunning) {
			UpdateTimer();
		}
	}

	void UpdateTimer(){
		float countMult = 1.0f;
		if (isCountDownTimer) {
			countMult = -1.0f;
		}

		seconds += (countMult * Time.deltaTime);

		if (seconds < 0.0f) {
			seconds = 0.0f;
		}

		if(timerText != null){
			int displayMinutes = (int)Mathf.Floor (seconds / 60.0f);
			int displaySeconds = (int)Mathf.Floor (seconds - (displayMinutes * 60));
			if (displaySeconds < 10) {
				timerText.text = displayMinutes + ":0" + displaySeconds;
			} else {
				timerText.text = displayMinutes + ":" + displaySeconds;
			}
		}
	}

	public void StartTimer(){
		isRunning = true;
	}

	public void StopTimer(){
		isRunning = false;
	}

	public void ResetTimerNoDelegate (float newSeconds){
		seconds = newSeconds;
	}

	public void ResetTimer(float newSeconds){
		myResetDelegate ();
		seconds = newSeconds;
	}

	public int GetSecondsInt(){
		return Mathf.FloorToInt(seconds);
	}

	public float GetSecondsFloat(){
		return seconds;
	}
}
