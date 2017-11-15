using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ConnectionService : IConnectionService
{
	public event System.Action<string> MessageReceived = (message) => { };
	public event Action<ServerData> ServerDataReceived = (data) => { }; 
	private string _address;
	private int _port; 
	
	public string Address {
		get { return _address; }
		set {
			if (value != _address)
			{
				_address = value;
				UpdateServerURL();
			}
		}
	}
	public int Port {
		get { return _port; }
		set {
			if (value != _port)
			{
				_port = value; 
				UpdateServerURL();
			}
		}
	}
	public string URL { get; private set; }

	private TcpClient _publisher;
	private TcpListener _receiver;
	
	private List<Client> _connectedClients = new List<Client>();
	private List<ICommand> _activeCommands = new List<ICommand>();
	public void StartServer()
	{
		var adress = IPAddress.Parse(Address); 
		_receiver = new TcpListener(adress, Port);
		_receiver.Start();
		_receiver.BeginAcceptSocket(AcceptTcpClient, _receiver);
	}

	public void Tick()
	{
		foreach (var client in _connectedClients)
		{
			var stream = client._tcpClient.GetStream();
			if (stream.DataAvailable)
			{
				var reader = new StreamReader(stream, true);
				string data = reader.ReadLine();
				MessageReceived(data);

			}
		}

		foreach (var command in _activeCommands)
		{
			if (!command.IsRunning)
			{
				command.Execute();
			}
		}
	}

	public void StartBroadcastData(string data)
	{
		_activeCommands.Add(new UdpBroadcastCommand(15000, data));
	}

	public void StopBroadcast()
	{
		ICommand commandToRemove = null; 
		foreach (var command in _activeCommands)
		{
			if (command is UdpBroadcastCommand)
			{
				commandToRemove = command;
			}
		}
		if (commandToRemove != null)
		{
			_activeCommands.Remove(commandToRemove);
		}
	}

	private void AcceptTcpClient(IAsyncResult ar) 
	{
		TcpListener listener = (TcpListener)ar.AsyncState;
		_connectedClients.Add(new Client(listener.EndAcceptTcpClient(ar)));
	}
	
	public void StopServer()
	{
		_receiver.Stop();
	}

	public void ConnectToServer()
	{
		_publisher = new TcpClient();
		_publisher.Connect(Address, Port);
		
	}

	public bool IsConnected()
	{
		return _publisher != null && _publisher.Connected; 
	}

	public void Disconnect()
	{
		if (_publisher != null)
		{
			_publisher.Close();
		}
	}

	public void Send(string command)
	{
		var stream = _publisher.GetStream();
		var writer = new StreamWriter(stream);
		writer.WriteLine(command); 
		writer.Flush();
	}

	private void UpdateServerURL()
	{
		var oldUrl = URL;
		URL = string.Format("{0}:{1}", Address, Port.ToString());
	}

	private UdpClient _broadcastReceiver;
	private IPEndPoint _broadcastReceiveEndPoint;
	public void StartReceiveBroadcast()
	{
		_broadcastReceiveEndPoint = new IPEndPoint(IPAddress.Any, 15000); 
		_broadcastReceiver = new UdpClient(_broadcastReceiveEndPoint);
		_broadcastReceiver.BeginReceive(UpdateServerUrl, new object());
	}

	private  void UpdateServerUrl(IAsyncResult ar)
	{
		Byte[] receiveBytes = _broadcastReceiver.EndReceive(ar, ref _broadcastReceiveEndPoint);
		string receiveString = Encoding.ASCII.GetString(receiveBytes);
		ServerDataReceived(new ServerData(receiveString));
		_broadcastReceiver.Close();
		StartReceiveBroadcast();
	}


	public void StopReceiveBroadcast()
	{
		if (_broadcastReceiver != null)
		{
			_broadcastReceiver.Close();
		}
	}
}