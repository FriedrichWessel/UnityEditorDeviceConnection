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
		var networkUtilities = new NetworkUtilities();
		_localIpAddress = networkUtilities.GetLocalIPAddress();
		_server = new TcpConnectionServer(_localIpAddress, 15845);
	}
	
	[TearDown]
	public void RunAfterEveryTest()
	{
		_localIpAddress = null;
		_server.StopServer();
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
		_server.DataReceived += (m) => { receivedMessage = m; };
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
		_server.DataReceived += m => received = true;
		yield return ConnectTestClientToServer(testClient);
		_server.SendDataToAllClients("test");
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(testClient.GetStream().DataAvailable);
		_server.Tick();
		Assert.IsFalse(received);
		
	}

	[UnityTest]
	public IEnumerator ClientConnectShouldRaiseClientConnectedEvent()
	{
		TcpClient testClient = new TcpClient();
		bool called = false;
		_server.ClientConnected += (client) =>
		{
			called = true;
		};
		yield return ConnectTestClientToServer(testClient);
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(called);
		
	}

	[UnityTest]
	[Ignore("... until we figure out a way to test this against local without port trouble")]
	public IEnumerator ServerShouldAcceptMultipleClients()
	{
		var client1 = new TcpClient();
		var client2 = new TcpClient();

		List<IConnectionClient> acceptedClients = new List<IConnectionClient>(9);
		_server.ClientConnected += (client) =>
		{
			acceptedClients.Add(client);
		};
		yield return ConnectTestClientToServer(client1);
		yield return ConnectTestClientToServer(client2);
		Assert.AreEqual(2,acceptedClients.Count);
		Assert.Contains(client1, acceptedClients);
		Assert.Contains(client2, acceptedClients);
	}

	private IEnumerator ConnectTestClientToServer(TcpClient testClient)
	{
		_server.StartServer();
		yield return new WaitForSeconds(2);
		testClient.Connect(_server.Adress, _server.Port);
		yield return new WaitForSeconds(0.5f);
	}
}
