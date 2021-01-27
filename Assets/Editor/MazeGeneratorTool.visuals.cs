using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
#############################
  TITLE: MazeGeneratorTool.visuals.cs
  AUTHOR: Philipe Go.
  DATE: 2021-Jan
#############################
*/

public partial class MazeGeneratorTool : EditorWindow {

    [MenuItem("Custom Tools/Maze Generator Tool")]
    private static void ShowWindow() {
        var window = GetWindow<MazeGeneratorTool>();
        window.titleContent = new GUIContent("MazeGeneratorTool");
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.Space();

        Wall = EditorGUILayout.ObjectField("Maze Wall", Wall, typeof(GameObject), true) as GameObject;
        Floor = EditorGUILayout.ObjectField("Maze Floor", Floor, typeof(GameObject), true) as GameObject;

        EditorGUILayout.Space();

        row = EditorGUILayout.IntField( "Maze Nr. rows", row);
        column = EditorGUILayout.IntField("Maze Nr. columns", column);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Maze")) BuildMaze();
    }
}
