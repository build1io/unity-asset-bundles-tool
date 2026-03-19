#if UNITY_EDITOR

using System;

namespace Build1.UnityAssetBundlesTool.Editor
{
    [Flags]
    public enum AssetBundleBuildTargetFlags
    {
        iOS       = 1 << 0,
        Android   = 1 << 1,
        WebGL     = 1 << 2,
        OSX       = 1 << 3,
        Windows64 = 1 << 4
    }
}

#endif