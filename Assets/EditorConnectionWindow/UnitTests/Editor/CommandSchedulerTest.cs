using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using NSubstitute;

namespace EditorConnectionWindow.BaseSystem.UnitTests
{
	public class CommandSchedulerTest
	{
		private ICommandScheduler _scheduler;
		private ICommand _testCommand;
	
		[SetUp]
		public void RunBeforeEveryTest()
		{
			_scheduler = new CommandScheduler();
			_testCommand = Substitute.For<ICommand>();
		}

		[Test]
		public void IsCommandActiveReturnsTrueOnActiveCommand()
		{
			_scheduler.AddCommand(_testCommand);
			Assert.IsTrue(_scheduler.IsCommandActive(_testCommand));
		}

		[Test]
		public void IsCommandActiveReturnsFalseOnMissingCommand()
		{
			Assert.IsFalse(_scheduler.IsCommandActive(_testCommand));
		}

		[Test]
		public void ActiveCommandGetExecutedOnTick()
		{
			_scheduler.AddCommand(_testCommand);
			_scheduler.Tick();
			_testCommand.Received(1).Execute();
		}

		[Test]
		public void IsCommandActiveReturnsFalseAfterRemoveCommand()
		{
			_scheduler.AddCommand(_testCommand);
			_scheduler.RemoveCommand(_testCommand);
			Assert.IsFalse(_scheduler.IsCommandActive(_testCommand));
		}

		[Test]
		public void CommandDoesNotReceiveExecuteAfterRemoved()
		{
			_scheduler.AddCommand(_testCommand);
			_scheduler.Tick();
			_scheduler.RemoveCommand(_testCommand);
			_scheduler.Tick();
			_testCommand.Received(1).Execute();
		}

		[Test]
		public void AlreadyRunningCommandIsNotExecuted()
		{
			_scheduler.AddCommand(_testCommand);
			_testCommand.IsRunning.Returns(true);
			_scheduler.Tick();
			_testCommand.Received(0).Execute();
		}
	}
}

