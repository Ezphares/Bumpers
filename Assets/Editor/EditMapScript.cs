using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBehaviour))]
public class EditMapScript : Editor {

    public override void OnInspectorGUI()
    {
        MapBehaviour myTarget = (MapBehaviour)target;

        serializedObject.Update();

        myTarget.width = EditorGUILayout.IntField("Width", myTarget.width);
        myTarget.height = EditorGUILayout.IntField("Height", myTarget.height);

        if (GUILayout.Button("Set Default Tiles"))
        {
            ((MapBehaviour)target).SetDefaultTiles();
        }

        SerializedProperty tiles = serializedObject.FindProperty("tiles");
        EditorGUILayout.BeginVertical();

        for (int i = 0; i < tiles.arraySize; ++i)
        {
            EditorGUILayout.PropertyField(tiles.GetArrayElementAtIndex(i), new GUIContent(string.Format("({0},{1})", i / myTarget.height, i % myTarget.height)), true);
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

}
