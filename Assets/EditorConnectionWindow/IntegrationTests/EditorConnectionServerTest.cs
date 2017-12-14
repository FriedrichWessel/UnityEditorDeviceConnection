using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditorConnectionServerTest  {
	private IConnectionServer _testConnectionServer;
	private EditorConnectionServer _testServer;
	private NetworkUtilities _utility;
	private ICommandScheduler _testScheduler;

	[SetUp]
	public void RunBeforeEveryTest()
	{
		_utility = new NetworkUtilities();
		_testConnectionServer = new TcpConnectionServer(_utility.GetLocalIPAddress(), 8080);
		_testScheduler = new CommandScheduler();
		_testServer = new EditorConnectionServer(_testConnectionServer, _testScheduler, 15000);
	}

	[TearDown]
	public void RunAfterEveryTest()
	{
		_testServer.StopServer();
	}

	[UnityTest]
	public IEnumerator ServerShouldBroadcastIpAndPortAfterStart()
	{
		var testReceiver = new UdpBroadcastReceiver(15000);
		testReceiver.StartReceiveBroadcast();
		_testServer.StartServer();
		var called = false;
		testReceiver.BroadcastDataReceived += data =>
		{
			called = true;
			Assert.AreEqual(_utility.GetLocalIPAddress() + ":8080", data);
		};
		_testScheduler.Tick();
		yield return new WaitForSeconds(0.5f);
		Assert.IsTrue(called);
		testReceiver.StopReceiveBroadcast();
	}
	
	[UnityTest]
	public IEnumerator ServerShouldNotBroadcastAfterStopBroadcast()
	{
		var testReceiver = new UdpBroadcastReceiver(15000);
		testReceiver.StartReceiveBroadcast();
		_testServer.StartServer();
		_testScheduler.Tick();
		yield return new WaitForSeconds(1);
		_testServer.StopServer();
		var called = false;
		testReceiver.BroadcastDataReceived += data => called = true;
		_testScheduler.Tick();
		yield return new WaitForSeconds(0.5f);
		Assert.False(called);
		testReceiver.StopReceiveBroadcast();
	}

}
