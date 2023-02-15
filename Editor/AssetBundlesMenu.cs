#if UNITY_EDITOR

using UnityEditor;

namespace Build1.UnityAssetBundlesTool.Editor
{
    internal static class AssetBundlesMenu
    {
        private const string AutoRebuildEnabledMenuItem  = "Tools/Build1/Asset Bundles/Auto Rebuild/Enable";
        private const string AutoRebuildDisabledMenuItem = "Tools/Build1/Asset Bundles/Auto Rebuild/Disable";
        private const string AutoRebuildInfoMenuItem     = "Tools/Build1/Asset Bundles/Auto Rebuild/Info";

        private const string CleanCacheAfterPlayEnabledMenuItem  = "Tools/Build1/Asset Bundles/Cache/Clean After Play";
        private const string CleanCacheAfterPlayDisabledMenuItem = "Tools/Build1/Asset Bundles/Cache/Preserve";
        private const string CleanCacheAfterPlayInfoMenuItem     = "Tools/Build1/Asset Bundles/Cache/Info";

        static AssetBundlesMenu()
        {
            EditorApplication.delayCall += UpdateMenu;
        }

        /*
         * Auto Rebuild.
         */
        
        [MenuItem(AutoRebuildEnabledMenuItem, false, 1010)]
        public static void AutoRebuildEnabled()
        {
            if (AssetBundlesProcessor.SetAutoRebuildEnabled(true))
                UpdateMenu();
        }

        [MenuItem(AutoRebuildEnabledMenuItem, true, 1010)]
        public static bool AutoRebuildEnabledValidation()
        {
            return !AssetBundlesProcessor.GetAutoRebuildEnabled();
        }

        [MenuItem(AutoRebuildDisabledMenuItem, false, 1011)]
        public static void AutoRebuildDisabled()
        {
            if (AssetBundlesProcessor.SetAutoRebuildEnabled(false))
                UpdateMenu();
        }

        [MenuItem(AutoRebuildDisabledMenuItem, true, 1011)]
        public static bool AutoRebuildDisabledValidation()
        {
            return AssetBundlesProcessor.GetAutoRebuildEnabled();
        }

        [MenuItem(AutoRebuildInfoMenuItem, false, 1040)]
        public static void AutoRebuildInfo()
        {
            EditorUtility.DisplayDialog("Auto Rebuild",
                                        "When Enabled, asset bundles will be rebuilt when the current target platform is changed or when building process is initiated.\n\n" +
                                        "The same will happen before Play in the Editor if any inconsistency in Asset Bundles files list is found.\n\n" +
                                        "NOTE: Currently, only Asset Bundles files (builds) list is analyzed. The tool doesn't track bundle content changes.",
                                        "Got it!");
        }
        
        /*
         * Clean Cache After Play.
         */
        
        [MenuItem(CleanCacheAfterPlayEnabledMenuItem, false, 1020)]
        public static void CleanCacheAfterPlayEnabled()
        {
            if (AssetBundlesProcessor.SetCleanCacheAfterPlayEnabled(true))
                UpdateMenu();
        }

        [MenuItem(CleanCacheAfterPlayEnabledMenuItem, true, 1020)]
        public static bool CleanCacheAfterPlayEnabledValidation()
        {
            return !AssetBundlesProcessor.GetCleanCacheAfterPlayEnabled();
        }

        [MenuItem(CleanCacheAfterPlayDisabledMenuItem, false, 1021)]
        public static void CleanCacheAfterPlayDisabled()
        {
            if (AssetBundlesProcessor.SetCleanCacheAfterPlayEnabled(false))
                UpdateMenu();
        }

        [MenuItem(CleanCacheAfterPlayDisabledMenuItem, true, 1021)]
        public static bool CleanCacheAfterPlayDisabledValidation()
        {
            return AssetBundlesProcessor.GetCleanCacheAfterPlayEnabled();
        }
        
        [MenuItem(CleanCacheAfterPlayInfoMenuItem, false, 1050)]
        public static void CleanCacheAfterPlayInfo()
        {
            EditorUtility.DisplayDialog("Cache",
                                        "Remote Asset Bundles can be put in local cache.\n\n" +
                                        "Clean after Play removes everything from cache when exiting Play Mode.",
                                        "Got it!");
        }
        
        /*
         * Build.
         */

        [MenuItem("Tools/Build1/Asset Bundles/Build", false, 1050)]
        public static void Build()
        {
            AssetBundlesBuilder.Build(AssetBundlesProcessor.GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode);
        }

        [MenuItem("Tools/Build1/Asset Bundles/Rebuild", false, 1051)]
        public static void Rebuild()
        {
            AssetBundlesBuilder.Build(AssetBundlesProcessor.GetLocalBuildTargetTyped(), BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ForceRebuildAssetBundle);
        }

        /*
         * Clean.
         */
        
        [MenuItem("Tools/Build1/Asset Bundles/Clean", false, 1100)]
        public static void Clean()
        {
            AssetBundlesBuilder.Clean();
        }
        
        /*
         * Tool Window.
         */

        [MenuItem("Tools/Build1/Asset Bundles/Tool Window...", false, 1150)]
        public static void ToolsWindow()
        {
            AssetBundlesWindow.Open();
        }

        /*
         * Private.
         */

        private static void UpdateMenu()
        {
            var enabled = AssetBundlesProcessor.GetAutoRebuildEnabled();
            Menu.SetChecked(AutoRebuildEnabledMenuItem, enabled);
            Menu.SetChecked(AutoRebuildDisabledMenuItem, !enabled);
            
            enabled = AssetBundlesProcessor.GetCleanCacheAfterPlayEnabled();
            Menu.SetChecked(CleanCacheAfterPlayEnabledMenuItem, enabled);
            Menu.SetChecked(CleanCacheAfterPlayDisabledMenuItem, !enabled);
        }
    }
}

#endif