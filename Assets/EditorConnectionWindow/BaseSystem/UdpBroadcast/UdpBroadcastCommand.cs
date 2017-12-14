using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UdpBroadcastCommand : ICommand
{

	public bool IsRunning { get; private set; }
	private UdpClient _broadcastServer;
	private IPEndPoint _endPoint;
	private byte[] _data;

	public UdpBroadcastCommand(int port, string data)
	{
		IsRunning = false;
		_endPoint = new IPEndPoint(IPAddress.Broadcast, port);
		_data = Encoding.ASCII.GetBytes(data);
	}

	public void Execute()
	{
		IsRunning = true;
		SendData();
	}

	private void SendData()
	{
		_broadcastServer = new UdpClient();
		_broadcastServer.Send(_data, _data.Length, _endPoint);
		_broadcastServer.Close();
		IsRunning = false;
	}
}
