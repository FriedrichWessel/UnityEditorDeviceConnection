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
			var networkUtilities = new NetworkUtilities();
			_localIpAddress = networkUtilities.GetLocalIPAddress();
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
			var testConnectionClient = new TcpConnectionClient();
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

		[UnityTest]
		public IEnumerator ConnectedClientCanReceiveData()
		{
			var testConnectionClient = new TcpConnectionClient();
			testConnectionClient.ConnectToServer(_localIpAddress, _testPort);
			yield return new WaitForSeconds(0.5f);
			_testListener.SendDataToAllClients("test");
			yield return new WaitForSeconds(0.5f);
			Assert.IsTrue(testConnectionClient.HasData);
			yield return new WaitForEndOfFrame();
			Assert.AreEqual("test", testConnectionClient.GetData());
		}
		
		[UnityTest]
		public IEnumerator DisconnectedClientShouldNotReceiveData()
		{
			var testConnectionClient = new TcpConnectionClient();
			testConnectionClient.ConnectToServer(_localIpAddress, _testPort);
			yield return new WaitForSeconds(0.5f);
			testConnectionClient.Disconnect();
			_testListener.SendDataToAllClients("test");
			yield return new WaitForSeconds(0.5f);
			Assert.IsFalse(testConnectionClient.HasData);
		}
	}

}
