#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    [InitializeOnLoad]
    internal static class AssetBundlesProcessor
    {
        public const string LocalBuildTarget = "Build1_AssetBundlesTool_LocalBuildTarget";
        public const string AutoRebuildKey   = "Build1_AssetBundlesTool_AutoRebuildEnabled";

        static AssetBundlesProcessor()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
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

        public static AssetBundleBuildTarget GetLocalBuildTarget()
        {
            if (!EditorPrefs.HasKey(LocalBuildTarget))
                return AssetBundleBuildTarget.CurrentBuildTarget;
            var str = EditorPrefs.GetString(LocalBuildTarget);
            return (AssetBundleBuildTarget)Enum.Parse(typeof(AssetBundleBuildTarget), str, true);
        }

        public static BuildTarget GetLocalBuildTargetTyped()
        {
            var buildTarget = GetLocalBuildTarget();
            if (buildTarget == AssetBundleBuildTarget.CurrentBuildTarget)
                return EditorUserBuildSettings.activeBuildTarget;
            return (BuildTarget)buildTarget;
        }

        public static void SetLocalBuildTarget(AssetBundleBuildTarget buildTarget)
        {
            if (GetLocalBuildTarget() == buildTarget)
                return;
            
            EditorPrefs.SetString(LocalBuildTarget, buildTarget.ToString());
            
            if (buildTarget == AssetBundleBuildTarget.CurrentBuildTarget)
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
            var buildAssetBundles = GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true);
            if (buildAssetBundles)
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget, BuildAssetBundleOptions.StrictMode, false);

            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);

            if (buildAssetBundles)
                Debug.Log("AssetBundles: Bundles were built before Building project");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(true))
            {
                AssetBundlesBuilder.Build(GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode, false);
                return;
            }
            
            if (state == PlayModeStateChange.EnteredPlayMode && GetEnabled() && AssetBundlesBuilder.CheckAssetBundlesExist(false))
                Debug.Log("AssetBundles: Bundles were built before Playing");
        }
    }
}

#endif