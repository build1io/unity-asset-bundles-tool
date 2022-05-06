#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    internal static class AssetBundlesBuilder
    {
        /*
         * Check.
         */

        public static bool CheckAssetBundlesBuilt()
        {
            if (!Directory.Exists(Application.streamingAssetsPath) && CheckAssetBundlesExist(false))
                return false;

            var bundlesNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundlesName in bundlesNames)
            {
                if (CheckAssetBundle(bundlesName))
                    return true;
            }
            
            return false;
        }

        public static bool CheckAssetBundlesExist(bool removeUnusedAssetBundles)
        {
            if (removeUnusedAssetBundles)
                AssetDatabase.RemoveUnusedAssetBundleNames();
            return AssetDatabase.GetAllAssetBundleNames().Length != 0;
        }
        
        /*
         * Build.
         */
        
        public static void Build(BuildTarget target, BuildAssetBundleOptions options, bool async = true, Action onComplete = null)
        {
            Log($"Building for {target}...");
            
            if (!CheckAssetBundlesExist(false))
            {
                Log("No bundles defined.");
                return;
            }

            if (!async)
            {
                BuildImpl(target, options);
                Log("Built.");
                onComplete?.Invoke();
                return;
            }
            
            EditorApplication.delayCall += () =>
            {
                BuildImpl(target, options);
                Log("Built.");
                onComplete?.Invoke();
            };
        }

        private static void BuildImpl(BuildTarget target, BuildAssetBundleOptions options)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            var output = BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, options, target);
            if (output != null) 
                return;
            
            Log("No asset bundles to build. Cleaning existing bundles...");
            ClearImpl();
        }
        
        /*
         * Clear.
         */

        public static void Clean(bool async = true, Action onComplete = null)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Log("Bundles folder not found. There is nothing to clear.");
                onComplete?.Invoke();
                return;
            }
            
            Log("Cleaning...");

            if (!async)
            {
                ClearImpl();
                Log("Cleaned.");
                onComplete?.Invoke();
                return;
            }
            
            EditorApplication.delayCall += () =>
            {
                ClearImpl();
                Log("Cleaned.");
                onComplete?.Invoke();
            };
        }
        
        private static void ClearImpl()
        {
            var names = AssetDatabase.GetAllAssetBundleNames().ToList();
            names.Add("StreamingAssets");
            
            foreach (var name in names)
                ClearAssetBundle(name);
        }

        private static void ClearAssetBundle(string bundleName)
        {
            var paths = GetBundleFilesPaths(bundleName);
            foreach (var path in paths)
                File.Delete(path);
        }
        
        /*
         * Check.
         */
        
        public static bool CheckAssetBundles()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                return false;
            
            var names = AssetDatabase.GetAllAssetBundleNames().ToList();
            names.Add("StreamingAssets");
            return names.All(CheckAssetBundle);
        }

        private static bool CheckAssetBundle(string bundleName)
        {
            return GetBundleFilesPaths(bundleName).All(File.Exists);
        }
        
        /*
         * Private.
         */

        private static IEnumerable<string> GetBundleFilesPaths(string bundleName)
        {
            return new[]
            {
                Path.Combine(Application.streamingAssetsPath, bundleName),
                Path.Combine(Application.streamingAssetsPath, bundleName) + ".manifest",
                Path.Combine(Application.streamingAssetsPath, bundleName) + ".manifest.meta",
                Path.Combine(Application.streamingAssetsPath, bundleName) + ".meta"
            };
        }

        /*
         * Logging.
         */

        private static void Log(string message)
        {
            Debug.Log($"AssetBundles: {message}");
        }
    }
}

#endif