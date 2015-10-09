using UnityEngine;
using System.Collections;

public class UnitCompleteUI : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh[] UnitScores;
	public TextMesh UnitText; // ie: 3/4 complete

	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(int currentUnitIndex, int currentUnitScore, int maxNumUnits){
		Enable (true);
		if (currentUnitIndex < UnitScores.Length) {
			UnitScores[currentUnitIndex].text = currentUnitScore.ToString();
		} else {
			Debug.Log("NOT ENOUGH UNIT SCORE TEXTMESHES");
		}

		UnitText.text = "unit " + (currentUnitIndex + 1) + "/" + maxNumUnits + " completed";
	}

	public void Stop(){
		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);
		
		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
