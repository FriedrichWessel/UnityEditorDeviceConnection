using NUnit.Framework; 

public class ConnectionServiceTest
{

	private IConnectionService _serviceToTest; 
	
	[SetUp]
	public void RunBeforeEveryTest()
	{
		_serviceToTest = new ConnectionService();
	}

	[Test]
	public void ConfigureAddressAndPortResultsInCorrectURL()
	{
		_serviceToTest.Address = "127.0.0.1";
		_serviceToTest.Port = 8080;
		Assert.AreEqual("127.0.0.1:8080", _serviceToTest.URL);
	}

}
