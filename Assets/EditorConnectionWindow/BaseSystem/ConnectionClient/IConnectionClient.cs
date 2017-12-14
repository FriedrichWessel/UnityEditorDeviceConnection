namespace EditorConnectionWindow.BaseSystem
{
	public interface IConnectionClient  {

		string IpAddress { get;  }
		bool HasData { get; }
		string GetData();
		void SendData(string data);
		void ConnectToServer(string ipAddress, int port);
		void Disconnect();
	}
}
