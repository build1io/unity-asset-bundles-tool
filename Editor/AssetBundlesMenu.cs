#if UNITY_EDITOR

using UnityEditor;

namespace Build1.UnityAssetBundlesTool.Editor
{
    internal static class AssetBundlesMenu
    {
        private const string AutoRebuildEnabledMenuItem = "Tools/Build1/Asset Bundles/Auto Rebuild/Enable";
        private const string AutoRebuildDisabledMenuItem = "Tools/Build1/Asset Bundles/Auto Rebuild/Disable";
        
        static AssetBundlesMenu()
        {
            EditorApplication.delayCall += UpdateMenu; 
        }
        
        [MenuItem(AutoRebuildEnabledMenuItem, false, 10)]
        public static void AutoRebuildEnabled()
        {
            if (AssetBundlesProcessor.SetEnabled(true))
                UpdateMenu();
        }

        [MenuItem(AutoRebuildEnabledMenuItem, true, 10)]
        public static bool AutoRebuildEnabledValidation()
        {
            return !AssetBundlesProcessor.GetEnabled();
        }
        
        [MenuItem(AutoRebuildDisabledMenuItem, false, 11)]
        public static void AutoRebuildDisabled()
        {
            if (AssetBundlesProcessor.SetEnabled(false))
                UpdateMenu();
        }
        
        [MenuItem(AutoRebuildDisabledMenuItem, true, 11)]
        public static bool AutoRebuildDisabledValidation()
        {
            return AssetBundlesProcessor.GetEnabled();
        }
        
        [MenuItem("Tools/Build1/Asset Bundles/Auto Rebuild/Info", false, 40)]
        public static void AutoRebuildInfo()
        {
            EditorUtility.DisplayDialog("Auto Rebuild",
                                        "When Enabled, asset bundles will be rebuilt when the current target platform is changed or when building process is initiated.\n\n" +
                                        "The same will happen before Play in the Editor if any inconsistency in Asset Bundles files list is found.\n\n" +
                                        "NOTE: Currently, only Asset Bundles files (builds) list is analyzed. The tool doesn't track bundle content changes.", 
                                        "Got it!");
        }
        
        [MenuItem("Tools/Build1/Asset Bundles/Build", false, 50)]
        public static void Build()
        {
            AssetBundlesBuilder.Build(AssetBundlesProcessor.GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode);
        }
        
        [MenuItem("Tools/Build1/Asset Bundles/Rebuild", false, 51)]
        public static void Rebuild()
        {
            AssetBundlesBuilder.Build(AssetBundlesProcessor.GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ForceRebuildAssetBundle);
        }
        
        [MenuItem("Tools/Build1/Asset Bundles/Clear", false, 100)]
        public static void Clear()
        {
            AssetBundlesBuilder.Clear();
        }
        
        [MenuItem("Tools/Build1/Asset Bundles/Tool Window...", false, 150)]
        public static void ToolsWindow()
        {
            AssetBundlesWindow.Open();
        }
        
        /*
         * Private.
         */

        private static void UpdateMenu()
        {
            var enabled = AssetBundlesProcessor.GetEnabled();
            Menu.SetChecked(AutoRebuildEnabledMenuItem, enabled);
            Menu.SetChecked(AutoRebuildDisabledMenuItem, !enabled);
        }
    }
}

#endif