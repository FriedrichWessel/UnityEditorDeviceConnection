using System;
using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{
	public class ConnectionServer : IConnectionServer {
		
		public event Action<string> MessageReceived;
		
		private Dictionary<string,IConnectionClient> _connectedClients = new Dictionary<string, IConnectionClient>();

		public bool IsClientConnected(string ipAddress)
		{
			return _connectedClients.ContainsKey(ipAddress);
		}
	
		public void AcceptClient(IConnectionClient connectionClient)
		{
			_connectedClients.Add(connectionClient.IpAddress, connectionClient);
		}

		public void Tick()
		{
			throw new NotImplementedException();
		}
	}
}
