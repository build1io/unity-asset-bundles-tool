#if UNITY_EDITOR

using Build1.UnityEGUI;
using Build1.UnityEGUI.Components.Title;
using Build1.UnityEGUI.Window;
using UnityEditor;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    internal sealed class BuilderWindow : EGUIWindow
    {
        private BuilderModel _model;

        protected override void OnEGUI()
        {
            _model ??= new BuilderModel();

            var config = _model.Config;
            
            EGUI.Title("Asset Bundles Builder", TitleType.H1, EGUI.OffsetX(5));
            EGUI.Space(18);

            var platforms = config.Platforms;
            var buildPath = config.BuildPath;
            var namingPattern = config.NamingPattern;
            
            EGUI.Property(config, config.Platforms, nameof(config.Platforms));
            EGUI.Property(config, config.BuildPath, nameof(config.BuildPath));
            EGUI.Property(config, config.NamingPattern, nameof(config.NamingPattern));
            EGUI.Space(18);
            
            if (platforms != config.Platforms || buildPath != config.BuildPath || namingPattern != config.NamingPattern)
                config.SetDirty();
            
            EGUI.MessageBox("Asset bundles list updates automatically depending on the bundles list in the project settings.", MessageType.Info);
            EGUI.Space(9);
            
            EGUI.PropertyList(config, config.Bundles, nameof(config.Bundles))
                .ItemRenderer<BuilderBundleInfoItemRenderer>()
                .OnItemAddAvailable(() => false)
                .Build();

            _model.SaveIfDirty();
            
            EGUI.Space(9);
            EGUI.Button("Build", EGUI.Height(EGUI.ButtonHeight01))
                .OnClick(_model.Build);
        }

        /*
         * Static.
         */

        public static void Open()
        {
            EGUI.Window<BuilderWindow>("Asset Bundles Builder", false)
                .Size(800, 600)
                .Get();
        }
    }
}

#endif