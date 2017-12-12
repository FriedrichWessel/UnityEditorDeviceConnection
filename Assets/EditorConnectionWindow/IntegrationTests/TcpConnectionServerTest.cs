using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TcpConnectionServerTest  {
	
	private TcpConnectionServer _server;
	private string _localIpAddress; 

	[SetUp]
	public void RunBeforeEveryTest()
	{
		var service = new ConnectionService();
		_localIpAddress = service.GetLocalIPAddress();
		_server = new TcpConnectionServer(_localIpAddress, 15845);
	}
	
	[TearDown]
	public void RunAfterEveryTest()
	{
		_localIpAddress = null;
		_server.Disconnect();
		_server = null;
	}
	
	[UnityTest]
	public IEnumerator StartServerShouldOpenATcpSocket()
	{
		_server.StartServer();
		yield return new WaitForSeconds(2);
		var testClient = new TcpClient();
		testClient.Connect(_server.Adress, _server.Port);
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(_server.IsClientConnected(_localIpAddress));
	}

	[UnityTest]
	public IEnumerator ServerShouldFireReceiveDataMessage()
	{
		TcpClient testClient = new TcpClient();
		yield return ConnectTestClientToServer(testClient);
		string receivedMessage = string.Empty;
		_server.MessageReceived += (m) => { receivedMessage = m; };
		var stream = new StreamWriter(testClient.GetStream(), Encoding.ASCII);
		stream.Write("test");
		stream.Close();
		_server.Tick();
		Assert.AreEqual("test", receivedMessage);
	}

	[UnityTest]
	public IEnumerator ServerCanSendDataToClient()
	{
		var testClient = new TcpClient();
		bool received = false;
		_server.MessageReceived += m => received = true;
		yield return ConnectTestClientToServer(testClient);
		_server.SendDataToAllClients("test");
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(testClient.GetStream().DataAvailable);
		_server.Tick();
		Assert.IsFalse(received);
		
	}

	private IEnumerator ConnectTestClientToServer(TcpClient testClient)
	{
		_server.StartServer();
		yield return new WaitForSeconds(2);
		testClient.Connect(_server.Adress, _server.Port);
		yield return new WaitForSeconds(0.5f);
	}
}
