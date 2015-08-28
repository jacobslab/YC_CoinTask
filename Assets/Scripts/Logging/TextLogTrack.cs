using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextLogTrack : LogTrack {

	Text myText;
	string currentText = "";

	bool firstLog = false; //should make an initial log

	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings_CoinTask.isLogging && ( currentText != myText.text || !firstLog) ){ //if the text has changed, log it!
			firstLog = true;
			LogText ();
		}
	}

	void LogText(){
		currentText = myText.text;

		string textToLog = myText.text;

		if (myText.text == "") {
			textToLog = " "; //log a space -- makes it easier to read it during replay!
		}
		else {
			textToLog = textToLog.Replace (System.Environment.NewLine, "_NEWLINE_");
		}

		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "TEXT" + separator + textToLog );
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "TEXT_COLOR" + separator + myText.color.r + separator + myText.color.g + separator + myText.color.b + separator + myText.color.a);
	}
}