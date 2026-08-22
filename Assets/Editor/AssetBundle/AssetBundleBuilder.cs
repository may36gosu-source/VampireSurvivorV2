using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder : EditorWindow
{
    private enum TargetPlatform
    {
        Windows,
        Android
    }

    private TargetPlatform targetPlatform = TargetPlatform.Windows;

    [MenuItem("Tools/AssetBundle/Builder")]
    private static void Open()
    {
        GetWindow<AssetBundleBuilder>("AssetBundle Builder");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "AssetBundle Resource Exporter",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space(10);

        targetPlatform =
            (TargetPlatform)EditorGUILayout.EnumPopup(
                "Target Platform",
                targetPlatform
            );

        if (GUILayout.Button("Export", GUILayout.Height(35)))
        {
            Export();
        }
    }

    private void Export()
    {
        BuildTarget buildTarget = targetPlatform == TargetPlatform.Android ? BuildTarget.Android : BuildTarget.StandaloneWindows64;

        AssetBundleResourceScanner scanner = new AssetBundleResourceScanner(buildTarget);

        scanner.Build();
    }
}