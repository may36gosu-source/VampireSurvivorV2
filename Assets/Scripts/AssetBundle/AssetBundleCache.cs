using System.Collections.Generic;
using UnityEngine;
namespace VampireSurvivors.Logic
{
	public static class AssetBundleCache
    {
        private static readonly Dictionary<string, AssetBundle>  bundles = new();

        // public Dictionary<string, AssetBundle> GetBundles => bundles;

        public static void Add(string bundleName, AssetBundle bundle)
        {
            bundles[bundleName] = bundle;
        }

        public static bool TryGet(string bundleName, out AssetBundle bundle)
        {
            return bundles.TryGetValue(bundleName, out bundle);
        }

        public static void Remove(string bundleName)
        {
            if (!TryGet(bundleName, out AssetBundle bundle))
                return;

            bundles.Remove(bundleName);

            if (bundle != null)
                bundle.Unload(false);
        }

        public static void Clear()
        {
            foreach (AssetBundle bundle in bundles.Values)
            {
                if (bundle != null)
                    bundle.Unload(false);
            }

            bundles.Clear();
        }
    }
}