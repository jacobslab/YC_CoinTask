using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PirateController : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public PirateUI WelcomingPirate;
	public PirateUI EncouragingPirate;

	public IEnumerator PlayWelcomingPirate(){
		yield return StartCoroutine(WelcomingPirate.Play (PirateUI.PirateTextType.start, true));
	}

	public IEnumerator PlayEncouragingPirate(){
		yield return StartCoroutine(EncouragingPirate.Play(PirateUI.PirateTextType.encouraging, false));
	}

}
