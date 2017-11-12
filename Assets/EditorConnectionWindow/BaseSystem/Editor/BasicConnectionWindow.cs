using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class BasicConnectionWindow : EditorWindow
{
	// Add menu named "My Window" to the Window menu
	[MenuItem("Tools/ConnectionWindow")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		var window = (BasicConnectionWindow) EditorWindow.GetWindow(typeof(BasicConnectionWindow));
		
		window.Show();
	}

	private ConnectionService _service;

	private string _command;
	private string _address = GetLocalIPAddress();
	private string _port = "8080";
	void OnGUI()
	{
		if (_service != null)
		{
			if (GUILayout.Button("StopServer"))
			{
				_service.Disconnect();
				_service = null;
			}
			_command = GUILayout.TextField(_command);
			if (GUILayout.Button("Send"))
			{
				_service.Send(_command);
			}
		}
		else
		{
			if (GUILayout.Button("Scan For Server"))
			{
				var url = ConnectionService.ReceiveBroadCast();
				if (!string.IsNullOrEmpty(url))
				{
					var data = url.Split(':');
					var adress = data[0];
					int port = Convert.ToInt32(data[1]);
					_service = new ConnectionService();
					_service.Address = adress;
					_service.Port = port;
					_service.ConnectToServer();
				}
			}
			
			_address = GUILayout.TextField(_address);
			_port = GUILayout.TextField(_port);
			if (GUILayout.Button("Start Client"))
			{
				_service = new ConnectionService();
				_service.Address = _address;
				_service.Port = Convert.ToInt32(_port);
				_service.ConnectToServer();
				
			}
			
		}
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