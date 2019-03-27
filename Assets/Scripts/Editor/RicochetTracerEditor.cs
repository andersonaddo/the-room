using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RicochetTracer))]
public class RicochetTracerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("GenerateEditorPath"))
        {
            RicochetTracer script = (RicochetTracer)target;
            script.setEditorPath();
            SceneView.RepaintAll();
        }
    }
}
