using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

	public void SendData(string data)
	{
		var stream = new StreamWriter(_tcpClient.GetStream(), Encoding.ASCII);
		stream.Write(data);
		stream.Flush();
	}

	public void ConnectToServer(string localIpAddress, int testPort)
	{
		throw new System.NotImplementedException();
	}
}