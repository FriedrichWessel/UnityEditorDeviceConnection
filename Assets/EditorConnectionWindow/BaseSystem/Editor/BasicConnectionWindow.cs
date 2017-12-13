﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using EditorConnectionWindow.BaseSystem.TimeProvider;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

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

	private UnityTimeProvider _timeProvider;
	private ServerList _serverList;
	
	private List<string> _popupServerNames = new List<string>();
	private int _currentServerIndex;
	private IConnectionClient _connectionClient;

	private void Setup()
	{
		_timeProvider = new UnityTimeProvider();
		_serverList = new ServerList(_timeProvider);
		_serverList.ServerAdded += AddServerToDropDown;
		_serverList.ServerRemoved += RemoveServerFromDropDown;
		_serverList.RemoveSelectedServer += DisconnectFromSelectedServer;
		_serverList.StartListingForServers(15099);

		var client = new TcpClient();
		_connectionClient = new TcpConnectionClient(client);

	}

	private void DisconnectFromSelectedServer(ServerData serverData)
	{
		
	}

	private void RemoveServerFromDropDown(ServerData serverData)
	{
		_popupServerNames.Remove(serverData.IpAddress);
	}

	private void AddServerToDropDown(ServerData serverData)
	{
		_popupServerNames.Add(serverData.IpAddress);
	}

	void OnGUI()
	{
		if (_serverList == null) // react on recompile
		{
			return;
		}
		if (_popupServerNames.Count == 0)
		{
			return;
		}
		
		int index = _currentServerIndex;
		index = EditorGUILayout.Popup(_currentServerIndex, _popupServerNames.ToArray());
		if (index != _currentServerIndex && index > 0)
		{
			_serverList.SelectServerFromList(_serverList.AvailableServers[index]);
			_currentServerIndex = index;
			
		}
		
	}
	/*private void OnDestroy()
	{
		if (_service != null)
		{
			_service.Disconnect();
			//_service.StopReceiveBroadcast();
		}
	}

	

	void OnGUI()
	{
		UpdateWindow();
		if (_service == null || _popupServerNames == null) // react on re-compile
		{
			return;
			
		}
		
		if(_popupServerNames.Length > 0)
		{
			int index = _currentServerIndex;
			index = EditorGUILayout.Popup(_currentServerIndex, _popupServerNames);
			if (index != _currentServerIndex && index > 0)
			{
				var server = _availableServer[index-1];
				ConnectToServer(server.ServerData);
			}
			if (_connectedServer != null)
			{
				_currentServerIndex = index;
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
			_currentServerIndex = index;
		}
		

	}

	

	// since there is no relialble Update source for EditorWindows - we take anyshot we can get
	private void OnInspectorUpdate()
	{
		UpdateWindow();
	}
	
	// since there is no relialble Update source for EditorWindows - we take anyshot we can get
	private void Update()
	{
		UpdateWindow();
	}

	private void UpdateWindow()
	{
		if (_service == null || _popupServerNames == null)
		{
			Setup();
		}
		var timeSinceLastServerListUpdate = Time.realtimeSinceStartup - _lastUpdateTime;
		if (timeSinceLastServerListUpdate > 1)
		{
			UpdateAvailableServers(timeSinceLastServerListUpdate);
			_lastUpdateTime = Time.realtimeSinceStartup;
		}
		// realtime can only be used in Main.Thread
		// so we store the last seen for the tcp callbacks
		_currentTimeStamp = Time.realtimeSinceStartup;
	}
	
	private void UpdateAvailableServers(float deltaTime)
	{
		bool removedServer = false; 
		for (int i = _availableServer.Count-1; i >= 0; i--)
		{
			if (Time.realtimeSinceStartup - _availableServer[i].LastConnectionTime > 10)
			{
				if (_connectedServer == _availableServer[i].ServerData)
				{
					_service.Disconnect();
					_connectedServer = null;
					_currentServerIndex = 0;
				}
				_availableServer.RemoveAt(i);
				i--;
				removedServer = true;
			}
		}

		if (removedServer)
		{
			UpdateServerNameList();
		}
	}
	private void UpdateServerData(ServerData serverData)
	{
		bool serverIsRegistered = false; 
		foreach (var server in _availableServer)
		{
			if (server.ServerData.IpAddress == serverData.IpAddress)
			{
				server.LastConnectionTime = _currentTimeStamp;
				serverIsRegistered = true;
			}
		}
		if (!serverIsRegistered)
		{ 
			_availableServer.Add(new AvailableServerData(serverData));
			UpdateServerNameList();
		}
	}

	private void UpdateServerNameList()
	{
		if (_availableServer.Count > _popupServerNames.Length -1)
		{
			_popupServerNames = new string[_popupServerNames.Length+10];
		}
		_popupServerNames[0] = "None";
		
		for (int i= 0; i < _popupServerNames.Length-1; i++)
		{
			string name = null;
			if (i < _availableServer.Count)
			{
				name = _availableServer[i].ServerData.IpAddress;
			}
			_popupServerNames[i+1] = name;
		}
	}

	private void ConnectToServer(ServerData serverData)
	{
		if (_service.IsConnected())
		{
			_service.Disconnect();
		}
		_service.Address = serverData.IpAddress;
		_service.Port = serverData.Port;
		_service.ConnectToServer();
		_connectedServer = serverData;
	}*/
	
	


}