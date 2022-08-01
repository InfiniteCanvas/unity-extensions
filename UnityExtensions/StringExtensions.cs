using UnityEngine;

namespace UnityExtensions;

public static class StringExtensions
{
    public static void Log(this string str) => Debug.Log(str);

    public static void LogError(this string str) => Debug.LogError(str);

    public static void LogWarning(this string str) => Debug.LogWarning(str);
}