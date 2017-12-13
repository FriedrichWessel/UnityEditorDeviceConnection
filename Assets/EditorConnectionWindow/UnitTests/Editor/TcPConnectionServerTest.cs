using System.Collections;
using System.Net.Sockets;
using NUnit.Framework; 
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class TcPConnectionServerTest
	{
	
		private TcpConnectionServer _server;
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
			_server.StopServer();
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

	}

}
