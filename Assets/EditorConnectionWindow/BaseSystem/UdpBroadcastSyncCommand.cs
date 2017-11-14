using System.Net.Sockets;
using System.Text;

public class UdpBroadcastSyncCommand : UdpBroadcastCommand {

	public UdpBroadcastSyncCommand(int port, string data) : base(port, data)
	{
	}

	public override void Execute()
	{
		SendData();
	}

	public void SendData()
	{
		BroadcastServer = new UdpClient();
		BroadcastServer.Send(Data, Data.Length, EndPoint);
		BroadcastServer.Close();
	}

}
