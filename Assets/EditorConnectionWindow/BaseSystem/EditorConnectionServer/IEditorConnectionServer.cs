using System;

public interface IEditorConnectionServer : IDisposable
{
	event Action<string> DataReceived;
	void StartServer();
	void StopServer();
}