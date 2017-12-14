using System.Collections;
using System.Collections.Generic;
using EditorConnectionWindow.BaseSystem;
using UnityEditor;
using UnityEngine;

public class LittleGuyEditorWindow : BasicConnectionWindow {

	[MenuItem("Tools/LittleGuyWindow")]
	public static void Init()
	{
		var window = (LittleGuyEditorWindow) EditorWindow.GetWindow<LittleGuyEditorWindow>();
		window.Show();
	}

	protected override void OnGUI()
	{
		DrawConnectionControls();
		DrawServerList();
		
		if (GUILayout.Button("Walk"))
		{
			ConnectionClient.SendData("Walk");
		}
		
		if (GUILayout.Button("Idle"))
		{
			ConnectionClient.SendData("Idle");
		}
	}
}
