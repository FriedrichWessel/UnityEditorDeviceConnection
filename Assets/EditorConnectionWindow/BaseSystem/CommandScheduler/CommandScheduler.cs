using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{
	
	public class CommandScheduler : ICommandScheduler {
		
		private List<ICommand> _activeCommands = new List<ICommand>();

		public bool IsCommandActive(ICommand command)
		{
			return _activeCommands.Contains(command);
		}

		public void AddCommand(ICommand command)
		{
			_activeCommands.Add(command);
		}

		public void Tick()
		{
			foreach (var command in _activeCommands)
			{
				if (!command.IsRunning)
				{
					command.Execute();
				}
			}
		}

		public void RemoveCommand(ICommand testCommand)
		{
			_activeCommands.Remove(testCommand);
		}
	}
}

