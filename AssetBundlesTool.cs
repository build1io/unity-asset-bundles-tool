#if UNITY_EDITOR

using System;
using System.Linq;
using Build1.UnityAssetBundlesTool.Editor;
using Build1.UnityAssetBundlesTool.Editor.Builder;
using UnityEditor;
using UnityEngine;

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

        public static AssetBundleToolResult SeBundleVersionInBuildConfiguration(string bundleName, int version)
        {
            var model = new BuilderModel();
            var bundleInfo = model.Config.Bundles.FirstOrDefault(info => info.Name == bundleName);
            if (bundleInfo == null)
                return AssetBundleToolResult.BundleNotFound;
            
            bundleInfo.SetVersion(version);
            model.SaveIfDirty();
            
            return AssetBundleToolResult.Success;
        }
        
        public static AssetBundleToolResult BuildBundleUsingBuilderConfiguration(string bundleName, 
                                                                                 AssetBundleBuildTargetFlags platforms, 
                                                                                 string buildPath, 
                                                                                 string namingPattern, 
                                                                                 BuildAssetBundleOptions options)
        {
            var model = new BuilderModel();
            var bundleInfo = model.Config.Bundles.FirstOrDefault(info => info.Name == bundleName);
            if (bundleInfo == null)
                return AssetBundleToolResult.BundleNotFound;

            try
            {
                model.Build(bundleName, platforms, buildPath, namingPattern, options);
                return AssetBundleToolResult.Success;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return AssetBundleToolResult.BuildError;
            }
            
        }
    }
}

#endif