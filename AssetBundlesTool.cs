using System;
using Build1.UnityAssetBundlesTool.Editor;
using UnityEditor;

namespace Build1.UnityAssetBundlesTool
{
    public static class AssetBundlesTool
    {
        public static void Build()
        {
            Build(EditorUserBuildSettings.activeBuildTarget);
        }

        public static void Build(BuildTarget buildTarget)
        {
            AssetBundlesBuilder.Build(buildTarget, BuildAssetBundleOptions.StrictMode, false);
        }

        public static void BuildAsync(Action onComplete = null)
        {
            BuildAsync(EditorUserBuildSettings.activeBuildTarget, onComplete);
        }

        public static void BuildAsync(BuildTarget buildTarget, Action onComplete = null)
        {
            AssetBundlesBuilder.Build(buildTarget, BuildAssetBundleOptions.StrictMode, true, onComplete);
        }
    }
}