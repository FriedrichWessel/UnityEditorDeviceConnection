using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UdpBroadcastAsyncCommand : UdpBroadcastCommand {

	private string _currentData;
	public UdpBroadcastAsyncCommand(int port, string data) : base(port, data)
	{
	}

	public override void Execute()
	{
		SendDataAsync();
	}

	private void SendDataAsync()
	{
		BroadcastServer.BeginSend(Data, Data.Length, EndPoint, FinishBroadcast, new object());
	}
	
	private void FinishBroadcast(IAsyncResult ar)
	{
		BroadcastServer.EndSend(ar);
		BroadcastServer.Close();
	}
}
