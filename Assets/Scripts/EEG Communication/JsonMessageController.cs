﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LitJson;
using System;
using System.Text;

using System.Text.RegularExpressions;

public class JsonMessageController : MonoBehaviour {

	public class Person
	{
		// C# 3.0 auto-implemented properties
		public string   Name     { get; set; }
		public int      Age      { get; set; }
		public DateTime Birthday { get; set; }
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public class MessageEvent{
		public string data { get; set; }
		public string type { get; set; }
		public long time { get; set; }
	}

	//Here's a line of JSON for you:
	//{"data": {"name": "ORIENT", "value": true}, "type": "STATE", "time": 1452273876684}

	public static string FormatSimpleJSONEvent(long time, string eventType, string eventData){
		MessageEvent mEvent = new MessageEvent();
		mEvent.data = eventData;
		mEvent.type = eventType;
		mEvent.time = time;
		
		string jsonEventString = JsonMapper.ToJson (mEvent);

		return jsonEventString;
	}

	//TODO: REFACTOR? A LOT OF DUPLICATE CODE HERE.
	public class MessageEventFloatData{
		public float data { get; set; }
		public string type { get; set; }
		public long time { get; set; }
	}

	//TODO: REFACTOR? A LOT OF DUPLICATE CODE HERE.
	public static string FormatSimpleJSONEvent(long time, string eventType, float eventData){
		MessageEventFloatData mEvent = new MessageEventFloatData();
		mEvent.data = eventData;
		mEvent.type = eventType;
		mEvent.time = time;
		
		string jsonEventString = JsonMapper.ToJson (mEvent);
		
		return jsonEventString;
	}

	public static string FormatJSONSessionEvent(long time, string sessionNum, string sessionType){
		StringBuilder sb = new StringBuilder();
		JsonWriter writer = new JsonWriter(sb);

		writer.WriteObjectStart();
		writer.WritePropertyName("data");

		//data object content
		writer.WriteObjectStart();

		writer.WritePropertyName("session_number");
		writer.Write(sessionNum);

		writer.WritePropertyName("session_type");
		writer.Write(sessionType);

		writer.WriteObjectEnd();


		//type
		writer.WritePropertyName ("type");
		writer.Write ("SESSION");

		//time
		writer.WritePropertyName ("time");
		writer.Write (time);


		writer.WriteObjectEnd ();

		return sb.ToString ();
	}

	public static string FormatJSONDefineEvent(long time, List<string> stateList){
		StringBuilder sb = new StringBuilder();
		JsonWriter writer = new JsonWriter(sb);
		
		writer.WriteObjectStart();
		writer.WritePropertyName("data");
		
		//data object content
		writer.WriteArrayStart();
		
		for (int i = 0; i < stateList.Count; i++) {
			writer.Write(stateList[i]);
		}
		
		writer.WriteArrayEnd();
		
		
		//type
		writer.WritePropertyName ("type");
		writer.Write ("DEFINE");
		
		//time
		writer.WritePropertyName ("time");
		writer.Write (time);
		
		
		writer.WriteObjectEnd ();
		
		return sb.ToString ();
	}

	public static string FormatJSONStateEvent(long time, string stateName, string boolValue){
		StringBuilder sb = new StringBuilder();
		JsonWriter writer = new JsonWriter(sb);
		
		writer.WriteObjectStart();
		writer.WritePropertyName("data");
		
		//data object content
		writer.WriteObjectStart();
		
		writer.WritePropertyName("statename");
		writer.Write(stateName);
		
		writer.WritePropertyName("value");
		writer.Write(boolValue);
		
		writer.WriteObjectEnd();
		
		
		//type
		writer.WritePropertyName ("type");
		writer.Write ("STATE");
		
		//time
		writer.WritePropertyName ("time");
		writer.Write (time);
		
		
		writer.WriteObjectEnd ();
		
		return sb.ToString ();

	}
}
