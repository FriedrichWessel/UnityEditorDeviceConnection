using System.Collections;
using EditorConnectionWindow.BaseSystem;
using EditorConnectionWindow.BaseSystem.TimeProvider;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IntegrationTests
{
	public class ServerListTest  {
		private ServerList _testList;
		private ITimeProvider _timeProvider;

		[SetUp]
		public void RunBeforeEveryTest()
		{
			_timeProvider = new UnityTimeProvider();
			_testList = new ServerList(_timeProvider);
		}

		[TearDown]
		public void RunAfterEveryTest()
		{
			_testList.StopListiningForServers();
		}

		[UnityTest]
		public IEnumerator BroadcastServerDataShouldAddDataToAvailableServers()
		{
			var testCmd = new UdpBroadcastCommand(15004, "192.168.0.22:8080");
			_testList.StartListingForServers(15004);
			testCmd.Execute();
			yield return new WaitForSeconds(0.5f);
			Assert.AreEqual(1, _testList.AvailableServers.Count);
			Assert.AreEqual("192.168.0.22", _testList.AvailableServers[0].ServerData.IpAddress);
			Assert.AreEqual(8080, _testList.AvailableServers[0].ServerData.Port);
		}

		[UnityTest]
		public IEnumerator ListDoesNotReceiveNewServersAfterStopListining()
		{
			var testCmd = new UdpBroadcastCommand(15004, "192.168.0.22:8080");
			_testList.StartListingForServers(15004);
			_testList.StopListiningForServers();
			testCmd.Execute();
			yield return new WaitForSeconds(0.5f);
			Assert.AreEqual(0, _testList.AvailableServers.Count);
		}

		[UnityTest]
		public IEnumerator NewServerInListShouldRaiseChangeServerListEvent()
		{
			var testCmd = new UdpBroadcastCommand(15004, "192.168.0.22:8080");
			bool called = false;
			_testList.ServerListChanged += () => { called = true; };
			_testList.StartListingForServers(15004);
			testCmd.Execute();
			yield return new WaitForSeconds(0.5f);
			Assert.IsTrue(called);
		}

		[UnityTest]
		public IEnumerator InactiveServerShouldBeRemovedAfterCheckIntervalSecond()
		{
			var testCmd = new UdpBroadcastCommand(15004, "192.168.0.22:8080");
			_testList.StartListingForServers(15004);
			bool called = false;
			_testList.ServerListChanged += () => { called = true; };
			testCmd.Execute();
			while (!called) yield return null;
			yield return new WaitForSeconds(ServerList.SERVER_INACTIVE_TOLERANCE+ 0.1f);
			_testList.Tick();
			Assert.AreEqual(0, _testList.AvailableServers.Count);
		}
	}

}

