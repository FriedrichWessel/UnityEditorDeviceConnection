namespace EditorConnectionWindow.BaseSystem
{
	public interface IConnectionServer  {
		event System.Action<string> DataReceived;
		event System.Action<IConnectionClient> ClientConnected;
		string Adress { get; }
		int Port { get;  }
		bool IsClientConnected(string ipAddress);
		void StartServer();
		void Tick();
		void StopServer();
	}

}
