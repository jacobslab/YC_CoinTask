using UnityEngine;
using System.Collections;

public class BlockCompleteUI : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 


	public ParticleSystem funParticles;

	public TextMesh[] BlockScores;
	public TextMesh BlockText; // ie: 3/4 complete

	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public IEnumerator Play(int currentBlockIndex, int maxNumBlocks){
		Enable (true);

		if (funParticles != null) {
			funParticles.Stop ();
			funParticles.Play ();
		}



//		if (currentBlockIndex < BlockScores.Length) {
//			BlockScores[currentBlockIndex].text = currentBlockScore.ToString();
//		} else {
//			Debug.Log("NOT ENOUGH BLOCK SCORE TEXTMESHES");
//		}

		BlockText.text = exp.currInstructions.blockLower + " " + (currentBlockIndex + 1) + "/" + maxNumBlocks + " " + exp.currInstructions.completed;
		yield return null;
	}

	public IEnumerator RedeemTrophies(int blockIndex,int currentBlockScore)
	{
		//redeem trophies
		yield return StartCoroutine(exp.scoreController.RedeemTrophies(blockIndex,currentBlockScore));
		yield return null;
	}

	public void Stop(){
		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);
		
		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
