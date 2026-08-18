using System.IO;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    private const string BundleName = "models.unity3d";

    private const string PlayerPrefabName = "Player-URP Variant";

    [SerializeField]
    private Transform spawnPoint;

    private AssetBundle assetBundle;

    private void Start()
    {
        LoadPlayer();
    }

    private void LoadPlayer()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles", "Windows", BundleName);

        Debug.Log($"[AssetBundleLoader] " + $"Load bundle:\n{bundlePath}");

        // -----------------------------------------
        // Check file
        // -----------------------------------------

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"[AssetBundleLoader] " + $"Không tìm thấy AssetBundle:\n" + $"{bundlePath}");

            return;
        }

        // -----------------------------------------
        // Load AssetBundle
        // -----------------------------------------

        assetBundle = AssetBundle.LoadFromFile(bundlePath);

        if (assetBundle == null)
        {
            Debug.LogError($"[AssetBundleLoader] " + $"Load AssetBundle thất bại:\n" + $"{bundlePath}");

            return;
        }

        Debug.Log($"[AssetBundleLoader] " + $"Loaded: {BundleName}");

        // -----------------------------------------
        // Debug asset list
        // -----------------------------------------

        string[] assetNames = assetBundle.GetAllAssetNames();

        Debug.Log($"[AssetBundleLoader] " + $"Asset count: {assetNames.Length}");

        foreach (string assetName in assetNames)
        {
            Debug.Log($"[AssetBundleLoader] Asset: " + $"{assetName}");
        }

        // -----------------------------------------
        // Find Player prefab
        // -----------------------------------------

        string playerAssetPath = null;

        foreach (string assetName in assetNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(assetName);

            if (string.Equals(fileName, PlayerPrefabName,System.StringComparison.OrdinalIgnoreCase))
            {
                playerAssetPath = assetName;
                break;
            }
        }

        if (string.IsNullOrEmpty(playerAssetPath))
        {
            Debug.LogError($"[AssetBundleLoader] " + $"Không tìm thấy prefab:\n" + $"{PlayerPrefabName}");

            return;
        }

        // -----------------------------------------
        // Load prefab
        // -----------------------------------------

        GameObject playerPrefab = assetBundle.LoadAsset<GameObject>(playerAssetPath);

        if (playerPrefab == null)
        {
            Debug.LogError($"[AssetBundleLoader] " + $"Không load được prefab:\n" + $"{playerAssetPath}");

            return;
        }

        Debug.Log($"[AssetBundleLoader] " + $"Prefab loaded: {playerPrefab.name}");

        // -----------------------------------------
        // Spawn
        // -----------------------------------------

        Transform spawn = spawnPoint != null ? spawnPoint : transform;

        GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation, transform.parent);

        player.name = PlayerPrefabName;

        Debug.Log( $"[AssetBundleLoader] " +$"Player spawned: {player.name}");
    }

    private void OnDestroy()
    {
        if (assetBundle != null)
        {
            assetBundle.Unload(false);
            assetBundle = null;
        }
    }
}