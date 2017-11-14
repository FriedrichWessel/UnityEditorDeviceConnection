using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public abstract class UdpBroadcastCommand : ICommand  {

	protected UdpClient BroadcastServer { get; set; }
	protected IPEndPoint EndPoint { get; private set; }
	protected byte[] Data { get; private set; }

	public UdpBroadcastCommand(int port, string data)
	{
		BroadcastServer = new UdpClient();
		EndPoint = new IPEndPoint(IPAddress.Broadcast, port);
		Data = Encoding.ASCII.GetBytes(data);
	}

	public abstract void Execute();
}
