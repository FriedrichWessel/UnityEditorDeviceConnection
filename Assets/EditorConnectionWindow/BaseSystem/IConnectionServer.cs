namespace EditorConnectionWindow.BaseSystem
{
	public interface IConnectionServer  {
		event System.Action<string> MessageReceived;

		string Adress { get; }
		int Port { get;  }
		bool IsClientConnected(string ipAddress);
		void AcceptClient(IConnectionClient connectionClient);
		void StartServer();
		void Tick();
		void Disconnect();
	}

}
