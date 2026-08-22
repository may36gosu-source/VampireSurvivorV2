using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
public class AssetBundleResourceScanner
{

	private readonly BuildTarget buildTarget;


	public AssetBundleResourceScanner(BuildTarget buildTarget){
	    this.buildTarget = buildTarget;
	}

	private string sourceFolder = "Assets/GameRes";

	private string streamAssetPrefixFolder = "Assets/StreamingAssets/AssetBundles/";

	private readonly List<string> assetPaths = new List<string>();

	#region Scan
		// scan folder cần xuất asset bundle
	#endregion
	private List<string> Scan() {

		List<string> assetPaths = new List<string>();

		if(!AssetDatabase.IsValidFolder(sourceFolder))
			return assetPaths;

		string[] guids = AssetDatabase.FindAssets("", new[] { sourceFolder });

		foreach(string guid in guids) {

			string assetPath = AssetDatabase.GUIDToAssetPath(guid);

			if(AssetDatabase.IsValidFolder(assetPath))
				continue;

			assetPaths.Add(assetPath);

		}

		assetPaths.Sort();

		return assetPaths;
	}

	
	#region GetBundlePath
		// Bundle Path
	#endregion
	private string GetBundlePath(string assetPath)
	{
	    string subResult = assetPath.Substring("Assets/".Length);
	    return Path.ChangeExtension(subResult, ".unity3d");
	}

	private List<AssetBundleBuild> CreateBuilds(List<string> assetPaths)
	{
		List<AssetBundleBuild> result = new List<AssetBundleBuild>();

		foreach(string asset in assetPaths) {

			string bundlePath = GetBundlePath(asset);

			result.Add(new AssetBundleBuild { assetBundleName = bundlePath, assetNames = new[] { asset} });
		}

		return result;
	}


	private void LogBuilds(List<AssetBundleBuild> builds)
	{
		foreach(AssetBundleBuild build in builds) {

			Debug.Log($"AssetBundle: {build.assetBundleName}");

		    foreach (string asset in build.assetNames)
		    {
		        Debug.Log($"  Asset: {asset}");
		    }
		}
	}

	private string GetOutputFolder()
	{
	    string platform = buildTarget == BuildTarget.Android ? "Android" : "Windows";

	    return Path.Combine(streamAssetPrefixFolder, platform).Replace("\\", "/");
	}

	private string GetOutputFile()
    {
    	string platform = buildTarget == BuildTarget.Android ? "Android" : "Windows";

        return Path.Combine("Assets","StreamingAssets", "AssetBundles", platform, "build_list.txt").Replace("\\", "/");
    }

	public void Build()
	{
		List<string> assetPaths = Scan();

		if(assetPaths.Count == 0)
			return;

		List<AssetBundleBuild> builds = CreateBuilds(assetPaths);

		if(builds.Count == 0)
			return;


		string outputFolder = GetOutputFolder();

		if (Directory.Exists(outputFolder))
        {
            Directory.Delete(outputFolder, true);
        }

        Directory.CreateDirectory(outputFolder);


        Debug.Log($"[AssetBundleBuilder]\n" + $"Target: {buildTarget}\n"  + $"Output: {outputFolder}");

        AssetBundleManifest manifest  = BuildPipeline.BuildAssetBundles( outputFolder, builds.ToArray(), BuildAssetBundleOptions.None, buildTarget);

        string playerBundle =
		    GetBundlePath(
		        "Assets/GameRes/Prefabs/Models/Player-URP Variant.prefab"
		    );

		string[] dependencies =
		    manifest.GetAllDependencies(playerBundle);

		Debug.Log(
		    $"[Dependency] Bundle: {playerBundle}"
		);

		foreach (string dependency in dependencies)
		{
		    Debug.Log(
		        $"[Dependency] -> {dependency}"
		    );
		}
        
        if (manifest == null)
        {
            Debug.LogError("[AssetBundleBuilder] " + "Build AssetBundle FAILED.");
            return;
        }

        // Unity build thành công nhưng đã lowercase tên bundle.
	    // Restore lại cấu trúc + casing giống Asset Path.
	    // RestoreBundlePaths(assetPaths, outputFolder);

        AssetDatabase.Refresh();

        WriteFileBuildList(sourceFolder, GetOutputFile());

        Debug.Log("[AssetBundleBuilder] " + "Build completed successfully.");
	}

	// đưa folder và assetbundle file về nguyên bản name trong unity để dễ xử lý:

	private void RestoreBundlePaths(List<string> assetPaths, string outputFolder)
	{
	    string tempFolder = outputFolder + "_Temp";

	    if (Directory.Exists(tempFolder))
	    {
	        Directory.Delete(tempFolder, true);
	    }

	    Directory.CreateDirectory(tempFolder);

	    foreach (string assetPath in assetPaths)
	    {
	        string bundlePath = GetBundlePath(assetPath);

	        string sourcePath = Path.Combine( outputFolder, bundlePath.ToLowerInvariant()).Replace("\\", "/");

	        string targetPath = Path.Combine( tempFolder,  bundlePath).Replace("\\", "/");

	        if (!File.Exists(sourcePath))
	        {
	            Debug.LogWarning( $"[AssetBundleBuilder] Bundle not found:\n{sourcePath}" );

	            continue;
	        }

	        string targetDirectory = Path.GetDirectoryName(targetPath);

	        if (!string.IsNullOrEmpty(targetDirectory))
	        {
	            Directory.CreateDirectory(targetDirectory);
	        }

	        File.Move(sourcePath, targetPath);
	    }

	    // Xóa output lowercase cũ.
	    Directory.Delete(outputFolder, true);

	    // Tạo lại output folder.
	    Directory.CreateDirectory(outputFolder);

	    // Đưa bundle đã restore casing về output thật.
	    foreach (string file in Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories))
	    {
	        string relativePath = file.Substring(tempFolder.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

	        string destination = Path.Combine( outputFolder, relativePath );

	        string destinationDirectory = Path.GetDirectoryName(destination);

	        if (!string.IsNullOrEmpty(destinationDirectory))
	        {
	            Directory.CreateDirectory(destinationDirectory);
	        }

	        File.Move(file, destination);
	    }

	    Directory.Delete(tempFolder, true);
	}

	public void WriteFileBuildList(string sourceFolder, string outputFile)
    {

        if (!AssetDatabase.IsValidFolder(sourceFolder))
        {
            Debug.LogError("[AssetBundleResourceExporter] " + $"Invalid source folder:\n{sourceFolder}");

            return;
        }

        List<string> assets = Scan();

        string directory = Path.GetDirectoryName(outputFile);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringBuilder builder = new StringBuilder();

        foreach (string assetPath in assets)
        {
            string bundlePath = GetBundlePath(assetPath);

        	builder.AppendLine(bundlePath);
        }

        File.WriteAllText(outputFile, builder.ToString(), new UTF8Encoding(false));

        Debug.Log( "[AssetBundleResourceExporter]\n"  + $"Source: {sourceFolder}\n" 
            + $"Assets: {assets.Count}\n" + $"Output: {outputFile}"
        );
    }
}