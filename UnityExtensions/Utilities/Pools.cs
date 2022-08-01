using System;
using System.Collections.Generic;

namespace UnityExtensions.Utilities;

public static class Pools<TSource> where TSource : new()
{
    public static Dictionary<string, Pool<TSource>> PoolsDict { get; } = new();

    public static IEnumerable<string> PoolNames => PoolsDict.Keys;

    public static Pool<TSource> GetOrCreatePool(string name = "", Func<TSource>? factory = null) =>
        PoolsDict.TryGetValue(name, out Pool<TSource> pool) switch
        {
            true  => pool!,
            false => CreateNewPool(name, factory ?? (() => new TSource())),
        };

    public static Pool<TSource> CreateNewPool(string name = "", Func<TSource>? factory = null)
    {
        var pool = new Pool<TSource>(factory ?? (() => new TSource()));
        PoolsDict.Add(name, pool);
        return pool;
    }

    public static Pool<TSource> CreateNewPool(string name, Func<TSource> factory, int initialSize)
    {
        Pool<TSource> pool = CreateNewPool(name, factory);
        pool.Create(initialSize);
        return pool;
    }
}