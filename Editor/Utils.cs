#if UNITY_EDITOR

using System;

namespace Build1.UnityAssetBundlesTool.Editor
{
    internal static class Utils
    {
        internal static string ToTimeFormatted(this TimeSpan timeSpan)
        {
            return $"{timeSpan.TotalMinutes:n0}:{timeSpan.Seconds}";
        }
    }
}

#endif