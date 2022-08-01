using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Instrumentation;
using JetBrains.Annotations;

namespace UnityExtensions.Utilities;

public static class Miscellaneous
{
    private static readonly Dictionary<int, object> _objects = new();

    public static TSource Provide<TSource>([NotNull] this TSource instance, bool replace = false) where TSource : new()
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        int hash = typeof(TSource).GetHashCode();
        if (replace && _objects.ContainsKey(hash)) _objects[hash] = instance;
        else _objects.Add(hash, instance);

        return instance;
    }

    public static TSource Retrieve<TSource>() where TSource : new()
    {
        int hash = typeof(TSource).GetHashCode();
        if (_objects.TryGetValue(hash, out object instance)) return (TSource)instance;
        throw new
            InstanceNotFoundException($"Instance of type {typeof(TSource).Name} not found, provide it with the {nameof(Provide)} method first.");
    }

    public static double TimeIt(this Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }
}