using UnityEngine;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
public class CommandExecution : MonoBehaviour {
	//  public Text trialText;
	private int testInt = 0;
	private string hmmPath= @"model/de/cmusphinx-de-voxforge-5.2";
	private string dictPath = @"model/de/cmusphinx-voxforge-de-1.dic";
	private string sphinxLogPath;
	private string keyphrase;
	private string appDataPath = "";
	private string resultFormat = "";
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
		appDataPath = Application.dataPath + "/";
		UnityEngine.Debug.Log("app data path is: " + appDataPath);
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {


	}

	public string ExecuteSphinx(int trialNumber, int recallNumber, string actualName, int thresInt)
	{
		var thread = new Thread(delegate () { CommandSphinx(trialNumber,recallNumber,actualName,thresInt); });
		thread.Start();
		UnityEngine.Debug.Log("Starting thread");
		thread.Join();
		UnityEngine.Debug.Log("first thread finished executing");
		if (File.Exists(Config_CoinTask.audioPath + resultFormat + ".txt"))
		{
			string contents = System.IO.File.ReadAllText(Config_CoinTask.audioPath + resultFormat + ".txt");
			if (contents.Contains(actualName))
			{

				UnityEngine.Debug.Log("Found " + actualName + " contents");
				return "found";
			}
			else
			{

				UnityEngine.Debug.Log(actualName + " NOT FOUND");
				return "not found";
			}
		}
		else
		{
			UnityEngine.Debug.Log("COULD NOT FIND FILE");
			return "not found";
		}

	}

	void CommandSphinx(int trialNum, int recallNum, string actualName, int thres)
	{
		//#if (UNITY_STANDALONE || UNITY_EDITOR)
		Process proc = new Process();
		proc.StartInfo.CreateNoWindow = true;
		proc.StartInfo.RedirectStandardOutput = false;
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.FileName = @"powershell.exe";
		// proc.StartInfo.Arguments = "notepad.exe";
		#if UNITY_EDITOR_WIN
		appDataPath=@"C:\Users\jacobslab\Desktop\TreasureHunt\";
		proc.StartInfo.WorkingDirectory = appDataPath;
		resultFormat = trialNum.ToString() + "_" + recallNum.ToString();
		UnityEngine.Debug.Log("working directory is: " + appDataPath);
		UnityEngine.Debug.Log("audio path is: " + Config_CoinTask.audioPath);
		UnityEngine.Debug.Log("result format is: " + resultFormat);
		proc.StartInfo.Arguments = appDataPath + @"bin/Release/Win32/pocketsphinx_continuous.exe -infile " + Config_CoinTask.audioPath + resultFormat + ".wav -hmm " + appDataPath + hmmPath + " -dict " + appDataPath + dictPath + " -logfn " + Config_CoinTask.audioPath + "ok.log" + " -time yes -kws_threshold 1e-" + thres + " -keyphrase " + actualName + " > " + Config_CoinTask.audioPath + resultFormat + ".txt";

		#else

		proc.StartInfo.WorkingDirectory = appDataPath;
		resultFormat = trialNum.ToString() + "_" + recallNum.ToString();
		UnityEngine.Debug.Log("working directory is: " + appDataPath);
		UnityEngine.Debug.Log("audio path is: " + Config_CoinTask.audioPath);
		UnityEngine.Debug.Log("result format is: " + resultFormat);
		proc.StartInfo.Arguments = appDataPath + @"bin\Release\Win32\pocketsphinx_continuous.exe -infile " + Config_CoinTask.audioPath + resultFormat+".wav -hmm " +appDataPath + hmmPath +" -dict " + appDataPath + dictPath +" -logfn " + Config_CoinTask.audioPath + "ok.log" +" -time yes -kws_threshold 1e-"+thres+ " -keyphrase " + actualName + " > " +  Config_CoinTask.audioPath + resultFormat + ".txt";
		#endif        
		proc.Start();
		//   string result = proc.StandardOutput.ReadToEnd() + " or " + proc.StandardError.ReadToEnd();
		// UnityEngine.Debug.Log(result);
		UnityEngine.Debug.Log("should have printed something at : " + Config_CoinTask.audioPath + resultFormat + ".txt");
		proc.WaitForExit();
		proc.Close();
		UnityEngine.Debug.Log("process executed");

	}
    public static void ExecuteTobiiEyetracker(string device_sn,string mode,string filepath)
    {
        var thread = new Thread(delegate () { CommandTobii(device_sn,mode,filepath); });
        thread.Start();
        UnityEngine.Debug.Log("Starting thread");
    }

    static void CommandTobii(string device_sn, string mode,string filepath)
    {
        UnityEngine.Debug.Log("executing command");
        var proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardOutput = false;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.FileName = @"powershell.exe";
#if UNITY_EDITOR_WIN
        proc.StartInfo.Arguments = filepath+ @"/Tobii.Pro.Eye.Tracker.Manager.Windows-AMD64-1.5.2.exe --device-sn=" + device_sn+" --mode="+mode;
        UnityEngine.Debug.Log("datapath is: " + filepath + @"/Tobii.Pro.Eye.Tracker.Manager.Windows-AMD64-1.5.2.exe");
#else
       proc.StartInfo.Arguments = filepath+@"/Tobii.Pro.Eye.Tracker.Manager.Windows-AMD64-1.5.2.exe --device-sn="+device_sn+" --mode="+mode;
        UnityEngine.Debug.Log("datapath is: " + filepath + @"/Tobii.Pro.Eye.Tracker.Manager.Windows-AMD64-1.5.2.exe");
#endif
        proc.Start();
        //
                proc.WaitForExit();
                proc.Close();
    }
}