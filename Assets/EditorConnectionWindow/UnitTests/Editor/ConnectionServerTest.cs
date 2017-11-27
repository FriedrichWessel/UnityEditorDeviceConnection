using System.Net.Sockets;
using NUnit.Framework; 
using NSubstitute;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem.Tests
{
	public class ConnectionServerTest
	{
	
		private IConnectionServer _server;
		private IConnectionClient _testClient;
		
		[SetUp]
		public void RunBeforeEveryTest()
		{
			_server = new ConnectionServer();
			_testClient = Substitute.For<IConnectionClient>();
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
			_server.MessageReceived += (data) => { compareData = data; };
			_server.Tick(); 
			Assert.AreEqual(testData, compareData);
		}

		/*[Test]
		public void StartServerOpensTcpConnectionOnGivenURL()
		{
			_server.StartServer();
		}*/
	}

}
