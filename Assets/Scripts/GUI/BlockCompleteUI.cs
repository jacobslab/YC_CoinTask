using UnityEngine;
using System.Collections;

public class BlockCompleteUI : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh[] BlockScores;
	public TextMesh BlockText; // ie: 3/4 complete

	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(int currentBlockIndex, int currentBlockScore, int maxNumBlocks){
		Enable (true);
		if (currentBlockIndex < BlockScores.Length) {
			BlockScores[currentBlockIndex].text = currentBlockScore.ToString();
		} else {
			Debug.Log("NOT ENOUGH BLOCK SCORE TEXTMESHES");
		}

		BlockText.text = "block " + (currentBlockIndex + 1) + "/" + maxNumBlocks + " completed";
	}

	public void Stop(){
		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);
		
		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
