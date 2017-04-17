using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ImposterTextureGenerator))]
public class ImposterTextureGeneratorEditor : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            (target as ImposterTextureGenerator).Generate();
            AssetDatabase.Refresh();
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }
}
