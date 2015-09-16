using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UsefulFunctions {

	//given the size of an array or a list, will return a list of indices in a random order
	public static List<int> GetRandomIndexOrder(int count){
		List<int> inOrderList = new List<int>();
		for(int i = 0; i < count; i++){
			inOrderList.Add(i);
		}
		
		List<int> randomOrderList = new List<int>();
		for(int i = 0; i < count; i++){
			int randomIndex = Random.Range(0, inOrderList.Count);
			randomOrderList.Add( inOrderList[randomIndex] );
			inOrderList.RemoveAt(randomIndex);
		}
		
		return randomOrderList;
	}
}
