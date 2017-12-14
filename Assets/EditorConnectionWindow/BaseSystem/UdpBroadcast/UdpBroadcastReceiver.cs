using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UdpBroadcastReceiver  {
	
	public event Action<string> BroadcastDataReceived = (data) => { };
	
	private UdpClient _broadcastReceiver;
	private IPEndPoint _broadcastReceiveEndPoint;
	private int _port;
	
	public UdpBroadcastReceiver(int port)
	{
		_port = port;
	}

	public void StartReceiveBroadcast()
	{
		_broadcastReceiveEndPoint = new IPEndPoint(IPAddress.Any, _port); 
		_broadcastReceiver = new UdpClient(_broadcastReceiveEndPoint);
		_broadcastReceiver.BeginReceive(ReadReceivedBroadcastData, new object());
	}

	private void ReadReceivedBroadcastData(IAsyncResult ar)
	{
		Byte[] receiveBytes = _broadcastReceiver.EndReceive(ar, ref _broadcastReceiveEndPoint);
		string receivedString = Encoding.ASCII.GetString(receiveBytes);
		BroadcastDataReceived(receivedString);
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
