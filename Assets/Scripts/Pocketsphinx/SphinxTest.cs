using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SphinxTest : MonoBehaviour {
	

	[DllImport ("SphinxPlugin")]
	private static extern IntPtr SphinxRun(int trialNumber, int recallNumber,int kws_threshold);

	[DllImport ("SphinxPlugin")]
	private static extern int PrintANumber();

	[DllImport ("SphinxPlugin")]
	private static extern int AddTwoIntegers(int i1,int i2);

	[DllImport ("SphinxPlugin")]
	private static extern float AddTwoFloats(float f1,float f2); 

	[DllImport ("SphinxPlugin")]
	private static extern int SetAudioPath(string someStr);

	[DllImport ("SphinxPlugin")]
	private static extern IntPtr GetAudioPath();
	// Use this for initialization
	void Start () {

//		string usbOpenFeedback = Marshal.PtrToStringAuto (SphinxRun(0,1));
//		UnityEngine.Debug.Log(usbOpenFeedback);


		UnityEngine.Debug.Log (PrintANumber ());
		UnityEngine.Debug.Log (AddTwoIntegers (3, 5));
		UnityEngine.Debug.Log (AddTwoFloats (2f, 3f));
//		string path = Marshal.PtrToStringAuto (GetAudioPath ());
//		UnityEngine.Debug.Log (path);
//		//int number = Marshal.ReadInt32(new IntPtr(PrintANumber ()));
		//string response = Marshal.PtrToStringAuto (Trying());
		//UnityEngine.Debug.Log (response);
		//run_sphinx ();
		//float okay=AddTwoFloats(2f,3f);
		//UnityEngine.Debug.Log (PrintANumber ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPath(string audioPath)
	{
		UnityEngine.Debug.Log ("set sphinx audio path to " + audioPath);
		SetAudioPath(audioPath);
	}

	public int CheckAudioResponse(int trialNumber, int recallNumber,string actualName,string kws_threshold)
	{

		UnityEngine.Debug.Log("INSIDE SPHINX RESPONSE " + actualName);
		int threshInt = int.Parse (kws_threshold);
		string response = Marshal.PtrToStringAuto (SphinxRun(trialNumber,recallNumber,threshInt));
		UnityEngine.Debug.Log(response);

		if (response == "found") {
			UnityEngine.Debug.Log("SPHINX FOUND");
			return 1;
		} else {
			UnityEngine.Debug.Log("SPHINX NOT ");
			return 0;
		}
	}
}
