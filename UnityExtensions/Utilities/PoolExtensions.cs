using System;

namespace UnityExtensions.Utilities;

public static class PoolExtensions
{
    public static PoolObject<TSource> MakePoolable<TSource>(this TSource source) where TSource : new() => new(source);

    public static PoolObject<TSource> MakePoolable<TSource>(this TSource source, Action<TSource> initialize)
        where TSource : new()
    {
        initialize(source);
        return new PoolObject<TSource>(source);
    }

    public static Pool<TSource> GetOrCreatePool<TSource>(this PoolObject<TSource> _,
                                                         string name = "",
                                                         Func<TSource>? factory = null) where TSource : new() =>
        Pools<TSource>.GetOrCreatePool(name, factory);

    public static void Despawn<TSource>(this PoolObject<TSource> poolObject, string name = "") where TSource : new() =>
        Pools<TSource>.GetOrCreatePool(name).Despawn(poolObject);

    public static void AddObjectCreationEvent<TSource>(this PoolObject<TSource>    _,
                                                       Action<PoolObject<TSource>> action,
                                                       string                      name = "") where TSource : new() =>
        Pools<TSource>.GetOrCreatePool(name).ObjectAddedEvent += action;

    public static void AddSpawnEvent<TSource>(this PoolObject<TSource>    _,
                                              Action<PoolObject<TSource>> action,
                                              string                      name = "") where TSource : new() =>
        Pools<TSource>.GetOrCreatePool(name).SpawnEvent += action;

    public static void AddDespawnEvent<TSource>(this PoolObject<TSource>    _,
                                                Action<PoolObject<TSource>> action,
                                                string                      name = "") where TSource : new() =>
        Pools<TSource>.GetOrCreatePool(name).DespawnEvent += action;
}