﻿using System.Collections;
using System.Net.Sockets;
using NUnit.Framework; 
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace EditorConnectionWindow.BaseSystem.Tests
{
	public class TcPConnectionServerTest
	{
	
		private IConnectionServer _server;
		private IConnectionClient _testClient;
		private string _localIpAddress; 
		
		[SetUp]
		public void RunBeforeEveryTest()
		{
			var service = new ConnectionService();
			_localIpAddress = service.GetLocalIPAddress();
			_server = new TcpConnectionServer(_localIpAddress, 15845);
			_testClient = Substitute.For<IConnectionClient>();
		}

		[TearDown]
		public void RunAfterEveryTest()
		{
			_server.Disconnect();
		}

		[Test]
		public void IsClientConnectedReturnsFalseIfIPAddressDoesNotMatchAnyClient()
		{
			Assert.IsFalse(_server.IsClientConnected("127.0.0.1"));
		}

		[Test]
		public void AcceptTcpClientAddsClientToConnectedClients()
		{
			var testIp = "127.0.0.1"; 
			_testClient.IpAddress.Returns(testIp);
			_server.AcceptClient(_testClient);
			Assert.IsTrue(_server.IsClientConnected(testIp));
		}

		[Test]
		public void MessageReceivedGetsFiredIfClientDataIsNotEmpty()
		{
			var testData = "testData";
			string compareData = string.Empty;
			_testClient.HasData.Returns(true);
			_testClient.GetData().Returns(testData);
			_server.AcceptClient(_testClient);
			_server.MessageReceived += (data) =>{compareData = data;};
			_server.Tick(); 
			Assert.AreEqual(testData, compareData);
		}

		[UnityTest]
		public IEnumerator StartServerShouldOpenATcpSocket()
		{
			// TODO move to an Integration test
			_server.StartServer();
			yield return null;
			var testClient = new TcpClient();
			testClient.Connect(_server.Adress, _server.Port);
			Assert.IsTrue(_server.IsClientConnected(_localIpAddress));
			
		}
	}

}