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

    private string bundleName = "models.unity3d";

    private string sourceFolder = "Assets/GameRes/Prefabs/Models";

    private string buildList;

    [MenuItem("Tools/AssetBundle/Builder")]
    private static void Open()
    {
        GetWindow<AssetBundleBuilder>("AssetBundle Builder");
    }

    private void OnEnable()
    {
        UpdateBuildListPath();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("AssetBundle Builder", EditorStyles.boldLabel);

        EditorGUILayout.Space(10);

        TargetPlatform newPlatform = (TargetPlatform)EditorGUILayout.EnumPopup("Target Platform", targetPlatform);

        if (newPlatform != targetPlatform)
        {
            targetPlatform = newPlatform;
            UpdateBuildListPath();
        }

        bundleName = EditorGUILayout.TextField("Bundle Name", bundleName);

        EditorGUILayout.BeginHorizontal();

        sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);

        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            SelectSourceFolder();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Build List", buildList);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Output", GetOutputFolder());

        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox("Nếu build_list.txt chưa tồn tại, " + "Builder sẽ tự scan Source Folder " + "và tạo file trước khi build.", MessageType.Info);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Build AssetBundles", GUILayout.Height(40)))
        {
            Build();
        }
    }

    private string GetPlatformFolder()
    {
        return targetPlatform == TargetPlatform.Android ? "Android" : "Windows";
    }

    private void UpdateBuildListPath()
    {
        buildList = Path.Combine("Assets", "GameRes", "AssetBundle", GetPlatformFolder(), "build_list.txt").Replace("\\", "/");
    }

    private string GetOutputFolder()
    {
        return Path.Combine("Assets", "StreamingAssets", "AssetBundles", GetPlatformFolder()).Replace("\\", "/");
    }

    private BuildTarget GetBuildTarget()
    {
        return targetPlatform == TargetPlatform.Android ? BuildTarget.Android : BuildTarget.StandaloneWindows64;
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

    private bool EnsureBuildList()
    {
        Debug.Log("[AssetBundleBuilder] " + "Generating build list from Source Folder...");

        AssetBundleResourceExporter.ExportBuildList(
            bundleName,
            sourceFolder,
            buildList
        );

        AssetDatabase.Refresh();

        if (!File.Exists(buildList))
        {
            Debug.LogError("[AssetBundleBuilder] " + "Failed to generate build list:\n" + buildList);

            return false;
        }

        Debug.Log("[AssetBundleBuilder] " + $"Build list updated:\n{buildList}"
        );

        return true;
    }

    private void Build()
    {
        // --------------------------------------------------
        // 1. Đảm bảo build_list.txt tồn tại
        // --------------------------------------------------

        if (!EnsureBuildList())
        {
            EditorUtility.DisplayDialog("Build Failed", "Không thể tạo build_list.txt.\n\n" + $"Source:\n{sourceFolder}\n\n" + $"Output:\n{buildList}", "OK");

            return;
        }

        // --------------------------------------------------
        // 2. Đọc build list
        // --------------------------------------------------

        List<AssetBundleBuild> builds = ReadBuildList();

        if (builds.Count == 0)
        {
            EditorUtility.DisplayDialog("Build Failed", "Build list không có resource hợp lệ.", "OK");

            return;
        }

        // --------------------------------------------------
        // 3. Chuẩn bị output
        // --------------------------------------------------

        string outputFolder = GetOutputFolder();

        if (Directory.Exists(outputFolder))
        {
            Directory.Delete(outputFolder, true);
        }

        Directory.CreateDirectory(outputFolder);

        // --------------------------------------------------
        // 4. Build AssetBundle
        // --------------------------------------------------

        BuildTarget buildTarget = GetBuildTarget();

        Debug.Log($"[AssetBundleBuilder]\n" + $"Target: {buildTarget}\n" + $"Build List: {buildList}\n" + $"Output: {outputFolder}");

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputFolder, builds.ToArray(), BuildAssetBundleOptions.None, buildTarget);

        if (manifest == null)
        {
            Debug.LogError("[AssetBundleBuilder] " + "Build AssetBundle FAILED.");

            EditorUtility.DisplayDialog("Build Failed", $"AssetBundle build thất bại.\n\n" + $"Target: {buildTarget}", "OK");

            return;
        }

        // --------------------------------------------------
        // 5. Done
        // --------------------------------------------------

        AssetDatabase.Refresh();

        Debug.Log("[AssetBundleBuilder] " + "Build completed successfully.");

        EditorUtility.DisplayDialog("Build Complete", $"AssetBundle build thành công!\n\n" + $"Target: {buildTarget}\n\n" + $"Output:\n{outputFolder}", "OK");
    }

    private List<AssetBundleBuild> ReadBuildList()
    {
        Dictionary<string, List<string>> bundleAssets = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        string[] lines = File.ReadAllLines(buildList);

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith("#"))
                continue;

            string[] parts = line.Split('|');

            if (parts.Length != 2)
            {
                Debug.LogWarning($"[AssetBundleBuilder] " + $"Invalid line:\n{line}");

                continue;
            }

            string currentBundleName = parts[0].Trim();

            string assetPath = parts[1].Trim();

            if (string.IsNullOrEmpty(currentBundleName))
            {
                continue;
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                continue;
            }

            // ----------------------------------------------
            // Check bằng Unity AssetDatabase
            // ----------------------------------------------

            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);

            if (asset == null)
            {
                Debug.LogWarning($"[AssetBundleBuilder] " + $"Unity asset không tồn tại:\n" + assetPath);

                continue;
            }

            if (!bundleAssets.TryGetValue(currentBundleName, out List<string> assets))
            {
                assets = new List<string>();

                bundleAssets.Add(currentBundleName, assets);
            }

            if (!assets.Contains(assetPath))
            {
                assets.Add(assetPath);
            }
        }

        List<AssetBundleBuild> result = new List<AssetBundleBuild>();

        foreach (KeyValuePair<string, List<string>> pair in bundleAssets)
        {
            result.Add(new AssetBundleBuild { assetBundleName = pair.Key, assetNames = pair.Value.ToArray() });
        }

        return result;
    }
}