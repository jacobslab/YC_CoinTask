using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;

public class NMQTest : MonoBehaviour {

	string serverAppend="@tcp://";
	string port=":5556";

	string HostIPAddress="127.0.0.1";
	string clientAppend=">tcp://";
	// Use this for initialization
	void Start () {
		UnityEngine.Debug.Log (serverAppend + HostIPAddress + port);
		UnityEngine.Debug.Log (clientAppend + HostIPAddress + port);
		using (var server = new ResponseSocket(serverAppend+HostIPAddress+port))
		using (var client = new RequestSocket(clientAppend+HostIPAddress+port))
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
