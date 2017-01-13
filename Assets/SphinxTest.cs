using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SphinxTest : MonoBehaviour {
	

	[DllImport ("ASimplePlugin")]
	private static extern IntPtr SphinxRun(int trialNumber, int recallNumber);

	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();

	[DllImport ("ASimplePlugin")]
	private static extern int AddTwoIntegers(int i1,int i2);

	[DllImport ("ASimplePlugin")]
	private static extern float AddTwoFloats(float f1,float f2); 
	// Use this for initialization
	void Start () {

//		string usbOpenFeedback = Marshal.PtrToStringAuto (SphinxRun(0,1));
//		UnityEngine.Debug.Log(usbOpenFeedback);

		UnityEngine.Debug.Log (PrintANumber ());
		UnityEngine.Debug.Log (AddTwoIntegers (3, 5));
		UnityEngine.Debug.Log (AddTwoFloats (2f, 3f));
		//int number = Marshal.ReadInt32(new IntPtr(PrintANumber ()));
		//string response = Marshal.PtrToStringAuto (Trying());
		//UnityEngine.Debug.Log (response);
		//run_sphinx ();
		//float okay=AddTwoFloats(2f,3f);
		//UnityEngine.Debug.Log (PrintANumber ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int CheckAudioResponse(int trialNumber, int recallNumber)
	{

		UnityEngine.Debug.Log("INSIDE SPHINX RESPONSE");
		string response = Marshal.PtrToStringAuto (SphinxRun(trialNumber,recallNumber));
		//UnityEngine.Debug.Log(response);

		if (response == "Found") {
			UnityEngine.Debug.Log("SPHINX FOUND");
			return 1;
		} else {
			UnityEngine.Debug.Log("SPHINX NOT ");
			return 0;
		}
	}
}
