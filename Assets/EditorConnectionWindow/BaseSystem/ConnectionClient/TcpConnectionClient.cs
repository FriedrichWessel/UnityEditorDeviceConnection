using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EditorConnectionWindow.BaseSystem
{

	public class TcpConnectionClient : IConnectionClient
	{
		private TcpClient _tcpClient;

		public string IpAddress { get; private set; }

		public bool HasData
		{
			get
			{
				if (_tcpClient.Connected)
				{
					return _tcpClient.GetStream().DataAvailable;
				}
				return false;
			}
		}

		public TcpConnectionClient()
		{
			_tcpClient = new TcpClient();
		}

		public TcpConnectionClient(TcpClient clientSocket)
		{
			_tcpClient = clientSocket;
			var adress = ((IPEndPoint) clientSocket.Client.RemoteEndPoint).Address.ToString();
			IpAddress = adress;
		}

		public string GetData()
		{
			string result = string.Empty;
			if (HasData)
			{
				var stream = new StreamReader(_tcpClient.GetStream());
				result = stream.ReadLine();
			}
			return result;
		}

		public void SendData(string data)
		{
			var stream = new StreamWriter(_tcpClient.GetStream(), Encoding.ASCII);
			stream.WriteLine(data);
			stream.Flush();
		}

		public void ConnectToServer(string ipAddress, int port)
		{
			IpAddress = ipAddress;
			_tcpClient.Connect(ipAddress, port);
		}

		public void Disconnect()
		{
			if (_tcpClient.Connected)
			{
				_tcpClient.GetStream().Close();
				_tcpClient.Close();
			}
		}
	}
}