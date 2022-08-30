using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityExtensions.Utilities;

/// <summary>
/// A collection of PoolObjects with events for Spawn/Despawn from the pool and Add whenever a PoolObject is added to the pool
/// </summary>
/// <typeparam name="TSource"> Type to be pooled </typeparam>
public class Pool<TSource> where TSource : new()
{
    // used to create new instances when no object is available in pool or when Create(int) is used  
    private readonly Func<TSource>                _factory;
    private readonly Stack<PoolObject<TSource>>   _availablePoolObjects;
    private readonly HashSet<PoolObject<TSource>> _totalPoolObjects;

    /// <summary>
    /// Instantiates a pool with specified factory for new objects
    /// </summary>
    /// <param name="factory"> Function to use for instantiating new objects for the pool </param>
    internal Pool(Func<TSource> factory)
    {
        _factory = factory;
        _availablePoolObjects = new Stack<PoolObject<TSource>>();
        _totalPoolObjects = new HashSet<PoolObject<TSource>>();
    }

    /// <summary>
    /// How many objects are available/free in the pool.
    /// </summary>
    public int Available => _availablePoolObjects.Count;
    /// <summary>
    /// How many objects are in the pool in total.
    /// </summary>
    public int Total     => _totalPoolObjects.Count;

    /// <summary>
    /// Event is called whenever an object is added with <see cref="Add(TSource)"/> or when one is created due to <see cref="Spawn()"/> when <see cref="Available"/> is 0
    /// </summary>
    public event Action<PoolObject<TSource>>? ObjectAddedEvent;
    /// <summary>
    /// Event is called whenever <see cref="Spawn()"/> is called
    /// </summary>
    public event Action<PoolObject<TSource>>? SpawnEvent;
    /// <summary>
    /// Event is called whenever <see cref="Despawn(UnityExtensions.Utilities.PoolObject{TSource})"/> is called
    /// </summary>
    public event Action<PoolObject<TSource>>? DespawnEvent;
    
    private void OnObjectAdded(PoolObject<TSource> obj) => ObjectAddedEvent?.Invoke(obj);

    private void OnSpawn(PoolObject<TSource> obj) => SpawnEvent?.Invoke(obj);

    private void OnDespawn(PoolObject<TSource> obj) => DespawnEvent?.Invoke(obj);

    /// <summary>
    /// Called by <see cref="Spawn()"/> if <see cref="Available"/> > 0
    /// </summary>
    /// <returns> <see cref="PoolObject{TSource}"/> from the available stack </returns>
    private PoolObject<TSource> SpawnFromPool()
    {
        PoolObject<TSource>? obj = _availablePoolObjects.Pop();
        OnSpawn(obj);
        return obj;
    }

    /// <summary>
    /// Called by <see cref="Spawn()"/> if <see cref="Available"/> == 0
    /// </summary>
    /// <returns> <see cref="PoolObject{TSource}"/> from the available stack </returns>
    private PoolObject<TSource> SpawnNewObject()
    {
        TSource obj = _factory();
        var poolObj = new PoolObject<TSource>(obj);
        Add(obj);
        OnSpawn(poolObj);
        return poolObj;
    }

    /// <summary>
    /// Spawns a <see cref="PoolObject{TSource}"/> and calls <see cref="SpawnEvent"/>
    /// </summary>
    /// <returns> Returns <see cref="PoolObject{TSource}"/> </returns>
    public PoolObject<TSource> Spawn() =>
        _availablePoolObjects.Count switch
        {
            0 => SpawnNewObject(),
            _ => SpawnFromPool(),
        };

    /// <summary>
    /// Spawns a range of <see cref="PoolObject{TSource}"/> and calls <see cref="SpawnEvent"/> for each of them
    /// </summary>
    /// <param name="count"> Number of objects to spawn</param>
    /// <returns> Returns a range of <see cref="PoolObject{TSource}"/> </returns>
    public IEnumerable<PoolObject<TSource>> Spawn(int count)
    {
        for (var i = 0; i < count; i++) yield return Spawn();
    }

    /// <summary>
    /// Despawns a <see cref="PoolObject{TSource}"/> to the pool and calls <see cref="DespawnEvent"/>
    /// If not already in the pool, will call <see cref="Add(TSource)"/> on the object first
    /// </summary>
    /// <param name="obj"> Object to despawn to the pool </param>
    public void Despawn(PoolObject<TSource> obj)
    {
        if (InPool(obj))
        {
            OnDespawn(obj);
            _availablePoolObjects.Push(obj);
        }
        else
        {
            Add(obj);
            OnDespawn(obj);
            _availablePoolObjects.Push(obj);
        }
    }

    public void Despawn(IEnumerable<PoolObject<TSource>> objs) => objs.ForEach(Despawn);

    public void DespawnAll()
    {
        foreach (PoolObject<TSource> obj in _totalPoolObjects) Despawn(obj);
    }

    public bool Add(TSource source)
    {
        PoolObject<TSource> obj = source.MakePoolable();

        if (InPool(obj)) return false;

        _totalPoolObjects.Add(obj);
        _availablePoolObjects.Push(obj);
        OnObjectAdded(obj);

        return true;
    }

    public bool Add(PoolObject<TSource> source)
    {
        if (InPool(source)) return false;

        _totalPoolObjects.Add(source);
        _availablePoolObjects.Push(source);
        OnObjectAdded(source);

        return true;
    }

    public bool InPool(PoolObject<TSource> obj) => _totalPoolObjects.Contains(obj);

    public bool IsAvailable(PoolObject<TSource> obj) => _availablePoolObjects.Contains(obj);

    public IEnumerable<bool> Add(IEnumerable<TSource> sources) => sources.Select(Add);

    public void Clear() => _availablePoolObjects.Clear();

    public void Create(int count)
    {
        // while loop in case of hash conflicts
        var i = 0;
        while (i < count)
            // only increase on successful add
            if (Add(_factory()))
                i++;
    }
}