#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesProcessor
    {
        public const string AutoRebuildKey = "Build1_AssetBundlesTool_AutoRebuildEnabled";
    
        static AssetBundlesProcessor()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);
            
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (AssetBundlesBuilder.CheckAssetBundlesBuilt() || !AssetBundlesBuilder.CheckAssetBundlesExist())
                return;
            
            Debug.Log("AssetBundles: Bundles not built. Building...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }

        /*
         * Public.
         */

        public static bool GetEnabled()
        {
            return EditorPrefs.GetBool(AutoRebuildKey);
        }

        public static bool SetEnabled(bool enabled)
        {
            if (GetEnabled() == enabled)
                return false;

            EditorPrefs.SetBool(AutoRebuildKey, enabled);
            
            Debug.Log(enabled
                          ? "AssetBundles: Auto Rebuild enabled."
                          : "AssetBundles: Auto Rebuild disabled.");

            return true;
        }
        
        /*
         * Private.
         */

        private static void OnBuildPlayer(BuildPlayerOptions options)
        {
            if (!GetEnabled() || !AssetBundlesBuilder.CheckAssetBundlesExist())
                return;
            
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && GetEnabled())
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
        }
    }
}

#endif