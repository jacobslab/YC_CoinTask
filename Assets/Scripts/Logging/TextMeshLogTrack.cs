using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//THIS CLASS IS SO CLOSE TO A DIRECT COPY OF TEXTLOGTRACK.CS. 
//...this bothers me, but alas...
public class TextMeshLogTrack : LogTrack {

	TextMesh myText;
	string currentText = "";

	bool firstLog = false; //should make an initial log

	// Use this for initialization
	void Awake () {
		myText = GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (myText == null) {
			Debug.Log("Text is null! Did you mean to add a regular TextLogTrack instead?");
		}
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

		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "TEXT_MESH" + separator + textToLog );
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "TEXT_MESH_COLOR" + separator + myText.color.r + separator + myText.color.g + separator + myText.color.b + separator + myText.color.a);
	}
}