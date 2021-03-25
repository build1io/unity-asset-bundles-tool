#if UNITY_EDITOR

using UnityEditor;

namespace Build1.AssetBundlesTool.Editor
{
    internal static class AssetBundlesMenu
    {
        [MenuItem("Build1/Asset Bundles/Build", false, 10)]
        public static void Build()
        {
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }
        
        [MenuItem("Build1/Asset Bundles/Rebuild", false, 11)]
        public static void Rebuild()
        {
            AssetBundlesBuilder.Clear(Build);
        }
        
        [MenuItem("Build1/Asset Bundles/Build Android", false, 50)]
        public static void BuildAndroid()
        {
            AssetBundlesBuilder.Build(BuildTarget.Android);
        }
        
        [MenuItem("Build1/Asset Bundles/Build IOS", false, 51)]
        public static void BuildIOS()
        {
            AssetBundlesBuilder.Build(BuildTarget.iOS);
        }
        
        [MenuItem("Build1/Asset Bundles/Build OSX", false, 100)]
        public static void BuildOSX()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneOSX);
        }

        [MenuItem("Build1/Asset Bundles/Build Windows", false, 101)]
        public static void BuildWindows()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneWindows);
        }
        
        [MenuItem("Build1/Asset Bundles/Build Windows 64", false, 102)]
        public static void BuildWindows64()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Build1/Asset Bundles/Build WebGL", false, 150)]
        public static void BuildWebGL()
        {
            AssetBundlesBuilder.Build(BuildTarget.WebGL);
        }

        [MenuItem("Build1/Asset Bundles/Clear", false, 200)]
        public static void Clear()
        {
            AssetBundlesBuilder.Clear();
        }
    }
}

#endif