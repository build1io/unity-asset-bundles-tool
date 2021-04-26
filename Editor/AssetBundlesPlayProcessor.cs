#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.AssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesPlayProcessor
    {
        static AssetBundlesPlayProcessor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
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