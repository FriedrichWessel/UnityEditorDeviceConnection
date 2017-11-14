using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
			command.Execute();
		}
	}

	public void StartBroadcastData(string data)
	{
		_activeCommands.Add(new UdpBroadcastSyncCommand(15000, data));
	}

	public void StopBroadcast()
	{
		ICommand commandToRemove = null; 
		foreach (var command in _activeCommands)
		{
			if (command is UdpBroadcastSyncCommand)
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

	public void Disconnect()
	{
		_publisher.Close();
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
		var state = new UdpState(_broadcastReceiveEndPoint, _broadcastReceiver);
		_broadcastReceiver.BeginReceive(UpdateServerUrl, new object());
	}

	private  void UpdateServerUrl(IAsyncResult ar)
	{
		Byte[] receiveBytes = _broadcastReceiver.EndReceive(ar, ref _broadcastReceiveEndPoint);
		string receiveString = Encoding.ASCII.GetString(receiveBytes);
		ServerDataReceived(new ServerData(receiveString));
	}
	
	// THis should work
	/* https://stackoverflow.com/questions/10832770/sending-udp-broadcast-receiving-multiple-messages
	 *public class Receiver {
  private readonly UdpClient udp = new UdpClient(15000);
  private void StartListening()
  {
    this.udp.BeginReceive(Receive, new object());
  }
  private void Receive(IAsyncResult ar)
  {
    IPEndPoint ip = new IPEndPoint(IPAddress.Any, 15000);
    byte[] bytes = udp.EndReceive(ar, ref ip);
    string message = Encoding.ASCII.GetString(bytes);
    StartListening();
  }
}

public class Sender {
  public void Send() {
    UdpClient client = new UdpClient();
    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
    byte[] bytes = Encoding.ASCII.GetBytes("Foo");
    client.Send(bytes, bytes.Length, ip);
    client.Close();
  }
}
	 * 
	 */

	public class UdpState{
		public IPEndPoint EndPoint { get; private set; }
		public UdpClient Client { get; private set; }

		public UdpState(IPEndPoint endpoint, UdpClient client)
		{
			EndPoint = endpoint;
			Client = client;
		}
	}

	public class ServerData
	{
		public string IpAddress { get; private set; }
		public int Port { get; private set; }

		public ServerData(string url)
		{
			var data = url.Split(':');
			IpAddress = data[0];
			Port = Convert.ToInt32(data[1]);
		}
	}
}