using System.Net.Sockets;

public class Client
{
	public TcpClient _tcpClient;
 
	public Client(TcpClient clientSocket) 
	{
		_tcpClient = clientSocket;
	}
}