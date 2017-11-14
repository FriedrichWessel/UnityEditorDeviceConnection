using System;

public class ServerData
{
	public string IpAddress { get; private set; }
	public int Port { get; private set; }

	public ServerData(string url)
	{
		var data = url.Split(':');
		IpAddress = data[0];
		Port = Convert.ToInt32(data[1]);
	}
}