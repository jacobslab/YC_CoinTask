using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public GameObject plane;
    public Texture testImage;
    // Start is called before the first frame update
    void Start()
    {
        plane.GetComponent<Renderer>().material.mainTexture = testImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
