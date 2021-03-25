#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Build1.AssetBundlesTool.Editor
{
    internal sealed class AssetBundlesBuilder : IActiveBuildTargetChanged
    {
        private const string AssetBundlesDirectory = "Assets/StreamingAssets";

        public int callbackOrder { get; }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Debug.Log("AssetBundles: Current build target changed");
            EditorApplication.delayCall += () =>
            {
                Build(EditorUserBuildSettings.activeBuildTarget);
            };
        }

        /*
         * Static.
         */

        public static void Build(BuildTarget target, Action onComplete = null)
        {
            Debug.Log($"AssetBundles: Building for {target}...");

            EditorApplication.delayCall += () =>
            {
                if (!Directory.Exists(AssetBundlesDirectory))
                    Directory.CreateDirectory(AssetBundlesDirectory);

                var output = BuildPipeline.BuildAssetBundles(AssetBundlesDirectory, BuildAssetBundleOptions.StrictMode, target);
                if (output == null) // No asset bundles.
                    ClearImpl();
                else
                    Debug.Log("AssetBundles: Done");
                
                onComplete?.Invoke();
            };
        }

        public static void Clear(Action onComplete = null)
        {
            if (!Directory.Exists(AssetBundlesDirectory))
            {
                onComplete?.Invoke();
                return;
            }
            
            Debug.Log("AssetBundles: Cleaning...");
            EditorApplication.delayCall += () =>
            {
                ClearImpl();
                Debug.Log("AssetBundles: Done");
                onComplete?.Invoke();
            };
        }

        private static void ClearImpl()
        {
            Directory.Delete(AssetBundlesDirectory, true);
            File.Delete($"{AssetBundlesDirectory}.meta");
        }
    }
}

#endif