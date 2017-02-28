using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using AsyncIO;

public class NMQTest : MonoBehaviour
{
	//private NetMQContext m_context;
	private PublisherSocket m_publisher;

	// Use this for initialization
	void Start()
	{
		ForceDotNet.Force();

		//	m_context = NetMQContext.Create();
		m_publisher=new PublisherSocket();
		m_publisher.Bind ("tcp://"+TCP_Config.HostIPAddress+":"+TCP_Config.ConnectionPort);
//		m_publisher.SendFrame ("A");


		//		m_pull = new PullSocket();
		//		m_pull.Options.Linger = System.TimeSpan.Zero;
		//		m_pull.Connect("tcp://127.0.0.1:8888");

	}

	// Update is called once per frame
	void Update()
	{
		m_publisher.SendFrame ("{\"data\":\"\",\"type\":\"CONNECTED\",\"time\":1488297347602}");
	}

	void OnApplicationQuit()
	{
		m_publisher.Dispose ();
		//		m_pull.Dispose();
		Debug.Log("Exit");

	}
}