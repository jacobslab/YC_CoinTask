//SCRIPT MODIFIED FROM: http://wiki.unity3d.com/index.php/Mic_Input

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RecordAudio : MonoBehaviour {



	public float sensitivity = 100;
	public float ramFlushSpeed = 5;//The smaller the number the faster it flush's the ram, but there might be performance issues...
	[Range(0,100)]
	public float sourceVolume = 100;//Between 0 and 100
	public bool GuiSelectDevice = true;
	//
	public string selectedDevice { get; private set; }	
	public float loudness { get; private set; } //dont touch
	//
	//private bool micSelected = false;
	private float ramFlushTimer;
	private int amountSamples = 256; //increase to get better average, but will decrease performance. Best to leave it
	private int minFreq, maxFreq; 
	private List<string> objectNames;
	public AudioSource beepHigh;
	public AudioSource beepLow;
	AudioSource audio;
	string path="";
	public AudioSource tada;
	void Start() {
		path = System.Environment.UserName + "_audio/";

		audio = GetComponent<AudioSource> ();
		objectNames = new List<string> ();
		if (CheckForRecordingDevice ()) {
			Debug.Log(Microphone.devices.Length);
			audio.loop = true; // Set the AudioClip to loop
			audio.mute = false; // Mute the sound, we don't want the player to hear it
			selectedDevice = Microphone.devices [0].ToString ();
			//micSelected = true;
			GetMicCaps ();
			StartCoroutine ("RecordWords");
		}
	}

	IEnumerator RecordWords()
	{
		yield return StartCoroutine ("ReadWords");
		for (int i = 0; i<objectNames.Count; i++) {
			beepHigh.Play ();
			recordText.text = objectNames [i];
			yield return StartCoroutine(Record(path,objectNames[i]+".wav",2));
			//beepLow.Play ();
			recordText.text = "";
			yield return new WaitForSeconds (0.25f);
		}
		tada.Play ();
		recordText.text = "Complete! Exiting application now...";
		yield return new WaitForSeconds (2f);
		Application.Quit ();
		yield return null;
	}

	IEnumerator ReadWords()
	{
		string[] lines = new string[111];
		lines = System.IO.File.ReadAllLines ("updatedNames.txt");
		for (int i = 0; i < lines.Length; i++) {
			objectNames.Add (lines [i]);
		}
		yield return null;
	}

	public static bool CheckForRecordingDevice(){
		if (Microphone.devices.Length > 0) {
			return true;
		}
		return false;
	}

	public void GetMicCaps () {
		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
		if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
			maxFreq = 44100;
	}

	/* //FOR DEBUGGING / TESTING
	int numRecordings = 0;
	void GetInput(){
		if (Input.GetKeyDown (KeyCode.A)) {
			StartCoroutine(Record("/Users/coreynovich/Desktop/Unity/DeliveryBoy/TextFiles", "testRecord" + numRecordings, 4));
			numRecordings++;
		}
	}

	void Update() {
		GetInput ();
	}*/

	public Text recordText;
	public IEnumerator Record(string filePath, string fileName, int duration){
		if (Microphone.devices.Length > 0) {
			Debug.Log (filePath);
			Color origTextColor = recordText.color;
			recordText.color = Color.green;
			StartMicrophone (duration);
			yield return new WaitForSeconds (duration);

			StopMicrophone ();
			recordText.color = origTextColor;

			SavWav.Save (filePath, fileName, audio.clip);
		} else {
			Debug.Log("No mic to record with!");
			yield return new WaitForSeconds(duration);
		}
	}

	public void StartMicrophone (int duration) {
		audio.clip = Microphone.Start(selectedDevice, true, duration, 16000);//Starts recording
		while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
		//audio.Play(); // Play the audio source!
	}

	public void StopMicrophone () {
		audio.Stop();//Stops the audio
		Microphone.End(selectedDevice);//Stops the recording of the device	
	}		

	private void RamFlush () {
		if (ramFlushTimer >= ramFlushSpeed && Microphone.IsRecording(selectedDevice)) {
			StopMicrophone();
			StartMicrophone(10);
			ramFlushTimer = 0;
		}
	}

	float GetAveragedVolume() {
		float[] data = new float[amountSamples];
		float a = 0;
		audio.GetOutputData(data,0);
		foreach(float s in data) {
			a += Mathf.Abs(s);
		}
		return a/amountSamples;
	}
}