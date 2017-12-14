using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using UnityEditor;
using UnityEngine;

public class ReceiveDeviceLogWindow : BasicConnectionWindow {

	[MenuItem("Tools/ReceiveDeviceLogWindow")]
	public static void Init()
	{
		var window = EditorWindow.GetWindow<ReceiveDeviceLogWindow>();
		window.Show();
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		if (ConnectionClient.HasData)
		{
			Debug.Log(string.Format("Server {0} says: {1}",ConnectionClient.IpAddress, ConnectionClient.GetData()));
		}
	}
}
