namespace EditorConnectionWindow.BaseSystem
{
	public interface IConnectionServer  {
		event System.Action<string> MessageReceived; 
		
		bool IsClientConnected(string ipAddress);
		void AcceptClient(IConnectionClient connectionClient);
		void Tick();
	}

}
