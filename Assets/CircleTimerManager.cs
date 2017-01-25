using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleTimerManager : MonoBehaviour {

	public bl_ProgressBar circleTimer;
	// Use this for initialization
	void Start () {

		TurnVisible (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ResetTimer()
	{
		circleTimer.Value = 0f;

	}

	void TurnVisible(bool isVisible)
	{
		if(isVisible)
			transform.GetChild (0).GetComponent<CanvasGroup> ().alpha = 1f;
		else
			transform.GetChild (0).GetComponent<CanvasGroup> ().alpha = 0f;
		circleTimer.transform.parent.GetChild (0).GetComponent<Image> ().enabled = isVisible; //the green radial circle
		circleTimer.GetComponent<Image> ().enabled = isVisible;
	}

	public IEnumerator InitiateTimerCountdown(float maxTime)
	{
		ResetTimer ();
		TurnVisible (true);
		circleTimer.MaxValue = maxTime;
		float currentTime = 0f;

		while (currentTime < maxTime) {
			currentTime += Time.deltaTime;
			circleTimer.Value = currentTime;
			yield return 0;
		}
		TurnVisible (false);
		yield return null;
	}
}
