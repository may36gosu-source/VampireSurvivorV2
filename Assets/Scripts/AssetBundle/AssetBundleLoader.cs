using System.Collections.Generic;
using UnityEngine;

using VampireSurvivors.Common;

namespace VampireSurvivors.Logic
{
    public class AssetBundleLoader : MonoBehaviour
    {
        private const string PlayerBundlePath =
            "gameres/prefabs/models/player-urp variant.unity3d";

        [SerializeField]
        private Transform spawnPoint;

        private AssetBundleManifest manifest;

        // Tạm thời giữ tất cả bundle đã load trong memory.
        // Chưa làm cache/ref-count ở bước test này.
        private readonly Dictionary<string, AssetBundle> loadedBundles =
            new Dictionary<string, AssetBundle>();

        private void Start()
        {
            if (!LoadManifest())
            {
                Debug.LogError(
                    "[AssetBundleLoader] Không load được AssetBundleManifest."
                );

                return;
            }

            LoadPlayer();
        }

        // ============================================================
        // Manifest
        // ============================================================

        private bool LoadManifest()
        {
#if UNITY_ANDROID
            string manifestPath = "AssetBundles/Android/Android";
#else
            string manifestPath = "AssetBundles/Windows/Windows";
#endif

            Debug.Log(
                $"[AssetBundleLoader] Load manifest:\n{manifestPath}"
            );

            if (!BetterStreamingAssets.FileExists(manifestPath))
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Không tìm thấy manifest:\n{manifestPath}"
                );

                return false;
            }

            AssetBundle manifestBundle =
                BetterStreamingAssets.LoadAssetBundle(manifestPath);

            if (manifestBundle == null)
            {
                Debug.LogError(
                    "[AssetBundleLoader] Load manifest bundle thất bại."
                );

                return false;
            }

            manifest =
                manifestBundle.LoadAsset<AssetBundleManifest>(
                    "AssetBundleManifest"
                );

            if (manifest == null)
            {
                Debug.LogError(
                    "[AssetBundleLoader] Không lấy được AssetBundleManifest."
                );

                manifestBundle.Unload(false);

                return false;
            }

            Debug.Log(
                "[AssetBundleLoader] AssetBundleManifest loaded."
            );

            return true;
        }

        // ============================================================
        // Path
        // ============================================================

        private string GetBundlePath(string relativePath)
        {
#if UNITY_ANDROID
            return $"AssetBundles/Android/{relativePath}";
#else
            return $"AssetBundles/Windows/{relativePath}";
#endif
        }

        // ============================================================
        // Load Bundle
        // ============================================================

        private AssetBundle LoadBundle(string relativePath)
        {
            if (loadedBundles.TryGetValue(
                    relativePath,
                    out AssetBundle cachedBundle))
            {
                Debug.Log(
                    $"[AssetBundleLoader] Bundle already loaded:\n{relativePath}"
                );

                return cachedBundle;
            }

            string bundlePath = GetBundlePath(relativePath);

            Debug.Log(
                $"[AssetBundleLoader] Load bundle:\n{bundlePath}"
            );

            if (!BetterStreamingAssets.FileExists(bundlePath))
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Không tìm thấy AssetBundle:\n{bundlePath}"
                );

                return null;
            }

            AssetBundle loadedBundle =
                BetterStreamingAssets.LoadAssetBundle(bundlePath);

            if (loadedBundle == null)
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Load AssetBundle thất bại:\n{bundlePath}"
                );

                return null;
            }

            loadedBundles.Add(relativePath, loadedBundle);

            return loadedBundle;
        }

        // ============================================================
        // Load Dependency
        // ============================================================

        private bool LoadDependencies(string bundleName)
        {
            if (manifest == null)
            {
                Debug.LogError(
                    "[AssetBundleLoader] Manifest chưa được load."
                );

                return false;
            }

            string[] dependencies =
                manifest.GetAllDependencies(bundleName);

            Debug.Log(
                $"[AssetBundleLoader] Dependency count: " +
                $"{dependencies.Length}\n" +
                $"Bundle: {bundleName}"
            );

            foreach (string dependency in dependencies)
            {
                Debug.Log(
                    $"[AssetBundleLoader] Dependency:\n{dependency}"
                );

                AssetBundle dependencyBundle =
                    LoadBundle(dependency);

                if (dependencyBundle == null)
                {
                    Debug.LogError(
                        $"[AssetBundleLoader] Không load được dependency:\n" +
                        $"{dependency}"
                    );

                    return false;
                }
            }

            return true;
        }

        // ============================================================
        // Load Prefab
        // ============================================================

        public GameObject LoadPrefab(string bundlePath)
        {
            // --------------------------------------------------------
            // 1. Load dependency trước
            // --------------------------------------------------------

            if (!LoadDependencies(bundlePath))
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Load dependencies thất bại:\n" +
                    $"{bundlePath}"
                );

                return null;
            }

            // --------------------------------------------------------
            // 2. Load bundle chính
            // --------------------------------------------------------

            AssetBundle bundle =
                LoadBundle(bundlePath);

            if (bundle == null)
                return null;

            // --------------------------------------------------------
            // 3. Lấy asset trong bundle
            // --------------------------------------------------------

            string[] assetNames =
                bundle.GetAllAssetNames();

            if (assetNames.Length == 0)
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Bundle không chứa asset:\n" +
                    $"{bundlePath}"
                );

                return null;
            }

            foreach (string assetName in assetNames)
            {
                Debug.Log(
                    $"[AssetBundleLoader] Asset:\n{assetName}"
                );
            }

            string assetPath = assetNames[0];

            Debug.Log(
                $"[AssetBundleLoader] Load asset:\n{assetPath}"
            );

            GameObject prefab =
                bundle.LoadAsset<GameObject>(assetPath);

            if (prefab == null)
            {
                Debug.LogError(
                    $"[AssetBundleLoader] Không load được prefab:\n" +
                    $"{assetPath}"
                );

                return null;
            }

            Debug.Log(
                $"[AssetBundleLoader] Prefab loaded:\n{prefab.name}"
            );

            return prefab;
        }

        // ============================================================
        // Runtime Initialize
        // ============================================================

        private void InitializeRuntime(GameObject instance)
        {
            RuntimeContext ct = new RuntimeContext();

            RuntimeBinder[] elements =
                instance.GetComponentsInChildren<RuntimeBinder>(true);

            if (elements == null)
                return;

            foreach (RuntimeBinder binder in elements)
            {
                binder.Initialize(ct);
            }
        }

        // ============================================================
        // Player
        // ============================================================

        private void LoadPlayer()
        {
            GameObject playerPrefab =
                LoadPrefab(PlayerBundlePath);

            if (playerPrefab == null)
                return;

            Transform spawn =
                spawnPoint != null
                    ? spawnPoint
                    : transform;

            GameObject player =
                Instantiate(
                    playerPrefab,
                    spawn.position,
                    spawn.rotation,
                    transform.parent
                );

            InitializeRuntime(player);

            player.name = playerPrefab.name;

            Debug.Log(
                $"[AssetBundleLoader] Player spawned: {player.name}"
            );
        }

        // ============================================================
        // Cleanup
        // ============================================================

        private void OnDestroy()
        {
            foreach (
                KeyValuePair<string, AssetBundle> pair
                in loadedBundles)
            {
                if (pair.Value != null)
                {
                    pair.Value.Unload(false);
                }
            }

            loadedBundles.Clear();
        }
    }
}