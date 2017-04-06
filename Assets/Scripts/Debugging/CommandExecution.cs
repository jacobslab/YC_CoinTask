using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
public class CommandExecution : MonoBehaviour {
  //  public Text trialText;
    private int testInt = 0;
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
		ExecuteCommand();
	}

	// Use this for initialization
	void Start () {
      //  trialText.text = testInt.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ExecuteCommand();
        }
//        if(Input.GetKeyDown(KeyCode.D))
//        {
//            testInt++;
//            trialText.text = testInt.ToString();
//        }
	
	}
    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
        UnityEngine.Debug.Log("Starting thread");
    }

    static void Command()
    {
        //#if (UNITY_STANDALONE || UNITY_EDITOR)
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
    }
}
