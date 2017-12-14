using System;
using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class EditorConnectionServerTest
	{
	
		private EditorConnectionServer _testServer;
		private IConnectionServer _dummyConnection;
	
		[SetUp]
		public void RunBeforeEveryTest()
		{
			_dummyConnection = Substitute.For<IConnectionServer>();
			
			_testServer = new EditorConnectionServer(_dummyConnection, Substitute.For<ICommandScheduler>(), 15000);	
		}
	
		[Test]
		public void ReceivingDataShouldRaiseDataReceivedEvent()
		{
			var called = false;
			_testServer.DataReceived += (data) =>
			{
				called = true;
				Assert.AreEqual("Testdata", data);
			};
			_dummyConnection.DataReceived += Raise.Event<Action<string>>("Testdata");
			Assert.IsTrue(called);
		}
	}
}
