﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
public class CommandExecution : MonoBehaviour {
  //  public Text trialText;
    private int testInt = 0;
    private string hmmPath= @"model\en-us\en-us";
    private string lmPath= @"model\en-us\en-us.lm.bin";
    private string dictPath = @"model\en-us\cmudict-en-us.dict";
    private string sphinxLogPath;
    private string keyphrase;
    private string appDataPath = "";
    private string resultFormat = "";
    public static string sphinxPath = @"C:\Users\JacobsLab\Desktop\Pocketsphinx\pocketsphinx\";
	//SINGLETON
	private static CommandExecution _instance;
	
	public static CommandExecution Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			UnityEngine.Debug.Log ("Instance already exists!");
			Destroy (transform.gameObject);
			return;
		}
		_instance = this;
		//ExecuteCommand(13,0,10);
        appDataPath = Application.dataPath + "/";
        UnityEngine.Debug.Log("app data path is: " + appDataPath);
	}

	// Use this for initialization
	void Start () {
      //  trialText.text = testInt.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A))
        {
           // ExecuteCommand(13,0,10);
        }
//        if(Input.GetKeyDown(KeyCode.D))
//        {
//            testInt++;
//            trialText.text = testInt.ToString();
//        }
	
	}
    
    public string ExecuteCommand(int trialNumber, int recallNumber, int thresInt)
    {
        var thread = new Thread(delegate () { Command(trialNumber,recallNumber,thresInt); });
        thread.Start();
        UnityEngine.Debug.Log("Starting thread");
        thread.Join();
        UnityEngine.Debug.Log("first thread finished executing");
        string contents = System.IO.File.ReadAllText(Config_CoinTask.audioPath + resultFormat + ".txt");
        if (contents.Contains("chocolate"))
        {
            return "found";
            UnityEngine.Debug.Log("Found chocolate in " + contents);
        }
        else
        {
            return "not found";
            UnityEngine.Debug.Log(" chocolate NOT FOUND");
        }

    }

    void Command(int trialNum, int recallNum, int thres)
    {
        //#if (UNITY_STANDALONE || UNITY_EDITOR)
        Process proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardOutput = false;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.FileName = @"powershell.exe";
        // proc.StartInfo.Arguments = "notepad.exe";
        proc.StartInfo.WorkingDirectory = Config_CoinTask.audioPath;
        resultFormat = trialNum.ToString() + "_" + recallNum.ToString();
        proc.StartInfo.Arguments = appDataPath + @"bin\Release\Win32\pocketsphinx_continuous.exe -infile " + Config_CoinTask.audioPath + resultFormat+".wav -hmm " +appDataPath + hmmPath +" -lm " + appDataPath + lmPath + " -dict " + appDataPath + dictPath +" -logfn " + Config_CoinTask.audioPath + "ok.log" +" -time yes -kws_threshold 1e-"+thres+ " > " + Config_CoinTask.audioPath+resultFormat+".txt";
        proc.Start();
     //   string result = proc.StandardOutput.ReadToEnd() + " or " + proc.StandardError.ReadToEnd();
       // UnityEngine.Debug.Log(result);
        proc.WaitForExit();
        proc.Close();
        UnityEngine.Debug.Log("process executed");
       

        /*
        var processInfo = new ProcessStartInfo("powershell.exe", "echo hi");
        
                //var processInfo = new ProcessStartInfo("powershell.exe", sphinxPath+@"bin\Release\Win32\pocketshinx_continuous.exe -inmic yes -hmm " + sphinxPath + @"model\en-us\en-us -lm "+sphinxPath+ @"\model\en-us\en-us.lm.bin -dict "+sphinxPath + @"model\en-us\cmudict-en-us.dict");
                //`#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
                //	var processInfo = new ProcessStartInfo("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", @"shutdown -s now");
                //#endif
                string filepath=System.IO.Directory.GetCurrentDirectory();
		UnityEngine.Debug.Log (filepath);
		//var processInfo = new ProcessStartInfo ("open","InputVolume.app");
		//processInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
		//processInfo.Arguments="-c \" " + "shutdown -s now" + " \"";
        processInfo.CreateNoWindow = true;
		processInfo.WorkingDirectory = filepath + "/";
		//processInfo.RedirectStandardOutput = False;
     //   processInfo.UseShellExecute = true;

        var process = Process.Start(processInfo);
        
        process.WaitForExit();
        process.Close();
        */
    }
}
