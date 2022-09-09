#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesProcessor
    {
        public const string LocalBuildTarget              = "Build1_AssetBundlesTool_LocalBuildTarget";
        public const string AutoRebuildKey                = "Build1_AssetBundlesTool_AutoRebuildEnabled";
        public const string CleanCacheAfterPlayEnabledKey = "Build1_AssetBundlesTool_CleanCacheAfterPlayEnabled";

        static AssetBundlesProcessor()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /*
         * Public.
         */

        public static bool GetAutoRebuildEnabled()
        {
            if (EditorPrefs.HasKey(AutoRebuildKey))
                return EditorPrefs.GetBool(AutoRebuildKey);
            return true;
        }

        public static bool SetAutoRebuildEnabled(bool enabled)
        {
            if (GetAutoRebuildEnabled() == enabled)
                return false;

            EditorPrefs.SetBool(AutoRebuildKey, enabled);

            Debug.Log(enabled
                          ? "AssetBundles: Auto Rebuild enabled."
                          : "AssetBundles: Auto Rebuild disabled.");

            return true;
        }

        public static bool GetCleanCacheAfterPlayEnabled()
        {
            if (EditorPrefs.HasKey(CleanCacheAfterPlayEnabledKey))
                return EditorPrefs.GetBool(CleanCacheAfterPlayEnabledKey);
            return false;
        }

        public static bool SetCleanCacheAfterPlayEnabled(bool enabled)
        {
            if (GetCleanCacheAfterPlayEnabled() == enabled)
                return false;

            EditorPrefs.SetBool(CleanCacheAfterPlayEnabledKey, enabled);

            Debug.Log(enabled
                          ? "AssetBundles: Clean cache after Play enabled."
                          : "AssetBundles: Clean cache after Play disabled.");

            return true;
        }

        public static AssetBundleBuildTarget GetLocalBuildTarget()
        {
            if (!EditorPrefs.HasKey(LocalBuildTarget))
                return AssetBundleBuildTarget.Current;
            
            var str = EditorPrefs.GetString(LocalBuildTarget);
            if (str == "CurrentBuildTarget")
            {
                EditorPrefs.SetString(LocalBuildTarget, AssetBundleBuildTarget.Current.ToString());
                return AssetBundleBuildTarget.Current;
            }
            
            return (AssetBundleBuildTarget)Enum.Parse(typeof(AssetBundleBuildTarget), str, true);
        }

        public static BuildTarget GetLocalBuildTargetTyped()
        {
            var buildTarget = GetLocalBuildTarget();
            if (buildTarget == AssetBundleBuildTarget.Current)
                return EditorUserBuildSettings.activeBuildTarget;
            return (BuildTarget)buildTarget;
        }

        public static void SetLocalBuildTarget(AssetBundleBuildTarget buildTarget)
        {
            if (GetLocalBuildTarget() == buildTarget)
                return;

            EditorPrefs.SetString(LocalBuildTarget, buildTarget.ToString());

            if (buildTarget == AssetBundleBuildTarget.Current)
                Debug.Log("AssetBundles: Local build target set to Current Build Target.");
            else
                Debug.Log($"AssetBundles: Local build target set to {buildTarget}.");
        }

        /*
         * Private.
         */

        private static void OnBuildPlayer(BuildPlayerOptions options)
        {
            // While building to a device we always use target platform build target.
            var buildAssetBundles = GetAutoRebuildEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true);
            if (buildAssetBundles)
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, BuildAssetBundleOptions.StrictMode, false);

            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);

            if (buildAssetBundles)
                Debug.Log("AssetBundles: Bundles were built before Building project");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (GetAutoRebuildEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true))
                        AssetBundlesBuilder.Build(GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode, false);
                    break;
                }
                case PlayModeStateChange.EnteredPlayMode:
                {
                    break;
                }
                case PlayModeStateChange.ExitingPlayMode:
                {
                    if (GetCleanCacheAfterPlayEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true))
                    {
                        AssetBundle.UnloadAllAssetBundles(false);
                        Caching.ClearCache();
                        Debug.Log("AssetBundles: Cache cleaned");
                    }
                    break;
                }
            }
        }
    }
}

#endif