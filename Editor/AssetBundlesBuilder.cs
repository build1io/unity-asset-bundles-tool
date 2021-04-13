#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Build1.AssetBundlesTool.Editor
{
    internal static class AssetBundlesBuilder
    {
        public static void Build(BuildTarget target, Action onComplete = null)
        {
            Debug.Log($"AssetBundles: Building for {target}...");

            EditorApplication.delayCall += () =>
            {
                if (!Directory.Exists(Application.streamingAssetsPath))
                    Directory.CreateDirectory(Application.streamingAssetsPath);

                var output = BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.StrictMode, target);
                if (output == null) // No asset bundles.
                    ClearImpl();
                else
                    Debug.Log("AssetBundles: Done");
                
                onComplete?.Invoke();
            };
        }

        public static void Clear(Action onComplete = null)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
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
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
                ClearAssetBundle(name);
            ClearAssetBundle("StreamingAssets");
        }

        private static void ClearAssetBundle(string bundleName)
        {
            var file01 = Path.Combine(Application.streamingAssetsPath, bundleName);
            var file02 = Path.Combine(Application.streamingAssetsPath, bundleName) + ".manifest";
            var file03 = Path.Combine(Application.streamingAssetsPath, bundleName) + ".manifest.meta";
            var file04 = Path.Combine(Application.streamingAssetsPath, bundleName) + ".meta";
            
            File.Delete(file01);
            File.Delete(file02);
            File.Delete(file03);
            File.Delete(file04);
        }
    }
}

#endif