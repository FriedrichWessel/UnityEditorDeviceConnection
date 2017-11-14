using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
 
	private ConnectionService _service;
	private Dictionary<ServerData, float> _availableServer = new Dictionary<ServerData, float>();
	private List<ServerData> _sortedServerList = new List<ServerData>();
	private string[] _popupServerNames; 
	
	private void Setup()
	{
		_service = new ConnectionService();
		_service.ServerDataReceived += UpdateServerData; 
		_service.StartReceiveBroadcast();
		_popupServerNames = new string[10];
		UpdateServerNameList();
		
	}


	private string _command;
	private int _currentServerIndex; 
	void OnGUI()
	{
		
		if (_service == null) // react on re-compile
		{
			return;
			
		}
		
		if(_popupServerNames.Length > 0)
		{
			int index = _currentServerIndex;
			index = EditorGUILayout.Popup(_currentServerIndex, _popupServerNames);
			if (index != _currentServerIndex)
			{
				_currentServerIndex = index;
				var serverData = _sortedServerList[_currentServerIndex-1];
				ConnectToServer(serverData);
			}
			if (_service.IsConnected())
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
		}
			
	}

	void Update()
	{
		if (_service == null)
		{
			Setup();
		}
	}

	private void UpdateServerData(ServerData serverData)
	{
		if (!_availableServer.ContainsKey(serverData))
		{
			_availableServer.Add(serverData, 0);
			_sortedServerList.Add(serverData);
			UpdateServerNameList();
		}
		_availableServer[serverData] = 0;
	}

	private void UpdateServerNameList()
	{
		if (_availableServer.Count > _popupServerNames.Length -1)
		{
			_popupServerNames = new string[_popupServerNames.Length+10];
		}
		_popupServerNames[0] = "None";
		
		for (int i= 0; i < _sortedServerList.Count; i++)
		{
			_popupServerNames[i+1] = _sortedServerList[i].IpAddress;
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
	}

}