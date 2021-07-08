#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesProcessor
    {
        static AssetBundlesProcessor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (AssetBundlesBuilder.CheckAssetBundlesBuilt() || !AssetBundlesBuilder.CheckAssetBundlesExist())
                return;
            
            Debug.Log("AssetBundles: Bundles not built. Building...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode || !AssetBundlesAutoRebuild.GetEnabled() || AssetBundlesBuilder.CheckAssetBundles() || !AssetBundlesBuilder.CheckAssetBundlesExist()) 
                return;
            Debug.Log("AssetBundles: Bundles inconsistency found. Rebuilding...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
        }
    }
}

#endif