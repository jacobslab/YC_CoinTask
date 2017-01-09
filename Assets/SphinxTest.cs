using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SphinxTest : MonoBehaviour {
	[DllImport ("SphinxTH")]
	private static extern IntPtr PrintHello();
	private static extern int PrintANumber();
	private static extern int AddTwoIntegers(int a,int b);
	//private static extern IntPtr run_sphinx();
	[DllImport("ASimplePlugin")]
	private static extern float AddTwoFloats (float a,float b);
	// Use this for initialization
	void Start () {
		//string usbOpenFeedback = Marshal.PtrToStringAuto (PrintHello());
		//UnityEngine.Debug.Log(usbOpenFeedback);
		//run_sphinx ();
		UnityEngine.Debug.Log(AddTwoFloats(3f,2f));
		UnityEngine.Debug.Log (AddTwoIntegers(3,2));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
