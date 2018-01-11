using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetBundleManager {
    public static Dictionary<string, AssetBundle> LoadedAssetBundles = new Dictionary<string, AssetBundle>();

    public static void AddAssetBundle(string key, AssetBundle bundle)
    {
        if (LoadedAssetBundles.ContainsKey(key))
            return;
        LoadedAssetBundles.Add(key, bundle);
    }

    public static AssetBundle GetAssetBundle(string bundleName)
    {
        AssetBundle ab;
        if (LoadedAssetBundles.TryGetValue(bundleName, out ab))
            return ab;
        else
            return null;
    }

    public static void Unload(string bundleName, bool allObjects)
    {
        AssetBundle ab;
        if (LoadedAssetBundles.TryGetValue(bundleName, out ab))
        {
            ab.Unload(allObjects);
            ab = null;
            LoadedAssetBundles.Remove(bundleName);
        }
    }
}
