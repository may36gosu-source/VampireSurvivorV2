using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetBundleResourceExporter : EditorWindow
{
    public enum TargetPlatform
    {
        Windows,
        Android
    }

    private TargetPlatform targetPlatform =
        TargetPlatform.Windows;

    private string bundleName = "models.unity3d";

    private string sourceFolder = "Assets/GameRes/Prefabs/Models";

    private readonly List<string> assetPaths = new List<string>();

    [MenuItem("Tools/AssetBundle/Resource Exporter")]
    private static void Open()
    {
        GetWindow<AssetBundleResourceExporter>("AssetBundle Resource Exporter");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("AssetBundle Resource Exporter", EditorStyles.boldLabel);

        EditorGUILayout.Space(10);

        targetPlatform = (TargetPlatform)EditorGUILayout.EnumPopup("Target Platform", targetPlatform);

        bundleName = EditorGUILayout.TextField("Bundle Name", bundleName);

        EditorGUILayout.BeginHorizontal();

        sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);

        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            SelectSourceFolder();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Output File", GetOutputFile());

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Scan Resources", GUILayout.Height(30)))
        {
            ScanResources();
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField($"Found: {assetPaths.Count}");

        EditorGUILayout.Space(5);

        foreach (string assetPath in assetPaths)
        {
            EditorGUILayout.LabelField(assetPath);
        }

        EditorGUILayout.Space(10);

        GUI.enabled = assetPaths.Count > 0 && !string.IsNullOrWhiteSpace(bundleName);

        if (GUILayout.Button("Export build_list.txt", GUILayout.Height(35)))
        {
            Export();
        }

        GUI.enabled = true;
    }

    private static string NormalizeBundleName(string name)
    {
        name = name.Trim();

        if (!name.EndsWith(".unity3d", System.StringComparison.OrdinalIgnoreCase))
        {
            name += ".unity3d";
        }

        return name;
    }

    private string GetPlatformFolder()
    {
        return targetPlatform == TargetPlatform.Android ? "Android" : "Windows";
    }

    private string GetOutputFile()
    {
        return Path.Combine("Assets", "GameRes", "AssetBundle", GetPlatformFolder(), "build_list.txt").Replace("\\", "/");
    }

    private void SelectSourceFolder()
    {
        string selectedPath = EditorUtility.OpenFolderPanel("Select Asset Folder", Application.dataPath, "");

        if (string.IsNullOrEmpty(selectedPath))
            return;

        selectedPath = selectedPath.Replace("\\", "/");

        string projectPath = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");

        if (!selectedPath.StartsWith(projectPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder", "Folder phải nằm trong Unity project.", "OK");

            return;
        }

        sourceFolder = selectedPath.Substring(projectPath.Length + 1);
    }

    private void ScanResources()
    {
        assetPaths.Clear();

        ScanFolder(sourceFolder, assetPaths);

        assetPaths.Sort();

        Debug.Log($"[AssetBundleResourceExporter] " + $"Found {assetPaths.Count} assets.");
    }

    private static void ScanFolder(string folder, List<string> result)
    {
        if (!AssetDatabase.IsValidFolder(folder))
            return;

        string[] guids = AssetDatabase.FindAssets( "", new[] { folder });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath))
                continue;

            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);

            if (asset == null)
                continue;

            result.Add(assetPath);
        }
    }

    private void Export()
    {
        bundleName = NormalizeBundleName(bundleName);

        ExportBuildList(bundleName, sourceFolder, GetOutputFile());

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Export Complete", $"Exported {assetPaths.Count} assets.\n\n" + $"Bundle: {bundleName}\n\n" 
            + $"Output:\n{GetOutputFile()}", "OK"
        );
    }

    public static void ExportBuildList(string bundleName, string sourceFolder, string outputFile)
    {
        bundleName = NormalizeBundleName(bundleName);

        if (!AssetDatabase.IsValidFolder(sourceFolder))
        {
            Debug.LogError("[AssetBundleResourceExporter] " + $"Invalid source folder:\n{sourceFolder}");

            return;
        }

        List<string> assets = new List<string>();

        ScanFolder(sourceFolder, assets);

        assets.Sort();

        string directory = Path.GetDirectoryName(outputFile);

        if (!string.IsNullOrEmpty(directory) &&
            !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringBuilder builder = new StringBuilder();

        foreach (string assetPath in assets)
        {
            builder.Append(bundleName);
            builder.Append("|");
            builder.Append(assetPath);
            builder.AppendLine();
        }

        File.WriteAllText(outputFile, builder.ToString(), new UTF8Encoding(false));

        Debug.Log( "[AssetBundleResourceExporter]\n" + $"Bundle: {bundleName}\n" + $"Source: {sourceFolder}\n" 
            + $"Assets: {assets.Count}\n" + $"Output: {outputFile}"
        );
    }
}