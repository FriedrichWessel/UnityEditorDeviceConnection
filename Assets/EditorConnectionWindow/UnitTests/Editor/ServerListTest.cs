using System;
using EditorConnectionWindow.BaseSystem;
using EditorConnectionWindow.BaseSystem.TimeProvider;
using NSubstitute;
using NUnit.Framework;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class ServerListTest
	{
	
		private ServerList _serverList;
		private ITimeProvider _timeProvider;
	
		[SetUp]
		public void RunBeforeEveryTest()
		{
			_timeProvider = Substitute.For<ITimeProvider>();
			_timeProvider.RealtimeSinceStartup.Returns(0);
			_serverList = new ServerList(_timeProvider);
		}
	
		[Test]
		public void AddServerToListShouldAddServerToConnectedServers()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			Assert.IsTrue(_serverList.AddAvailableServer(testData)); 
			Assert.Contains(testData, _serverList.AvailableServers);
		}
	
		[Test]
		public void AddServerToListShouldNotAddExistingServer()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			var testData2 = new AvailableServerData(testServerData);
			Assert.IsTrue(_serverList.AddAvailableServer(testData)); 
			Assert.IsFalse(_serverList.AddAvailableServer(testData2)); 
			Assert.AreEqual(1, _serverList.AvailableServers.Count);
		}

		[Test]
		public void AddServerShouldRaiseServerListChangedEvent()
		{
			bool called = false;
			_serverList.ServerListChanged += () => { called = true; };
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			_serverList.AddAvailableServer(testData);
			Assert.IsTrue(called);
		}
		
		[Test]
		public void AddServerShouldRaiseServerAddedEvent()
		{
			bool called = false;
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			_serverList.ServerAdded += (newServer) =>
			{
				called = true;
				Assert.AreEqual(testServerData, newServer);
			};
			_serverList.AddAvailableServer(testData);
			Assert.IsTrue(called);
		}

		[Test]
		public void SelectServerFromListShouldMakeServerActive()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			_serverList.AddAvailableServer(testData);
			_serverList.SelectServerFromList(testData);
			Assert.AreEqual(testData, _serverList.SelectedServer);
			
		}
	
		[Test]
		public void SelectServerThatIsNotInListThrowsArgumentException()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			Assert.Throws<ArgumentOutOfRangeException>( () => _serverList.SelectServerFromList(testData));
		}

		[Test]
		public void InactiveServerShouldBeRemovedAfterToleranceTime()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			_serverList.AddAvailableServer(testData);
			_timeProvider.RealtimeSinceStartup.Returns(ServerList.SERVER_INACTIVE_TOLERANCE + 0.1f);
			_serverList.Tick();
			Assert.AreEqual(0, _serverList.AvailableServers.Count);
		}

		[Test]
		public void RemoveInactiveServerShouldFireRemoveEvent()
		{
			var testServerData = new ServerData("192.168.0.1:8080");
			var testData = new AvailableServerData(testServerData);
			_serverList.AddAvailableServer(testData);
			bool called = false;
			_serverList.ServerRemoved += (removedServer) =>
			{
				called = true;
				Assert.AreEqual(testServerData, removedServer);
			};
			_timeProvider.RealtimeSinceStartup.Returns(ServerList.SERVER_INACTIVE_TOLERANCE + 0.1f);
			_serverList.Tick();
			Assert.IsTrue(called);
		}
	}
}
