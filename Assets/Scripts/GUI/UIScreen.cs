using UnityEngine;
using System.Collections;

public class UIScreen : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 
	
	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Play(){
		Enable (true);
	}
	
	public void Stop(){
		Enable (false);
	}
	
	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);
		
		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
