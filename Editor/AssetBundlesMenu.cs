#if UNITY_EDITOR

using UnityEditor;

namespace Build1.AssetBundlesTool.Editor
{
    internal static class AssetBundlesMenu
    {
        private const string AutoRebuildEnabledMenuItem = "Build1/Asset Bundles/Auto Rebuild/Enable";
        private const string AutoRebuildDisabledMenuItem = "Build1/Asset Bundles/Auto Rebuild/Disable";
        
        static AssetBundlesMenu()
        {
            EditorApplication.delayCall += UpdateMenu; 
        }
        
        [MenuItem(AutoRebuildEnabledMenuItem, false, 10)]
        public static void AutoRebuildEnabled()
        {
            if (AssetBundlesAutoRebuild.SetEnabled(true))
                UpdateMenu();
        }

        [MenuItem(AutoRebuildEnabledMenuItem, true, 10)]
        public static bool AutoRebuildEnabledValidation()
        {
            return !AssetBundlesAutoRebuild.GetEnabled();
        }
        
        [MenuItem(AutoRebuildDisabledMenuItem, false, 11)]
        public static void AutoRebuildDisabled()
        {
            if (AssetBundlesAutoRebuild.SetEnabled(false))
                UpdateMenu();
        }
        
        [MenuItem(AutoRebuildDisabledMenuItem, true, 11)]
        public static bool AutoRebuildDisabledValidation()
        {
            return AssetBundlesAutoRebuild.GetEnabled();
        }
        
        [MenuItem("Build1/Asset Bundles/Auto Rebuild/Info", false, 40)]
        public static void AutoRebuildInfo()
        {
            EditorUtility.DisplayDialog("Auto Rebuild",
                                        "When Enabled, asset bundles will be rebuilt when the current target platform is changed.\n\n" +
                                        "The same will happen before Play in the Editor if any inconsistency in Asset Bundles files list is found.\n\n" +
                                        "NOTE: Currently, only Asset Bundles files (builds) list is analyzed. The tool doesn't track if some asset from some bundle was changed.", 
                                        "Got it!");
        }
        
        [MenuItem("Build1/Asset Bundles/Build", false, 50)]
        public static void Build()
        {
            AssetBundlesBuilder.Build(EditorUserBuildSettings.activeBuildTarget);
        }
        
        [MenuItem("Build1/Asset Bundles/Rebuild", false, 51)]
        public static void Rebuild()
        {
            AssetBundlesBuilder.Clear(true, Build);
        }
        
        [MenuItem("Build1/Asset Bundles/Build Android", false, 100)]
        public static void BuildAndroid()
        {
            AssetBundlesBuilder.Build(BuildTarget.Android);
        }
        
        [MenuItem("Build1/Asset Bundles/Build IOS", false, 101)]
        public static void BuildIOS()
        {
            AssetBundlesBuilder.Build(BuildTarget.iOS);
        }
        
        [MenuItem("Build1/Asset Bundles/Build OSX", false, 150)]
        public static void BuildOSX()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneOSX);
        }

        [MenuItem("Build1/Asset Bundles/Build Windows", false, 151)]
        public static void BuildWindows()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneWindows);
        }
        
        [MenuItem("Build1/Asset Bundles/Build Windows 64", false, 152)]
        public static void BuildWindows64()
        {
            AssetBundlesBuilder.Build(BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Build1/Asset Bundles/Build WebGL", false, 200)]
        public static void BuildWebGL()
        {
            AssetBundlesBuilder.Build(BuildTarget.WebGL);
        }

        [MenuItem("Build1/Asset Bundles/Clear", false, 250)]
        public static void Clear()
        {
            AssetBundlesBuilder.Clear();
        }
        
        /*
         * Private.
         */

        private static void UpdateMenu()
        {
            var enabled = AssetBundlesAutoRebuild.GetEnabled();
            Menu.SetChecked(AutoRebuildEnabledMenuItem, enabled);
            Menu.SetChecked(AutoRebuildDisabledMenuItem, !enabled);
        }
    }
}

#endif