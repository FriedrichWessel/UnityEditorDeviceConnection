using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem.TimeProvider
{
	public interface ITimeProvider
	{
		float RealtimeSinceStartup { get; }
	}
}

