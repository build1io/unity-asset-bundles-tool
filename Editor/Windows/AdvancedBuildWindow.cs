#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Build1.UnityAssetBundlesTool.Editor.Windows
{
    internal sealed class AdvancedBuildWindow : EditorWindow
    {
        private const int Width   = 640;
        private const int Height  = 320;
        private const int Padding = 10;

        private static AssetBundleBuildTargetFlags _targets = AssetBundleBuildTargetFlags.iOS;
        private static string                      _path    = "/Builds/Palaces/";
        private static string                      _pattern = "{0}_013_{1}";
        private static Dictionary<string, bool>    _bundles = new();

        private void OnGUI()
        {
            var changed = false;
            var enabled = !Application.isPlaying;

            if (GUI.enabled != enabled)
            {
                GUI.enabled = enabled;
                changed = true;
            }

            GUILayout.BeginVertical();
            GUILayout.Space(Padding);

            GUILayout.BeginHorizontal();
            GUILayout.Space(6);

            GUILayout.Label("Platforms:", GUILayout.Width(85));
            var buildTarget = _targets;
            var buildTargetNew = (AssetBundleBuildTargetFlags)RenderFlagsDropDown(buildTarget);
            _targets = buildTargetNew;

            GUILayout.Space(6);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(6);

            GUILayout.Label("Result Path:", GUILayout.Width(85));
            _path = GUILayout.TextField(_path);

            GUILayout.Space(6);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(6);

            GUILayout.Label("Name Pattern:", GUILayout.Width(85));
            _pattern = GUILayout.TextField(_pattern);

            GUILayout.Space(6);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Bundles:", GUILayout.Width(75));
            GUILayout.Space(6);

            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(6);

                _bundles.TryGetValue(name, out var value);
                _bundles[name] = GUILayout.Toggle(value, $" {name}");

                GUILayout.Space(6);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            var build = GUILayout.Button("Build");
            if (build)
                BuildImpl();

            GUILayout.FlexibleSpace();

            GUILayout.Space(Padding);
            GUILayout.EndVertical();

            if (changed)
                GUI.enabled = !enabled;
        }

        private static Enum RenderFlagsDropDown(Enum value)
        {
            var listFull = Enum.GetValues(value.GetType()).Cast<Enum>();
            var list = listFull.Where(value.HasFlag).ToList();
            var label = list.Count > 0 ? string.Join(" ", list) : "Nothing";

            var valueNewImpl = EditorGUILayout.EnumFlagsField(value);

            var positionLast = GUILayoutUtility.GetLastRect();
            var positionRect = new Rect(positionLast.x + 2, positionLast.y + 2, positionLast.width - 20, positionLast.height - 4);

            if (EditorGUIUtility.isProSkin)
                EditorGUI.DrawRect(positionRect, new Color(0.3176F, 0.3176F, 0.3176F));
            else
                EditorGUI.DrawRect(positionRect, new Color(0.8745F, 0.8745F, 0.8745F));

            var position = new Rect(positionLast.x + 3, positionLast.y - 1, positionRect.width, positionLast.height);
            EditorGUI.LabelField(position, label);

            return valueNewImpl;
        }

        private void BuildImpl()
        {
            var platforms = 0;
            foreach (Enum value in Enum.GetValues(_targets.GetType()))
            {
                if (_targets.HasFlag(value))
                    platforms++;
            }

            if (platforms == 0)
            {
                Debug.LogError("Bundles Builder: No platforms selected for building");
                return;
            }

            if (_bundles.Count(i => i.Value) == 0)
            {
                Debug.LogError("Bundles Builder: No bundles selected for building");
                return;
            }

            var path = Path.Join(Application.dataPath, "../", _path);
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                Debug.Log($"Bundles Builder: Folder created. Path: {path.Replace(Application.dataPath, string.Empty)}");
            }

            foreach (Enum target in Enum.GetValues(_targets.GetType()))
            {
                if (!_targets.HasFlag(target))
                    continue;

                Debug.Log($"Bundles Builder: Building for {target}...");

                var platformPath = Path.Join(path, target.ToString());
                if (!Directory.Exists(platformPath))
                    Directory.CreateDirectory(platformPath);

                var assetBundleNames = new List<string>();

                foreach (var pair in _bundles)
                {
                    if (pair.Value)
                        assetBundleNames.Add(pair.Key);
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

                BuildPipeline.BuildAssetBundles(platformPath, 
                                                builds.ToArray(), 
                                                BuildAssetBundleOptions.AssetBundleStripUnityVersion | BuildAssetBundleOptions.StrictMode, 
                                                FlagValueToTarget((AssetBundleBuildTargetFlags)target));

                File.Delete(Path.Combine(platformPath, target.ToString()));
                File.Delete(Path.Combine(platformPath, target + ".manifest"));

                var files = Directory.GetFiles(platformPath);
                foreach (var file in files)
                {
                    if (file.EndsWith(".manifest"))
                    {
                        var extension = Path.GetExtension(file);
                        File.Move(file, string.Format(_pattern, file.Replace(extension, string.Empty), target.ToString().ToLower()) + extension);
                    }
                    else
                    {
                        File.Move(file, string.Format(_pattern, file, target.ToString().ToLower()));
                    }
                }

                Debug.Log("Bundles Builder: OK");
            }
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

        /*
         * Static.
         */

        public static void Open()
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var centerWidth = (main.width - Width) * 0.5f;
            var centerHeight = (main.height - Height) * 0.5f;

            var window = GetWindow<AdvancedBuildWindow>(false, "Bundles Building", true);
            window.position = new Rect(main.x + centerWidth, main.y + centerHeight, Width, Height);
            window.minSize = new Vector2(Width, Height);
        }
    }
}

#endif