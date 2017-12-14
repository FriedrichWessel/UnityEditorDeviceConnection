using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{
	public class BasicConnectionWindow : EditorWindow
	{
		// Add menu named "My Window" to the Window menu
		[MenuItem("Tools/ConnectionWindow")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			var window = (BasicConnectionWindow) EditorWindow.GetWindow(typeof(BasicConnectionWindow));
			EditorApplication.update += window.EditorUpdateLoop;
			window.Setup();
			window.Show();
		}



		private UnityTimeProvider _timeProvider;
		private ServerList _serverList;

		private List<string> _popupServerNames = new List<string>();
		private int _currentServerIndex;
		private string _command;
		private int _broadcastPort;

		protected IConnectionClient ConnectionClient { get; private set; }

		private void Setup()
		{
			_timeProvider = new UnityTimeProvider();
			_serverList = new ServerList(_timeProvider);
			_serverList.ServerAdded += AddServerToDropDown;
			_serverList.ServerRemoved += RemoveServerFromDropDown;
			_serverList.RemoveSelectedServer += DisconnectFromSelectedServer;
			_popupServerNames.Clear();
			ConnectionClient = new TcpConnectionClient();
			PostSetup();

		}

		protected virtual void PostSetup()
		{
			
		}

		private void StartListenToBroadcast()
		{
			_serverList.StartListingForServers(_broadcastPort);
		}

		private void EditorUpdateLoop()
		{
			if (_serverList == null)
			{
				return;
			}
			_serverList.Tick();
		}

		private void OnDestroy()
		{
			if (_serverList != null)
			{
				_serverList.StopListiningForServers();
			}
			if (ConnectionClient != null)
			{
				ConnectionClient.Disconnect();
			}

		}

		private void DisconnectFromSelectedServer(ServerData serverData)
		{
			ConnectionClient.Disconnect();
		}

		private void RemoveServerFromDropDown(ServerData serverData)
		{
			_popupServerNames.Remove(serverData.IpAddress);
		}

		private void AddServerToDropDown(ServerData serverData)
		{
			var oldCount = _popupServerNames.Count;
			_popupServerNames.Add(serverData.IpAddress);
			if (oldCount == 0)
			{
				_currentServerIndex = 0;
				_serverList.SelectServerFromList(_serverList.AvailableServers[_currentServerIndex]);
				ConnectToServer();

			}
		}

		protected virtual void OnGUI()
		{
			DrawConnectionControls();
			DrawServerList();
			DrawInputArea();
		}

		protected void DrawConnectionControls()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Broadcast Port: ");
			_broadcastPort = EditorGUILayout.IntField(_broadcastPort);
			if (GUILayout.Button("StartListen"))
			{
				StartListenToBroadcast();
			}
			GUILayout.EndHorizontal();
		}
		
		protected void DrawServerList()
		{
			if (_serverList == null) // react on recompile
			{
				Setup();
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
				ConnectToServer();
			}
		}
		
		protected void DrawInputArea()
		{
			if (_serverList == null) // react on recompile
			{
				Setup();
			}
			if (_popupServerNames.Count == 0)
			{
				return;
			}
			
			if (_serverList.SelectedServer != null)
			{
				_command = GUILayout.TextField(_command);
				if (GUILayout.Button("Send"))
				{
					ConnectionClient.SendData(_command);
				}
			}
		}

		private void ConnectToServer()
		{
			var serverData = _serverList.SelectedServer.ServerData;
			ConnectionClient.ConnectToServer(serverData.IpAddress, serverData.Port);
		}
	}
}