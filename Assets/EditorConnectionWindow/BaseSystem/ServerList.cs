using System;
using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem.TimeProvider;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{
	public class ServerList {
		
		public const float CHECK_FOR_INACTIVE_SERVER_INTERVAL = 1;
		public const float SERVER_INACTIVE_TOLERANCE = 2;
		
		public event Action ServerListChanged = () => { };
		public event Action<ServerData> ServerRemoved = (removedServer) => { };
		public event Action<ServerData> ServerAdded = (newServer) => { };
		
		public List<AvailableServerData> AvailableServers { get; private set; }
		
		private UdpBroadcastReceiver _broadcastReceiver;
		private int _selectedServerIndex;
		private float _currentTimeStamp;
		private ITimeProvider _timeProvider;
		private float _lastInactiveServerCheck;
		
		public AvailableServerData SelectedServer {
			get { return AvailableServers[_selectedServerIndex]; }
		}


		public ServerList(ITimeProvider timeProvider)
		{
			AvailableServers = new List<AvailableServerData>();
			_timeProvider = timeProvider;
			_currentTimeStamp = _timeProvider.RealtimeSinceStartup;
			_lastInactiveServerCheck = _currentTimeStamp;
		}
	
		public bool AddAvailableServer(AvailableServerData availableServerData)
		{
			if (!IsServerInList(availableServerData))
			{
				AvailableServers.Add(availableServerData);
				ServerAdded(availableServerData.ServerData);
				ServerListChanged();
				return true;
			}
			return false;
		}
	
		public void StartListingForServers(int port)
		{
			_broadcastReceiver = new UdpBroadcastReceiver(port);
			_broadcastReceiver.BroadcastDataReceived += UpdateConnectedServer;
			_broadcastReceiver.StartReceiveBroadcast();
		}
	
		public void StopListiningForServers()
		{
			_broadcastReceiver.StopReceiveBroadcast();
		}
	
		public void SelectServerFromList(AvailableServerData testData)
		{
			if (!IsServerInList(testData))
			{
				throw new ArgumentOutOfRangeException(string.Format("Server: {0} is not in list", testData.ToString()));
			}
			_selectedServerIndex =  AvailableServers.IndexOf(testData);
		}
	
		public void Tick()
		{
			// realtime can only be used in Main.Thread
			// so we store the last seen for the tcp callbacks
			_currentTimeStamp = _timeProvider.RealtimeSinceStartup;
			var timeSinceLastServerListUpdate = _timeProvider.RealtimeSinceStartup - _lastInactiveServerCheck;
			if (timeSinceLastServerListUpdate > CHECK_FOR_INACTIVE_SERVER_INTERVAL)
			{
				RemoveInactiveServers();
				_lastInactiveServerCheck = _timeProvider.RealtimeSinceStartup;
			}
		}

		private void RemoveInactiveServers()
		{
			ServerData removedServer = null; 
			for (int i = AvailableServers.Count-1; i >= 0; i--)
			{
				if (_timeProvider.RealtimeSinceStartup - AvailableServers[i].LastConnectionTime > SERVER_INACTIVE_TOLERANCE)
				{
					removedServer = AvailableServers[i].ServerData;
					AvailableServers.RemoveAt(i);
					i--;
				}
			}
			if (removedServer != null)
			{
				ServerRemoved(removedServer);
				ServerListChanged();
			}
		}

		private void UpdateConnectedServer(string broadcastData)
		{
			var data = new ServerData(broadcastData);
			var availableServer = new AvailableServerData(data);
			if (!AddAvailableServer(availableServer))
			{
				var serverIndex = IndexOfServer(availableServer);
				AvailableServers[serverIndex].LastConnectionTime = _currentTimeStamp;
			}
		}
	
		private bool IsServerInList(AvailableServerData availableServerData)
		{
			return IndexOfServer(availableServerData) >= 0;
		}
	
		private int IndexOfServer(AvailableServerData data)
		{
			int result = -1;
			for(int i=0; i<AvailableServers.Count; i++)
			{
				var server = AvailableServers[i];
				if (server.ServerData == data.ServerData)
				{
					result = i;
					break;
				}
			}
			return result;
		}
	}
}
