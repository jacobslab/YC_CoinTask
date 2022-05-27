using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRButton : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClickRR(){

        obj.transform.Rotate(0, 20*Time.deltaTime, 0);
    }
}
