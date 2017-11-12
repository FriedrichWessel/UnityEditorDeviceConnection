using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ConnectionService : IConnectionService
{
	public event System.Action<string> MessageReceived = (message) => { }; 
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

	public void StartServer()
	{
		var adress = IPAddress.Parse(Address); 
		_receiver = new TcpListener(adress, Port);
		_receiver.Start();
		_receiver.BeginAcceptSocket(AcceptTcpClient, _receiver);
		/*Socket socket = _receiver.AcceptSocket();
		var size = socket.ReceiveBufferSize;
		byte[] bytes = new byte[size];
		var m = socket.Receive(bytes);
		string message = ""; 
		for (int i = 0; i < m; i++)
		{
			message += Convert.ToChar(bytes[i]);
		}
		MessageReceived(message);
		socket.Close();*/
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
	}

	private void AcceptTcpClient(IAsyncResult ar) 
	{
		TcpListener listener = (TcpListener)ar.AsyncState;
		_connectedClients.Add(new Client(listener.EndAcceptTcpClient(ar)));
		//clients.Add (new ServerClient (listener.EndAcceptTcpClient (ar)));
		//Debug.Log("New connection established");
		//StartListening ();
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
	
	public class Client
	{
		public TcpClient _tcpClient;
 
		public Client(TcpClient clientSocket) 
		{
			_tcpClient = clientSocket;
		}
	}

	public IEnumerator BroadCastIPAndPort()
	{
		var responseData = Encoding.ASCII.GetBytes(URL);     
		while (true)
		{
			var server = new UdpClient(8888);
			var clientEp = new IPEndPoint(IPAddress.Any, 0);
			var clientRequestData = server.Receive(ref clientEp);
			var clientRequest = Encoding.ASCII.GetString(clientRequestData);

			//Console.WriteLine($"Recived {clientRequest} from {clientEp.Address}, sending 
			//response: {responseData}");
			server.Send(responseData, responseData.Length, clientEp);
			server.Close();
			yield return new WaitForSeconds(1);
		}
	}

	public static string ReceiveBroadCast()
	{
		var Client = new UdpClient();
		var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
		var ServerEp = new IPEndPoint(IPAddress.Any, 0);

		Client.EnableBroadcast = true;
		Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

		var ServerResponseData = Client.Receive(ref ServerEp);
		var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
		//Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());

		Client.Close();
		return ServerResponse;
	}
}