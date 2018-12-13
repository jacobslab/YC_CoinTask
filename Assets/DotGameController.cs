using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotGameController : MonoBehaviour {

    private CanvasGroup dotGamePanel;
    public RawImage dotImage;
    public CanvasGroup instrPanel;

    private float maxAlpha = 0.75f;

	void Start () {
        dotGamePanel = gameObject.GetComponent<CanvasGroup>();
        instrPanel.alpha = 0f;
        dotGamePanel.alpha = 0f;
        dotImage.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator RunGame()
    {
        dotImage.enabled = false;
        dotGamePanel.alpha = maxAlpha;

        instrPanel.alpha = 1f;
        //yield return StartCoroutine(Experiment_CoinTask.Instance.WaitForActionButton());
        //instrPanel.alpha = 0f;

        dotImage.enabled = true;
        //show dots in random locations with a random probability of red-white for 20 seconds

        float elapsedTime = 0f;

        for (int i = 0; i < 20; i++)
        {
            dotImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(380f, 845f), Random.Range(-454f, -224f));
        if (Random.value < 0.5f)
            dotImage.GetComponent<RawImage>().color = Color.red;
        else
            dotImage.GetComponent<RawImage>().color = Color.white;

        yield return new WaitForSeconds(1f);
        elapsedTime += 1f;
        }
        instrPanel.alpha = 0f;
        dotImage.enabled = false;
        dotGamePanel.alpha = 0f;
		yield return null;
	}
}

