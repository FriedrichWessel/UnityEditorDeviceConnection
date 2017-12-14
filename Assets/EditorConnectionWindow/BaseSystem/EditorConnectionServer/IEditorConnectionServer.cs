using System;

namespace EditorConnectionWindow.BaseSystem
{
	public interface IEditorConnectionServer : IDisposable
	{
		event Action<string> DataReceived;
		void StartServer();
		void StopServer();
	}
}