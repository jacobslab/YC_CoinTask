using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class PluginImport : MonoBehaviour {
	//Lets make our calls from the Plugin
	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();
	
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr PrintHello();
	
	[DllImport ("ASimplePlugin")]
	private static extern int AddTwoIntegers(int i1,int i2);

	[DllImport ("ASimplePlugin")]
	private static extern float AddTwoFloats(float f1,float f2);

    public Text text;
	void Start () {
        Debug.Log(PrintANumber());
        text.text = PrintANumber().ToString();
        
		Debug.Log(Marshal.PtrToStringAuto (PrintHello()));
        
		Debug.Log(AddTwoIntegers(4,2));
		Debug.Log(AddTwoFloats(2.5F,4F));
	}
}
