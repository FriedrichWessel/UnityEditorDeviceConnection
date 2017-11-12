using UnityEditor;

public class BasicConnectionWindow : EditorWindow {

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/ConnectionWindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (BasicConnectionWindow)EditorWindow.GetWindow(typeof(BasicConnectionWindow));
        window.Show();
    }
}
