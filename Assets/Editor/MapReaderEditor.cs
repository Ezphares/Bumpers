using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapReaderBehaviour))]
public class MapReaderEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapReaderBehaviour myTarget = (MapReaderBehaviour)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("map"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("board"));

        if (GUILayout.Button("Generate"))
        {
            myTarget.board.map = myTarget.map;
            myTarget.board.Generate();
            myTarget.board.isPregenerated = true;
        }

        if (GUILayout.Button("Clear"))
        {
            myTarget.board.Clear();
            myTarget.board.isPregenerated = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
