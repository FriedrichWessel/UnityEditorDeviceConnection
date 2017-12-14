namespace EditorConnectionWindow.BaseSystem
{

	public interface ICommandScheduler  {
		void AddCommand(ICommand testCommand);
		bool IsCommandActive(ICommand command);
		void Tick();
		void RemoveCommand(ICommand testCommand);
	}
	
}
