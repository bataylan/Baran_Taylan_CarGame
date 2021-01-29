using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TurnPointInfo)), CanEditMultipleObjects]
public class TurnPointInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TurnPointInfo script = (TurnPointInfo)target;

        if (GUILayout.Button("Update Turn Point"))
        {
            script.UpdateTurnPointInfo();
        }
    }
}