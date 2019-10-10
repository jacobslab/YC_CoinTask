using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SphinxTest : MonoBehaviour
{


    [DllImport("SphinxPlugin")]
    private static extern void SphinxRun(int trialNumber, int recallNumber, int kws_threshold);


    [DllImport("SphinxPlugin")]
    private static extern IntPtr SphinxCheck();

    [DllImport("SphinxPlugin")]
    private static extern int PrintANumber();

    [DllImport("SphinxPlugin")]
    private static extern int AddTwoIntegers(int i1, int i2);

    [DllImport("SphinxPlugin")]
    private static extern float AddTwoFloats(float f1, float f2);

    [DllImport("SphinxPlugin")]
    private static extern int SetAudioPath(string someStr);

    [DllImport("SphinxPlugin")]
    private static extern IntPtr GetAudioPath();


    [DllImport("SphinxPlugin")]
    private static extern void AbortSphinxCheck();


    ThreadedSphinx _threadedSphinx;
    // Use this for initialization
    void Start()
    {
        _threadedSphinx = new ThreadedSphinx(); 
        UnityEngine.Debug.Log(PrintANumber());
        UnityEngine.Debug.Log(AddTwoIntegers(3, 5));
        UnityEngine.Debug.Log(AddTwoFloats(2f, 3f));


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BreakSphinx()
    {
        AbortSphinxCheck();
    }

    public void SetPath(string audioPath)
    {
        UnityEngine.Debug.Log("set sphinx audio path to " + audioPath);
        SetAudioPath(audioPath);
    }

    public void RunAudioCheck(int trialNumber, int recallNumber, string actualName, string kws_threshold)
    {

        UnityEngine.Debug.Log("running sphinx audio");
        UnityEngine.Debug.Log("running SPHINX audio " + actualName + " for " + kws_threshold);
        int threshInt = int.Parse(kws_threshold);
        //UnityEngine.Debug.Log("thres int is: " + threshInt.ToString());
        //SphinxRun(trialNumber, recallNumber, threshInt);
        if(_threadedSphinx!=null)
            _threadedSphinx.Start(); //start the sphinx
    }

    public int CheckAudioResponse()
    {
        UnityEngine.Debug.Log("checking sphinx response");
        //string response = Marshal.PtrToStringAuto(SphinxCheck());
        string response = _threadedSphinx.CheckResult();
        UnityEngine.Debug.Log(response);

        if (response == "found")
        {
            UnityEngine.Debug.Log("SPHINX FOUND");
            return 1;
        }
        else
        {
            UnityEngine.Debug.Log("SPHINX NOT ");
            return 0;
        }
    }




    //THREADED SERVER
    public class ThreadedSphinx : ThreadedJob
    {
        public bool isRunning = false;


        public ThreadedSphinx()
        {

        }

        protected override void ThreadFunction()
        {
            
            SphinxRun(0,0,20);
        }

        public string CheckResult()
        {
            UnityEngine.Debug.Log("checking result inside thread");
            string res = "not found";
            res = Marshal.PtrToStringAuto(SphinxCheck());
            return res;

        }


    }
}
