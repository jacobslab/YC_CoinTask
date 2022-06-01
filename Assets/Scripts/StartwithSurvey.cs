using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartwithSurvey : MonoBehaviour
{
    // Start is called before the first frame update
    static public bool isAlreadyInvoked = false;
    void Start()
    {
        if (isAlreadyInvoked == false) {
            Application.LoadLevel(16);
            isAlreadyInvoked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
