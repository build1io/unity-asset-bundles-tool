#if UNITY_EDITOR

using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEditor;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    internal sealed class BuilderConfig
    {
        [JsonProperty("platforms")]      public AssetBundleBuildTargetFlags Platforms     { get; private set; }
        [JsonProperty("build_path")]     public string                      BuildPath     { get; private set; }
        [JsonProperty("naming_pattern")] public string                      NamingPattern { get; private set; }
        [JsonProperty("options")]        public BuildAssetBundleOptions     Options       { get; private set; }
        [JsonProperty("bundles")]        public List<BuilderBundleInfo>     Bundles       { get; private set; }

        [JsonIgnore] public bool IsDirty { get; private set; }

        private BuilderConfig() { }

        /*
         * Public.
         */

        public void SetDirty()
        {
            IsDirty = true;
        }

        public void ResetDirty()
        {
            IsDirty = false;
        }

        /*
         * Serialization.
         */

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Options == 0)
                Options = BuildAssetBundleOptions.AssetBundleStripUnityVersion | BuildAssetBundleOptions.StrictMode;
            
            Bundles ??= new List<BuilderBundleInfo>();
        }

        /*
         * Static.
         */

        public static BuilderConfig Default()
        {
            return new BuilderConfig
            {
                Platforms = AssetBundleBuildTargetFlags.iOS | AssetBundleBuildTargetFlags.Android,
                BuildPath = "/Assets/StreamingAssets/",
                NamingPattern = "{name}_{version}_{platform}",
                Bundles = new List<BuilderBundleInfo>()
            };
        }
    }
}

#endif