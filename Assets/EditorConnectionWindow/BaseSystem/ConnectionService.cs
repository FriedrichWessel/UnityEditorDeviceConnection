using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using EditorConnectionWindow.BaseSystem;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ConnectionService : IConnectionService
{
	 
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

	private IConnectionServer _server;

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


	
	public string GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}