using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using UnityEditor.Experimental.Build.AssetBundle;
using UnityEngine;

public class CommandReceiver : MonoBehaviour
{

	public UnityEngine.UI.Text MessageText;
	public UnityEngine.UI.Text ServerText; 
	
	private ConnectionService _service;
	private ICommandScheduler _scheduler;
	private UdpBroadcastCommand _broadcastCommand;
	
	// Use this for initialization
	void Start ()
	{
		_scheduler = new CommandScheduler();
		_service = new ConnectionService();
		_service.Address = _service.GetLocalIPAddress();
		_service.Port = 8080;
		_service.MessageReceived += UpdateMessageText;
		_service.StartServer();
		ServerText.text = string.Format("Server running on {0}", _service.URL);
		MessageText.text = "Not received";
		_broadcastCommand = new UdpBroadcastCommand(8080, _service.URL);
		_scheduler.AddCommand(_broadcastCommand);
	}
	
	private void UpdateMessageText(string message)
	{
		MessageText.text = message;
	}

	private void OnDestroy()
	{
		_service.StopServer();
		_scheduler.RemoveCommand(_broadcastCommand);
		_service = null;
	}

	void Update()
	{
		_service.Tick();
	}
}
