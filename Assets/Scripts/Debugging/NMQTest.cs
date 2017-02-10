using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;

public class NMQTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		using (var server = new ResponseSocket("@tcp://127.0.0.1:5556"))
		using (var client = new RequestSocket(">tcp://127.0.0.1:5556"))
		{
			// client sends message consisting of two frames
			UnityEngine.Debug.Log("Client sending");
			client.SendMoreFrame("A").SendFrame("Hello");

			// server receives frames
			bool more = true;
			while (more)
			{
				string frame = server.ReceiveFrameString(out more);
				UnityEngine.Debug.Log("Server received frame" + frame + " more " + more);
			}

			UnityEngine.Debug.Log("================================");

			// server sends message, this time using NetMqMessage
			var msg = new NetMQMessage();
			msg.Append("From");
			msg.Append("Server");

			UnityEngine.Debug.Log("Server sending");
			server.SendMultipartMessage(msg);

			// client receives the message
			msg = client.ReceiveMultipartMessage();
			UnityEngine.Debug.Log("Client received {0} frames" + msg.FrameCount);

			foreach (var frame in msg)
				UnityEngine.Debug.Log("Frame={0}"+ frame.ConvertToString());

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
