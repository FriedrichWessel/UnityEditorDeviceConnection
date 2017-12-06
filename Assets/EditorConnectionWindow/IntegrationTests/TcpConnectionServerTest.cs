using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TcpConnectionServerTest  {
	
	private TcpConnectionServer _server;
	private string _localIpAddress; 

	public void Setup()
	{
		var service = new ConnectionService();
		_localIpAddress = service.GetLocalIPAddress();
		_server = new TcpConnectionServer(_localIpAddress, 15845);
	}
	private void TearDown()
	{
		_localIpAddress = null;
		_server.Disconnect();
		_server = null;
	}
	
	[UnityTest]
	public IEnumerator StartServerShouldOpenATcpSocket()
	{
		Setup();
		_server.StartServer();
		yield return new WaitForSeconds(2);
		var testClient = new TcpClient();
		testClient.Connect(_server.Adress, _server.Port);
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(_server.IsClientConnected(_localIpAddress));
		TearDown();

	}

}
