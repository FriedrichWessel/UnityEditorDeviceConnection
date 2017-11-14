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
		window.Setup();
		window.Show();
	}
 
	private void Setup()
	{
		_service = new ConnectionService();
		_service.ServerDataReceived += UpdateServerData; 
		_service.StartReceiveBroadcast();
	}

	private ConnectionService _service;

	private string _command;
	void OnGUI()
	{
		if (_service == null) // react on re-compile
		{
			Setup();
		}
		if (!_service.IsConnected())
		{
			GUILayout.Label("Waiting for Server....");
		}
		else
		{
			if (GUILayout.Button("StopServer"))
			{
				_service.Disconnect();
				Setup();
			}
			
			_command = GUILayout.TextField(_command);
			if (GUILayout.Button("Send"))
			{
				_service.Send(_command);
			}
		}
			
	}

	private void UpdateServerData(ServerData serverData)
	{
		_service.Address = serverData.IpAddress;
		_service.Port = serverData.Port;
		_service.ConnectToServer();
	}

	
}