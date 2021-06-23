#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.AssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesProcessor
    {
        static AssetBundlesProcessor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (AssetBundlesBuilder.CheckAssetBundlesBuilt())
                return;
            
            Debug.Log("AssetBundles: Bundles not built. Building...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode || !AssetBundlesAutoRebuild.GetEnabled() || AssetBundlesBuilder.CheckAssetBundles()) 
                return;
            Debug.Log("AssetBundles: Bundles inconsistency found. Rebuilding...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
        }
    }
}

#endif