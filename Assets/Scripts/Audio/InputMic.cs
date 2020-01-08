﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMic : MonoBehaviour
{

    public static float MicLoudness;
    public Dropdown micDrops;
    private float maxLoud = 0f;
    private string _device;
    public bl_ProgressBar volProg;
    private bool cannotHear = true;
    public Text spokenWord;
    public string[] wordList;
    private int currentWord = 0;
    public Image marker;
    public Text beginExperimentText;
    private List<string> micList = new List<string>();
    //mic initialization
    public CanvasGroup micCanvasGroup;
    AudioSource recAudio;
    public AudioRecorder recorder;
    private bool samsonFound = false;
    public CanvasGroup micTestGroup;
    private string samsonDeviceName = "";
    AudioClip _clipRecord;
    private int samsonIndex = 0;
    public int samplerate = 44100;

    public CanvasGroup micButtonGroup;
    private bool micReady = false;

    void Awake()
    {
        recAudio = GetComponent<AudioSource>();
        micCanvasGroup.alpha = 0f;
        micTestGroup.alpha = 0f;
    }
    void Start()
    {

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log("length of mics is" + Microphone.devices.Length + "  and  " + Microphone.devices[i].ToString());
            micList.Add(Microphone.devices[i].ToString());
#if !DEMO
            //if (Microphone.devices[i].ToString().Contains("Samson"))
            //{
            //    samsonIndex = i;
            //    samsonDeviceName = Microphone.devices[samsonIndex].ToString();
            //    samsonFound = true;

            //}
#endif
        }
        beginExperimentText.enabled = false;
        micDrops.AddOptions(micList);
    }
    IEnumerator RotateWords()
    {
        spokenWord.text = wordList[0];
        float timer = 0f;
        while (cannotHear)
        {
            _clipRecord = AudioClip.Create("clipRecord",samplerate * 2, 1,samplerate,true);
            //            	yield return StartCoroutine (Experiment.Instance.audioRecorder.Record(Experiment.Instance.SessionDirectory, "micTest.wav", 5));
            if (_device == null && Microphone.devices.Length > 0)
            {
                _device = Microphone.devices[micDrops.value];
                UnityEngine.Debug.Log("setting as " + Microphone.devices[micDrops.value].ToString());
                _clipRecord = Microphone.Start(_device, true, 5, 44100);
            }
            else
            {
                spokenWord.text = "No microphone detected!";
            }
            UnityEngine.Debug.Log("device is " + _device.ToString());
            spokenWord.color = Color.red;
            spokenWord.color = Color.white;
            timer = 0f;
            currentWord++;
            spokenWord.text = wordList[currentWord];
            yield return new WaitForSeconds(5f);
            Microphone.End(_device);
            _device = null;
            recAudio.PlayOneShot(_clipRecord);
            spokenWord.text = "Playing back recorded audio...";
            yield return new WaitForSeconds(5f);

            bool givenResponse = false;

            spokenWord.text = "Proceed (X) \n Retry (A)";
            while (!givenResponse)
            {
                if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0))
                { // x button
                    cannotHear = false;
                    givenResponse = true;
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton1))
                { // a button
                    cannotHear = true;
                    givenResponse = true;
                }
                yield return 0;
            }


            yield return 0;
        }
        yield return null;
    }

    public void SetMicReady()
    {
        micReady = true;
    }

    IEnumerator WaitForReadyMicTest()
    {
        micButtonGroup.alpha = 1f;
        while(!micReady)
        {
            yield return 0;
        }
        micButtonGroup.alpha = 0f;
        yield return null;
    }

    public IEnumerator RunMicTest()
    {
        //        InitMic();

        micCanvasGroup.alpha = 1f;
        yield return StartCoroutine(WaitForReadyMicTest());

        micTestGroup.alpha = 1f;
        yield return StartCoroutine("RotateWords");
        yield return new WaitForSeconds(1.5f);
        micCanvasGroup.alpha = 0f;
        StopMicrophone();
        yield return null;

    }

    void StopMicrophone()
    {
        Microphone.End(_device);
    }


    int _sampleWindow = 128;

    //    //get data from microphone into audioclip
    float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(samsonDeviceName) - (_sampleWindow + 1); // null means the first microphone
        if (micPosition < 0) return 0;
        if (_clipRecord != null)
        {
            _clipRecord.GetData(waveData, micPosition);
            // Getting a peak on the last 128 samples
            for (int i = 0; i < _sampleWindow; i++)
            {
                float wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
        }
        return levelMax;
    }



    void Update()
    {

        //		if (MicLoudness > Config.micLoudnessThreshold)
        //		{
        //			cannotHear = false;
        //			marker.color = Color.green;
        //		}

        if (Input.GetKeyDown(KeyCode.L))
            cannotHear = false;
        // levelMax equals to the highest normalized value power 2, a small number because < 1
        // pass the value to a static var so we can access it from anywhere
        MicLoudness = LevelMax();
        // Debug.Log(MicLoudness);
        if (maxLoud < MicLoudness)
            maxLoud = MicLoudness;
        if (cannotHear)
            volProg.Value = MicLoudness;
        //        else {
        //            beginExperimentText.enabled = true;
        //            spokenWord.color = Color.green;
        //            spokenWord.text = "I heard you say " + wordList[currentWord];
        //        }
    }

    bool _isInitialized;
    // start mic when scene starts
    void OnEnable()
    {
        //        InitMic();

        _isInitialized = true;
    }

    //stop mic when loading a new level or quit application
    void OnDisable()
    {
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }


    // make sure the mic gets started & stopped when application gets focused
    //    void OnApplicationFocus(bool focus)
    //    {
    //        if (focus)
    //        {
    //            //Debug.Log("Focus");
    //
    //            if (!_isInitialized)
    //            {
    //                //Debug.Log("Init Mic");
    //                InitMic();
    //                _isInitialized = true;
    //            }
    //        }
    //        if (!focus)
    //        {
    //            //Debug.Log("Pause");
    //            StopMicrophone();
    //            //Debug.Log("Stop Mic");
    //            _isInitialized = false;
    //
    //        }
    //    }
}