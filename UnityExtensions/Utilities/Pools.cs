using System;
using System.Collections.Generic;

namespace UnityExtensions.Utilities;

public static class Pools<TSource> where TSource : new()
{
    /// <summary>
    /// Holds all pools of type <typeparamref name="TSource"/>
    /// </summary>
    private static Dictionary<string, Pool<TSource>> PoolsDict { get; } = new();

    /// <summary>
    /// Holds all the names of all saved pools.
    /// </summary>
    public static IEnumerable<string> PoolNames => PoolsDict.Keys;
    
    /// <summary>
    /// Gets <see cref="Pool{TSource}"/> with specified name. Default name is "". <br/>
    /// If <see cref="Pool{TSource}"/> doesn't exist, it is created with given <paramref name="factory"/>. Default <paramref name="factory"/> is () => new <typeparamref name="TSource"/>()
    /// </summary>
    /// <param name="name"> Name of the pool. (Key in <see cref="Dictionary{TKey,TValue}"/>). </param>
    /// <param name="factory"> Method used to create new instances of <typeparamref name="TSource"/>. </param>
    /// <returns> Instance of <see cref="Pool{TSource}"/> </returns>
    public static Pool<TSource> GetOrCreatePool(string name = "", Func<TSource>? factory = null) =>
        PoolsDict.TryGetValue(name, out Pool<TSource> pool) switch
        {
            true  => pool!,
            false => CreateNewPool(name, factory ?? (() => new TSource())),
        };

    /// <summary>
    /// Creates a new <see cref="Pool{TSource}"/> with specified <paramref name="name"/> and <paramref name="factory"/>. <br/>
    /// Defaults: name = "", factory = () => new <typeparamref name="TSource"/>()
    /// </summary>
    /// <param name="name"> Name of the pool. (Key in <see cref="Dictionary{TKey,TValue}"/>). </param>
    /// <param name="factory"> Method used to create new instances of <typeparamref name="TSource"/>. </param>
    /// <returns>Newly createad <see cref="Pool{TSource}"/></returns>
    private static Pool<TSource> CreateNewPool(string name = "", Func<TSource>? factory = null)
    {
        var pool = new Pool<TSource>(factory ?? (() => new TSource()));
        PoolsDict.Add(name, pool);
        return pool;
    }
}