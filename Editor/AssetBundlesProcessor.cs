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

            if (AssetBundlesBuilder.CheckAssetBundlesBuilt() || !AssetBundlesBuilder.CheckAssetBundlesExist(true))
                return;
            
            Debug.Log("AssetBundles: Bundles not built. Building...");
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }

        /*
         * Public.
         */

        public static bool GetEnabled()
        {
            if (EditorPrefs.HasKey(AutoRebuildKey))
                return EditorPrefs.GetBool(AutoRebuildKey);
            return true;
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
            var buildAssetBundles = GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true);
            if (buildAssetBundles)
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
            
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);

            if (buildAssetBundles)
                Debug.Log("AssetBundles: Bundles were built before Building project");
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true))
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, false);
            else if (state == PlayModeStateChange.EnteredPlayMode && GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(false))
                Debug.Log("AssetBundles: Bundles were built before Playing");
        }
    }
}

#endif