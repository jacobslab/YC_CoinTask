using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SphinxTest : MonoBehaviour {
	[DllImport ("SphinxTH")]
	private static extern IntPtr nice();
	private static extern IntPtr SphinxRun();

//	private static extern int AddTwoIntegers(int a,int b);
	//private static extern IntPtr run_sphinx();
	[DllImport("ASimplePlugin")]
	//private static extern float AddTwoFloats (float a,float b);
	private static extern IntPtr Trying();
	//private static extern IntPtr Trying();
	//private static extern int PrintANumber ();
	// Use this for initialization
	void Start () {
		string usbOpenFeedback = Marshal.PtrToStringAuto (SphinxRun());
		UnityEngine.Debug.Log(usbOpenFeedback);
		//string response = Marshal.PtrToStringAuto (Trying());
		//UnityEngine.Debug.Log (response);
		//run_sphinx ();
		//UnityEngine.Debug.Log(AddTwoFloats(3f,2f));
		//UnityEngine.Debug.Log (PrintANumber ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
