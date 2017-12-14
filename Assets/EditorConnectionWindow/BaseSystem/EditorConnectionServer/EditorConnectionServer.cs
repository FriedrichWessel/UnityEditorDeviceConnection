using System;
using EditorConnectionWindow.BaseSystem;

namespace EditorConnectionWindow.BaseSystem
{
	public class EditorConnectionServer : IEditorConnectionServer
	{
		public event Action<string> DataReceived;
		private IConnectionServer _server;
		private UdpBroadcastCommand _command;
		private ICommandScheduler _scheduler;

		public EditorConnectionServer(IConnectionServer server, ICommandScheduler scheduler, int broadcastPort)
		{
			_server = server;
			_server.DataReceived += PublishReceivedData;
			_scheduler = scheduler;
			var data = string.Format("{0}:{1}", _server.Adress, _server.Port);
			_command = new UdpBroadcastCommand(broadcastPort, data);
		}

		private void PublishReceivedData(string data)
		{
			if (DataReceived != null)
			{
				DataReceived(data);
			}
		}

		public void Dispose()
		{
			StopServer();
			_server.StopServer();
			_scheduler.RemoveCommand(_command);
			_server.DataReceived -= PublishReceivedData;
		}

		public void StartServer()
		{
			_server.StartServer();
			_scheduler.AddCommand(_command);
		}

		public void StopServer()
		{
			_server.StopServer();
			_scheduler.RemoveCommand(_command);
		}

		public void Tick()
		{
			_scheduler.Tick();
			_server.Tick();
		}
	}
}
