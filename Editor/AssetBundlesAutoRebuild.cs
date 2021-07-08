using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    internal sealed class AssetBundlesAutoRebuild : IActiveBuildTargetChanged
    {
        public const string AutoRebuildKey = "Build1_AssetBundlesTool_AutoRebuildEnabled";
        
        public int callbackOrder { get; }
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (!GetEnabled() || !AssetBundlesBuilder.CheckAssetBundlesExist())
                return;
            
            Debug.Log("AssetBundles: Current build target changed. Rebuilding...");
            
            EditorApplication.delayCall += () =>
            {
                AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
            };
        }
        
        /*
         * Static.
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
    }
}