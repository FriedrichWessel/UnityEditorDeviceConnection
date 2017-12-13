using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class ServerDataTest
	{
		[Test]
		public void SeverDataShouldReadIpAndPortFromURL()
		{
			var testData = new ServerData("192.0.0.1:9900");
			Assert.AreEqual(testData.IpAddress, "192.0.0.1");
			Assert.AreEqual(testData.Port, 9900);
		}

		[Test]
		public void ServerDataWithSameIPandPortShouldBeConsideredEqual()
		{
			var testData = new ServerData("192.0.0.1:9900");
			var testData2 = new ServerData("192.0.0.1:9900");
			Assert.IsTrue(testData == testData2);
		}

		[Test]
		public void ServerDataWithDifferentIPOrPortShouldBeConsideredNotEqual()
		{
			var testData = new ServerData("192.0.0.2:9900");
			var testData2 = new ServerData("192.0.0.1:9900");
			Assert.IsTrue(testData != testData2);
			Assert.IsFalse(testData == testData2);
		}

		[Test]
		public void ComparisionBetweenServerDataAndNullShouldReturnFalse()
		{
			var testData = new ServerData("192.0.0.2:9900");
			Assert.IsFalse(testData == null);
		}

		[Test]
		public void ToStringShouldShowIPandPort()
		{
			var testData = new ServerData("192.0.0.1:9090");
			Assert.AreEqual("{IP:192.0.0.1 Port:9090}", testData.ToString());
		}


	}
}