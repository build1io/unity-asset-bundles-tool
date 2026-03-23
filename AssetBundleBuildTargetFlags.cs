using System;

namespace Build1.UnityAssetBundlesTool
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