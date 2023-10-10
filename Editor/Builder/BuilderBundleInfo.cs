#if UNITY_EDITOR

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    public sealed class BuilderBundleInfo
    {
        [JsonProperty("id")]      public string Id                      { get; private set; }
        [JsonProperty("name")]    public string Name                    { get; private set; }
        [JsonProperty("version")] public int    Version                 { get; private set; }
        [JsonProperty("exclude")] public bool   ExcludeFromBuilderScope { get; private set; }

        [JsonIgnore] public bool IncludeInBuildSequence
        {
            get
            {
                var key = $"bbi_{Id}";
                if (PlayerPrefs.HasKey(key))
                    return PlayerPrefs.GetInt(key) == 1;
                return true;
            }
            private set => PlayerPrefs.SetInt($"bbi_{Id}", value ? 1 : 0);
        }

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
         * Serialization.
         */
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Id == null)
            {
                Id = Guid.NewGuid().ToString();
                SetDirty();
            }
        }

        /*
         * Static.
         */

        public static BuilderBundleInfo New(string name)
        {
            return new BuilderBundleInfo
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Version = 1,
                IncludeInBuildSequence = true,
                ExcludeFromBuilderScope = false
            };
        }
    }
}

#endif