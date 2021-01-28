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

public partial class MazeGeneratorTool : EditorWindow
{

    bool wallGroup = false;
    bool floorGroup = false;


    [MenuItem("Custom Tools/Maze Generator Tool")]
    private static void ShowWindow()
    {
        var window = GetWindow<MazeGeneratorTool>();
        window.titleContent = new GUIContent("MazeGeneratorTool");
        window.Show();
    }

    private void OnGUI()
    {

        EditorGUILayout.Space(15, true);


        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
        GUILayout.Label("Maze Construction Prefabs");
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
        EditorGUILayout.Space(5, true);
        wallGroup = EditorGUILayout.Foldout(wallGroup, "Wall Prefab List");
        if (wallGroup)
        {
            EditorGUI.indentLevel += 2;
            wallArraySize = EditorGUILayout.IntField("How many walls?", wallArraySize);
            if (Wall != null && wallArraySize != Wall.Length)
                Wall = new GameObject[wallArraySize];
            for (int i = 0; i < wallArraySize; i++)
            {
                Wall[i] = EditorGUILayout.ObjectField("Wall " + i.ToString(), Wall[i], typeof(GameObject), false) as GameObject;
            }
            EditorGUI.indentLevel = 0;
        }

        floorGroup = EditorGUILayout.Foldout(floorGroup, "Wall Prefab List");
        if (floorGroup)
        {
            EditorGUI.indentLevel += 2;
            floorArraySize = EditorGUILayout.IntField("How many floors?", floorArraySize);
            if (Floor != null && floorArraySize != Floor.Length)
                Floor = new GameObject[floorArraySize];
            for (int i = 0; i < floorArraySize; i++)
            {
                Floor[i] = EditorGUILayout.ObjectField("Floor " + i.ToString(), Floor[i], typeof(GameObject), false) as GameObject;
            }
            EditorGUI.indentLevel = 0;
        }

        EditorGUILayout.Space(15, true);

        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
        GUILayout.Label("Maze Size Parameters");
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);

        EditorGUILayout.Space(15, true);

        row = EditorGUILayout.IntField("Maze Nr. rows", row);
        column = EditorGUILayout.IntField("Maze Nr. columns", column);

        EditorGUILayout.Space(10, true);

        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
        if (GUILayout.Button("Generate Maze")) BuildMaze();
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);

    }
}
