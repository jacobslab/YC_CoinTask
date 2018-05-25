using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MathOperations : MonoBehaviour {

	public List<double> vals;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//		float m = Mean (vals, 0, vals.Count);
//		Debug.Log ("mean is " + m.ToString ());
//
//		float variance = Variance (vals, m, 0, vals.Count);
//		Debug.Log ("variance is: " + variance.ToString ());
	}

	public float Mean(List<float> values, int start, int end)
	{
		float s = 0;

		for (int i = start; i < end; i++)
		{
			s += values[i];
		}

		return s / (end - start);
	}

	public float Variance(List<float> values, float mean, int start, int end)
	{
		float variance = 0;

		for (int i = start; i < end; i++)
		{
			variance += (float)Math.Pow((values[i] - mean), 2);
		}

		int n = end - start;
		if (start > 0) n -= 1;

		return variance / (n);
	}
}
