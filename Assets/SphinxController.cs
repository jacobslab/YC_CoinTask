using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SphinxController : MonoBehaviour
{
    public SphinxTest sphinxTest;
    public AudioRecorder audioRecorder;
    private string audioPath = "";
    private int result = -1;

    public Text counterText;
    public Text resText;

    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {

        audioPath = "/Users/anshpatel/Desktop/sphinxAudio/";
        sphinxTest.SetPath(audioPath);
    }

    // Update is called once per frame
    void Update()
    {
     if(Input.GetKeyDown(KeyCode.R))
        {
            sphinxTest.RunAudioCheck(0, 0, "here", "20");
        }

        counter++;
        counterText.text = counter.ToString();

        if (Input.GetKeyDown(KeyCode.C))
        {
            result = sphinxTest.CheckAudioResponse();
            UnityEngine.Debug.Log("result is  " + result.ToString());
            if (result == 1)
            {
                resText.text = "SPHINX";
                resText.color = Color.green;
            }
            else
            {
                resText.text = "NOT SPHINX";
                resText.color = Color.red;
            }

        }
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    sphinxTest.BreakSphinx();
        //}


        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StartCoroutine(audioRecorder.Record(audioPath, "0_0.wav",5));
        //}

    }
}
