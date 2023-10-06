#if UNITY_EDITOR

using Newtonsoft.Json;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    public sealed class BuilderBundleInfo
    {
        [JsonProperty("name")]                      public string Name                    { get; private set; }
        [JsonProperty("version")]                   public int    Version                 { get; private set; }
        [JsonProperty("include_in_build_sequence")] public bool   IncludeInBuildSequence  { get; private set; }
        [JsonProperty("exclude")]                   public bool   ExcludeFromBuilderScope { get; private set; }

        [JsonIgnore] public bool IsDirty { get; private set; }

        private BuilderBundleInfo() { }

        /*
         * Public.
         */

        public void IncrementVersion()
        {
            Version++;
            SetDirty();
        }

        public void DecrementVersion()
        {
            if (Version <= 0)
                return;

            Version--;
            SetDirty();
        }

        public void ExcludeFromBuilder()
        {
            ExcludeFromBuilderScope = true;
            SetDirty();
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public void ResetDirty()
        {
            IsDirty = false;
        }

        /*
         * Static.
         */

        public static BuilderBundleInfo New(string name)
        {
            return new BuilderBundleInfo
            {
                Name = name,
                Version = 1,
                IncludeInBuildSequence = true,
                ExcludeFromBuilderScope = false
            };
        }
    }
}

#endif