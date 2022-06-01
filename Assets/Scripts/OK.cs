using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OK : MonoBehaviour
{
    public GameObject okbutton;
    static public bool okenabled = false;

    // Start is called before the first frame update
    void Start()
    {
        okbutton.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (okenabled)
        {
            okbutton.GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else {
            okbutton.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
