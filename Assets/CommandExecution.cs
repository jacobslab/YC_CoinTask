using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
public class CommandExecution : MonoBehaviour
{
    //  public Text trialText;
    private int testInt = 0;

    //SINGLETON
    private static CommandExecution _instance;
    private static string dataPath;
    public static CommandExecution Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {

        if (_instance != null)
        {
            UnityEngine.Debug.Log("Instance already exists!");
            Destroy(transform.gameObject);
            return;
        }
        _instance = this;
        dataPath = Application.dataPath;
   
    }

    // Use this for initialization
    void Start()
    {
        //  trialText.text = testInt.ToString();
    }

    // Update is called once per frame
    void Update()
    {
      
        //        if(Input.GetKeyDown(KeyCode.D))
        //        {
        //            testInt++;
        //            trialText.text = testInt.ToString();
        //        }

    }
    public static void ExecuteCommand(string device_sn,string mode)
    {
        var thread = new Thread(delegate () { Command(device_sn,mode); });
        thread.Start();
        UnityEngine.Debug.Log("Starting thread");
    }

    static void Command(string device_sn, string mode)
    {
        var proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardOutput = false;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.FileName = @"powershell.exe";
        proc.StartInfo.Arguments = @"C:\Users\exp\Downloads\Tobii.Pro.Eye.Tracker.Manager.Windows-1.4.0.exe --device_sn="+device_sn+" --mode="+mode;
        //`#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
        //	var processInfo = new ProcessStartInfo("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", @"shutdown -s now");
        //#endif
        //		string filepath=System.IO.Directory.GetCurrentDirectory();
        //		UnityEngine.Debug.Log (filepath);
        //		var processInfo = new ProcessStartInfo ("open",dataPath+"/InputVolume.app");
        //processInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
        //processInfo.Arguments="-c \" " + "shutdown -s now" + " \"";
        //processInfo.CreateNoWindow = true;
        //processInfo.WorkingDirectory = filepath + "/";
        //processInfo.RedirectStandardOutput = False;
        // processInfo.UseShellExecute = false;
        //
        proc.Start();
        //
                proc.WaitForExit();
                proc.Close();
    }
}