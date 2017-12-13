using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IntegrationTests
{
	public class TcpConnectionClientTest  {

		private string _localIpAddress;
		private TcpConnectionServer _testListener;
		private int _testPort;

		[SetUp]
		public void RunBeforeEveryTest()
		{
			var service = new ConnectionService();
			_localIpAddress = service.GetLocalIPAddress();
			_testPort = 15987;
			_testListener = new TcpConnectionServer(_localIpAddress,_testPort );
			_testListener.StartServer();
		}

		[TearDown]
		public void RunAfterEveryTest()
		{
			_testListener.StopServer();
		}

		[UnityTest]
		public IEnumerator ClientShouldBeAbleToConnectToAServer()
		{
			var testClient = new TcpClient();
			var testConnectionClient = new TcpConnectionClient(testClient);
			bool called = false;
			_testListener.ClientConnected += (client) =>
			{
				called = true;
				Assert.AreEqual(testConnectionClient, client);
			};
			testConnectionClient.ConnectToServer(_localIpAddress, _testPort);
			yield return new WaitForSeconds(0.5f);
			Assert.IsTrue(called);
		}
	}

}
