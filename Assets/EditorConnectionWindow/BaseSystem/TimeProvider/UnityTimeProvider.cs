using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorConnectionWindow.BaseSystem
{

	public class UnityTimeProvider : ITimeProvider {
		public float RealtimeSinceStartup {
			get { return Time.realtimeSinceStartup; }
		}
	}

}
