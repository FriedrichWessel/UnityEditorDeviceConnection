using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using NUnit.Framework;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class AvailableServerDataTest
	{
		[Test]
		public void ToStringShouldShowIPandPort()
		{
			var testData = new AvailableServerData(new ServerData("192.0.0.1:9090"));
			Assert.AreEqual("{{IP:192.0.0.1 Port:9090} LastConnectionTime:0}", testData.ToString());
		}
	}
}