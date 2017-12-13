using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{
	public class TcpConnectionServer : IConnectionServer {
		
		public event Action<string> MessageReceived = (data) => { };
		public event Action<IConnectionClient> ClientConnected = (client) => { };

		private Dictionary<string,IConnectionClient> _connectedClients = new Dictionary<string, IConnectionClient>();
		public string Adress { get; private set; }
		public int Port { get; private set; }
		

		private TcpListener _tcpListener;

		public TcpConnectionServer(string address, int port)
		{
			Adress = address;
			Port = port;
		}

		public bool IsClientConnected(string ipAddress)
		{
			return _connectedClients.ContainsKey(ipAddress);
		}
	
		public void AcceptClient(IConnectionClient connectionClient)
		{
			_connectedClients.Add(connectionClient.IpAddress, connectionClient);
			ClientConnected(connectionClient);
		}

		public void StartServer()
		{
			var adress = IPAddress.Parse(Adress); 
			_tcpListener = new TcpListener(adress, Port);
			_tcpListener.Start();
			_tcpListener.BeginAcceptSocket(OnTcpClientConnected, _tcpListener);
		}

		private void OnTcpClientConnected(IAsyncResult ar)
		{
			TcpListener listener = (TcpListener)ar.AsyncState;
			var connectionClient = new TcpConnectionClient(listener.EndAcceptTcpClient(ar));
			AcceptClient(connectionClient);
		}

		public void Tick()
		{
			foreach (var client in _connectedClients.Values)
			{
				if (client.HasData)
				{
					MessageReceived(client.GetData());
				}
			}
		}

		public void StopServer()
		{
			if (_tcpListener != null)
			{
				_tcpListener.Stop();
			}
		}

		public void SendDataToAllClients(string data)
		{
			foreach (var client in _connectedClients.Values)
			{
				client.SendData(data);
			}
		}
	}
}
