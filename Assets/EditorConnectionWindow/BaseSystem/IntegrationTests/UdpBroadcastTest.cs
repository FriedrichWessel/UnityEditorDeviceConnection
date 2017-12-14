using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace EditorConnectionWindow.BaseSystem.IntegrationTests
{
	public class UdpBroadcastTest  {
	
		[UnityTest]
		public IEnumerator ExecuteBroadCastSendsUdpBroadcast()
		{
			var receiver = new UdpBroadcastReceiver(15003);
			receiver.StartReceiveBroadcast();
			string receivedMessage = string.Empty;
			receiver.BroadcastDataReceived += data => receivedMessage = data;
			var testCmd = new UdpBroadcastCommand(15003,"TestData");
			testCmd.Execute();
			yield return new WaitForSeconds(0.5f);
			Assert.AreEqual("TestData", receivedMessage);
		}
	}
}
