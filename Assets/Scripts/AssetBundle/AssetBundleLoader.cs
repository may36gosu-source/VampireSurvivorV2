using System.IO;
using UnityEngine;

using VampireSurvivors.Common;
namespace VampireSurvivors.Logic{

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

        private void InitializeRuntime(GameObject instance)
        {
            RuntimeContext ct = new RuntimeContext();
            RuntimeBinder[] elements = instance.GetComponentsInChildren<RuntimeBinder>(true);

            if(elements == null)
            {
                return;
            }
            foreach (RuntimeBinder binder in elements)
            {
                binder.Initialize(ct);
            }
        }

        private string GetBundlePath()
        {
        #if UNITY_ANDROID
            return "AssetBundles/Android/models.unity3d";
        #else
            return "AssetBundles/Windows/models.unity3d";
        #endif
        }

        private string GetBundlePath(string bundleName)
        {
            #if UNITY_ANDROID
                return $"AssetBundles/Android/{bundleName}";
            #else
                return $"AssetBundles/Windows/{bundleName}";
            #endif
        }

        private AssetBundle LoadBundle(string bundleName)
        {
         
            bool cacheBundle = AssetBundleCache.TryGet(bundleName, out AssetBundle bundle);

            if(cacheBundle)
            {
                return bundle;
            }
          
            string bundlePath = GetBundlePath(bundleName);

            Debug.Log($"[AssetBundleLoader] " + $"Load bundle:\n{bundlePath}");

            if (!BetterStreamingAssets.FileExists(bundlePath))
            {
                Debug.LogError( "[AssetBundleLoader] " + $"Không tìm thấy AssetBundle:\n{bundlePath}");
                return null;
            }

            AssetBundle loadedBundle = BetterStreamingAssets.LoadAssetBundle(bundlePath);

            if (loadedBundle == null)
            {
                Debug.LogError($"[AssetBundleLoader] " + $"Load AssetBundle thất bại:\n" + $"{bundlePath}");
                return null;
            }

            AssetBundleCache.Add(bundleName, loadedBundle); // add vào cache

            return loadedBundle;

        }

        private GameObject LoadPrefab(string bundleName, string prefabName) {
            AssetBundle bundle =  LoadBundle(bundleName);

            if(!bundle) {
                return null;
            }

            string findAsset = FindAssetPath(bundle, prefabName);

            if(string.IsNullOrEmpty(findAsset))
                return null;

            
            GameObject prefab = bundle.LoadAsset<GameObject>(findAsset);

            if (prefab == null)
            {
                Debug.LogError( $"[AssetBundleLoader] Không load được prefab:\n{findAsset}");

                return null;
            }

            return prefab;

        }

        private string FindAssetPath(AssetBundle assetBundle, string prefabName) {
            string[] assetNames = assetBundle.GetAllAssetNames();

            Debug.Log($"[AssetBundleLoader] " + $"Asset count: {assetNames.Length}");

            // -----------------------------------------
            // Find Player prefab
            // -----------------------------------------


            foreach (string assetPath in assetNames)
            {
                string fileName = Path.GetFileNameWithoutExtension(assetPath);

                if (string.Equals(fileName, prefabName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return assetPath;
                }
            }
            return null;
        }

        private void LoadPlayer()
        {
            // string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles", "Windows", BundleName);
            // string bundlePath = GetBundlePath();

            // Debug.Log($"[AssetBundleLoader] " + $"Load bundle:\n{bundlePath}");

            // Debug.Log("[AssetBundleLoader] " + $"Load bundle:\n{bundlePath}");

            // if (!BetterStreamingAssets.FileExists(bundlePath))
            // {
            //     Debug.LogError( "[AssetBundleLoader] " + $"Không tìm thấy AssetBundle:\n{bundlePath}");

            //     return;
            // }

            // // -----------------------------------------
            // // Load AssetBundle
            // // -----------------------------------------

            // assetBundle = BetterStreamingAssets.LoadAssetBundle(bundlePath);
            // // assetBundle = AssetBundle.LoadFromFile(bundlePath);

            // if (assetBundle == null)
            // {
            //     Debug.LogError($"[AssetBundleLoader] " + $"Load AssetBundle thất bại:\n" + $"{bundlePath}");

            //     return;
            // }

            // Debug.Log($"[AssetBundleLoader] " + $"Loaded: {BundleName}");

            // // -----------------------------------------
            // // Debug asset list
            // // -----------------------------------------

            // string[] assetNames = assetBundle.GetAllAssetNames();

            // Debug.Log($"[AssetBundleLoader] " + $"Asset count: {assetNames.Length}");

            // foreach (string assetName in assetNames)
            // {
            //     Debug.Log($"[AssetBundleLoader] Asset: " + $"{assetName}");
            // }

            // // -----------------------------------------
            // // Find Player prefab
            // // -----------------------------------------

            // string playerAssetPath = null;

            // foreach (string assetName in assetNames)
            // {
            //     string fileName = Path.GetFileNameWithoutExtension(assetName);

            //     if (string.Equals(fileName, PlayerPrefabName,System.StringComparison.OrdinalIgnoreCase))
            //     {
            //         playerAssetPath = assetName;
            //         break;
            //     }
            // }

            // if (string.IsNullOrEmpty(playerAssetPath))
            // {
            //     Debug.LogError($"[AssetBundleLoader] " + $"Không tìm thấy prefab:\n" + $"{PlayerPrefabName}");

            //     return;
            // }

            // // -----------------------------------------
            // // Load prefab
            // // -----------------------------------------

            // GameObject playerPrefab = assetBundle.LoadAsset<GameObject>(playerAssetPath);

            // if (playerPrefab == null)
            // {
            //     Debug.LogError($"[AssetBundleLoader] " + $"Không load được prefab:\n" + $"{playerAssetPath}");

            //     return;
            // }

            // Debug.Log($"[AssetBundleLoader] " + $"Prefab loaded: {playerPrefab.name}");

            // -----------------------------------------
            // Spawn
            // -----------------------------------------

            GameObject playerPrefab = LoadPrefab(BundleName, PlayerPrefabName);

            if(playerPrefab == null)
                return;

            Transform spawn = spawnPoint != null ? spawnPoint : transform;

            GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation, transform.parent);

            InitializeRuntime(player);

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
}