using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoStartClient))]
public class Editor_AutoStartClient : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Start Client"))
        {
            if (!EditorApplication.isPlaying)
                return;

            var command = target as AutoStartClient;
            command.StartClient();
        }
    }
}
