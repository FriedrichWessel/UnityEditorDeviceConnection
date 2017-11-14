using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class CommandReceiver : MonoBehaviour
{

	public UnityEngine.UI.Text MessageText;
	public UnityEngine.UI.Text ServerText; 
	
	private ConnectionService _service; 
	// Use this for initialization
	void Start () {
		_service = new ConnectionService();
		_service.Address = GetLocalIPAddress();
		_service.Port = 8080;
		_service.MessageReceived += UpdateMessageText;
		_service.StartServer();
		ServerText.text = string.Format("Server running on {0}", _service.URL);
		MessageText.text = "Not received";
		_service.StartBroadcastData(_service.URL);
	}
	
	private void UpdateMessageText(string message)
	{
		MessageText.text = message;
	}

	private void OnDestroy()
	{
		_service.StopServer();
		_service.StopBroadcast();
		_service = null;
	}

	void Update()
	{
		_service.Tick();
	}

	public static string GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}
