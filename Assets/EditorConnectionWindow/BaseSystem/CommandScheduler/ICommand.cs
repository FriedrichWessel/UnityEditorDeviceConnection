namespace EditorConnectionWindow.BaseSystem
{
	public interface ICommand
	{
		void Execute();
		bool IsRunning { get; }
	}
}
