#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build1.UnityEGUI;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor.Builder
{
    internal sealed class BuilderModel
    {
        private const string ConfigPath = "Build1/build1-asset-bundles-builder-config.json";

        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public BuilderConfig Config { get; }

        public BuilderModel()
        {
            Config = LoadConfig();
            if (Config == null)
            {
                Config = BuilderConfig.Default();
                SaveConfig();
            }

            if (ActualizeBundlesList())
                SaveConfig();
        }

        /*
         * Config.
         */

        private BuilderConfig LoadConfig()
        {
            var path = Path.Combine(Application.dataPath, ConfigPath);
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BuilderConfig>(json);
        }

        private void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented, JsonSerializerSettings);
            var path = Path.Combine(Application.dataPath, ConfigPath);
            File.WriteAllText(path, json);
        }

        /*
         * Actualization.
         */

        private bool ActualizeBundlesList()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            var dirty = false;

            foreach (var name in names)
            {
                if (Config.Bundles.FirstOrDefault(i => i.Name == name) != null)
                    continue;

                Config.Bundles.Add(BuilderBundleInfo.New(name));
                dirty = true;
            }

            for (var i = Config.Bundles.Count - 1; i >= 0; i--)
            {
                var info = Config.Bundles[i];
                if (names.Contains(info.Name))
                    continue;

                Config.Bundles.Remove(info);
                dirty = true;
            }

            return dirty;
        }

        /*
         * Saving.
         */

        public void SaveIfDirty()
        {
            var requiresSave = Config.IsDirty || Config.Bundles.Any(i => i.IsDirty);
            if (!requiresSave)
                return;

            SaveConfig();

            Config.ResetDirty();

            foreach (var info in Config.Bundles)
                info.ResetDirty();
        }

        /*
         * Building.
         */

        public void Build()
        {
            var platforms = 0;

            foreach (Enum value in Enum.GetValues(Config.Platforms.GetType()))
            {
                if (Config.Platforms.HasFlag(value))
                    platforms++;
            }

            if (platforms == 0)
            {
                EGUI.Alert("Asset Bundles Builder", "No platforms selected for building.");
                return;
            }

            if (!Config.Bundles.Any(i => i.IncludeInBuildSequence))
            {
                EGUI.Alert("Asset Bundles Builder", "No bundles included in build sequence.");
                return;
            }

            var path = Path.Join(Application.dataPath, "../", Config.BuildPath);
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Asset Bundles Builder: Build folder created. Path: {path.Replace(Application.dataPath, string.Empty)}");
            }

            var start = DateTime.UtcNow;
            var targetTimes = new Dictionary<Enum, TimeSpan>();
            
            foreach (Enum target in Enum.GetValues(Config.Platforms.GetType()))
            {
                if (!Config.Platforms.HasFlag(target))
                    continue;

                Debug.Log($"Asset Bundles Builder: Building for {target}...");

                var platformPath = Path.Join(path, target.ToString());
                if (!Directory.Exists(platformPath))
                    Directory.CreateDirectory(platformPath);

                var assetBundleNames = new List<string>();

                foreach (var bundleInfo in Config.Bundles)
                {
                    if (bundleInfo.IncludeInBuildSequence && !bundleInfo.ExcludeFromBuilderScope)
                        assetBundleNames.Add(bundleInfo.Name);
                }

                var builds = new List<AssetBundleBuild>();
                foreach (var assetBundle in assetBundleNames)
                {
                    builds.Add(new AssetBundleBuild
                    {
                        assetBundleName = assetBundle,
                        assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundle)
                    });
                }
                
                
                var startForTarget = DateTime.UtcNow;

                BuildPipeline.BuildAssetBundles(platformPath,
                                                builds.ToArray(),
                                                Config.Options,
                                                FlagValueToTarget((AssetBundleBuildTargetFlags)target));

                File.Delete(Path.Combine(platformPath, target.ToString()));
                File.Delete(Path.Combine(platformPath, target + ".manifest"));

                var files = Directory.GetFiles(platformPath);
                var pattern = Config.NamingPattern
                                    .Replace("{name}", "{0}")
                                    .Replace("{platform}", "{1}")
                                    .Replace("{version}", "{2}");

                foreach (var filePath in files)
                {
                    if (filePath.Contains("DS_Store"))
                        continue;

                    if (filePath.EndsWith(".manifest"))
                    {
                        var extension = Path.GetExtension(filePath);

                        var index = filePath.Replace(extension, string.Empty).LastIndexOf("/", StringComparison.Ordinal) + 1;
                        var name = filePath[index..].Replace(extension, string.Empty);

                        var info = Config.Bundles.First(i => i.Name == name);
                        var filePathNew = filePath[..index] + string.Format(pattern, name, target.ToString().ToLower(), info.Version) + extension;

                        File.Move(filePath, filePathNew);
                    }
                    else
                    {
                        var index = filePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
                        var name = filePath[index..];
                        var info = Config.Bundles.First(i => i.Name == name);
                        var filePathNew = filePath[..index] + string.Format(pattern, name, target.ToString().ToLower(), info.Version);

                        File.Move(filePath, filePathNew);
                    }
                }

                targetTimes.Add(target, DateTime.UtcNow - startForTarget);
                
                Debug.Log($"Asset Bundles Builder: Built for {target}");
            }

            foreach (var pair in targetTimes)
                Debug.Log($"Asset Bundles Builder: {pair.Key} build time: {pair.Value.ToTimeFormatted()}");

            var totalTime = DateTime.UtcNow - start;
            Debug.Log($"Asset Bundles Builder: Total build time: {totalTime.ToTimeFormatted()}");
        }

        private BuildTarget FlagValueToTarget(AssetBundleBuildTargetFlags flag)
        {
            return flag switch
            {
                AssetBundleBuildTargetFlags.iOS       => BuildTarget.iOS,
                AssetBundleBuildTargetFlags.Android   => BuildTarget.Android,
                AssetBundleBuildTargetFlags.WebGL     => BuildTarget.WebGL,
                AssetBundleBuildTargetFlags.OSX       => BuildTarget.StandaloneOSX,
                AssetBundleBuildTargetFlags.Windows64 => BuildTarget.StandaloneWindows64,
                _                                     => throw new ArgumentOutOfRangeException(nameof(flag), flag, null)
            };
        }
    }
}

#endif