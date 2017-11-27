using System.IO;
using System.Net;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using UnityEngine;

public class TcpConnectionClient : IConnectionClient
{
	private TcpClient _tcpClient;
 
	public string IpAddress { get; private set; }
	public bool HasData {
		get { return _tcpClient.GetStream().DataAvailable; }
	}
	
	public TcpConnectionClient(TcpClient clientSocket) 
	{
		_tcpClient = clientSocket;
		var adress = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();
		IpAddress = adress;
	}
	

	public string GetData()
	{
		string result = string.Empty;
		if (HasData)
		{
			var reader = new StreamReader(_tcpClient.GetStream(), true);
			result = reader.ReadLine();
		}
		return result;
	}
}