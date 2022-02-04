using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor
{
    public sealed class AssetBundlesWindow : EditorWindow
    {
        private const int Width   = 360;
        private const int Height  = 60;
        private const int Padding = 10;
        
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(Padding);
                
            GUILayout.BeginHorizontal();
            GUILayout.Space(6);
            
            GUILayout.Label("Build Target:", GUILayout.Width(75));
            
            var buildTarget = AssetBundlesProcessor.GetLocalBuildTarget();
            var buildTargetNew = (AssetBundleBuildTarget)EditorGUILayout.EnumPopup(buildTarget);
            if (buildTargetNew != buildTarget)
                AssetBundlesProcessor.SetLocalBuildTarget(buildTargetNew);

            var buildTargetTyped = AssetBundlesProcessor.GetLocalBuildTargetTyped();
            
            if (GUILayout.Button("Build", GUILayout.Width(60), GUILayout.Height(18)))
                AssetBundlesBuilder.Build(buildTargetTyped, BuildAssetBundleOptions.StrictMode);

            if (GUILayout.Button("Rebuild", GUILayout.Width(60), GUILayout.Height(18)))
                AssetBundlesBuilder.Build(buildTargetTyped, BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ForceRebuildAssetBundle);
            
            GUILayout.Space(6);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(Padding);
            GUILayout.Space(76);
            
            var enabled = AssetBundlesProcessor.GetEnabled();
            var enabledNew = GUILayout.Toggle(enabled, "Auto rebuild");
            if (enabled != enabledNew)
                AssetBundlesProcessor.SetEnabled(enabledNew);
            
            GUILayout.FlexibleSpace();
            
            GUILayout.Space(Padding);
            GUILayout.EndHorizontal();

            GUILayout.Space(Padding);
            GUILayout.EndVertical();
        }
        
        /*
         * Static.
         */

        public static void Open()
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var centerWidth = (main.width - Width) * 0.5f;
            var centerHeight = (main.height - Height) * 0.5f;
            
            var window = GetWindow<AssetBundlesWindow>(false, "Asset Bundles Tool", true);
            window.position = new Rect(main.x + centerWidth, main.y + centerHeight, Width, Height);
            window.minSize = new Vector2(Width, Height);
        }
    }
}