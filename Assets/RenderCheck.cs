using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCheck : MonoBehaviour
{
    private Renderer m_renderer;
    private Vector3 screenPos;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_renderer.isVisible)
        {
            screenPos = Camera.main.ViewportToScreenPoint(transform.position);
            Experiment_CoinTask.Instance.trialController.trialLogger.LogChestVisiblity(true,screenPos);
        }

        /*else
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Experiment_CoinTask.Instance.trialController.trialLogger.LogChestVisiblity(false, screenPos);
        }*/
    }
}
