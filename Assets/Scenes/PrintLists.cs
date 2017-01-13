using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintLists : MonoBehaviour {

	Object[] prefabs;
	List<GameObject> objList;
	string allNames;
	// Use this for initialization
	void Start () {
		objList = new List<GameObject> ();

		prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			objList.Add((GameObject)prefabs[i]);
		}

		for (int i = 0; i < objList.Count; i++) {
			allNames += objList [i].gameObject.name + "\n";
		}

		System.IO.File.WriteAllText("/Users/anshpatel/Desktop/updatedNames.txt",allNames);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
