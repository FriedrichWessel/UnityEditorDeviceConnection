using System;
using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using UnityEngine;

public class LittleGuyExampleReceiver : MonoBehaviour
{
	[SerializeField] private LittleGuyView View; 
	
	private EditorConnectionServer _server;

	// Use this for initialization
	void Start ()
	{
		_server = new EditorConnectionServer(8080, 15000);
		_server.DataReceived += SendCommandToLittleGuy;
		_server.StartServer();
	}

	private void SendCommandToLittleGuy(string command)
	{
		switch (command)
		{
			case "Walk": 
				View.StartWalkAnimation();
				break;
			case "Idle": 
				View.StartIdleAnimation();
				break;
			default:
				Debug.Log(string.Format("Command {0} not available", command));
				break;
				
		}
	}

	private void OnDestroy()
	{
		_server.StopServer();
	}

	void Update () {
		_server.Tick();
	}
}
