using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityExtensions.Utilities
{
    /// <summary>
    ///     A structure for a collection of <see cref="PoolObject{TSource}" /> with events for Spawn/Despawn from the pool and
    ///     Add whenever a
    ///     <see cref="PoolObject{TSource}" /> is added to the pool
    /// </summary>
    /// <typeparam name="TSource"> Type to be pooled </typeparam>
    public class Pool<TSource> where TSource : new()
    {
        /// <summary>
        ///     Instantiates a pool with specified factory for new objects
        /// </summary>
        /// <param name="factory"> Function to use for instantiating new objects for the pool </param>
        internal Pool(Func<TSource> factory)
        {
            _factory = factory;
            _availablePoolObjects = new Stack<PoolObject<TSource>>();
            _totalPoolObjects = new HashSet<PoolObject<TSource>>();
        }

        /// <summary>
        ///     Called by <see cref="Spawn()" /> if <see cref="Available" /> > 0
        /// </summary>
        /// <returns> <see cref="PoolObject{TSource}" /> from the stack </returns>
        private PoolObject<TSource> SpawnFromPool()
        {
            PoolObject<TSource>? obj = _availablePoolObjects.Pop();
            OnSpawn(obj);
            return obj;
        }

        /// <summary>
        ///     Called by <see cref="Spawn()" /> if <see cref="Available" /> == 0 (stack empty)
        /// </summary>
        /// <returns> <see cref="PoolObject{TSource}" /> by provided factory </returns>
        private PoolObject<TSource> SpawnNewObject()
        {
            var obj = _factory();
            var poolObj = new PoolObject<TSource>(obj);
            Add(obj);
            OnSpawn(poolObj);
            return poolObj;
        }

        /// <summary>
        ///     Spawns a <see cref="PoolObject{TSource}" /> and calls <see cref="SpawnEvent" />
        ///     <br /> If no <see cref="PoolObject{TSource}" /> is available, a new one is created and
        ///     <see cref="ObjectAddedEvent" /> is also called.
        /// </summary>
        /// <returns>
        ///     <see cref="PoolObject{TSource}" />
        /// </returns>
        public PoolObject<TSource> Spawn() =>
            Available switch
            {
                0 => SpawnNewObject(),
                _ => SpawnFromPool(),
            };

        /// <summary>
        ///     Calls <see cref="Spawn()" /> for an <paramref name="amount" /> of times and lazily returns an
        ///     <see cref="IEnumerable{T}" /> of <see cref="PoolObject{TSource}" />
        ///     <br /> Beware that the objects are spawned lazily with yield.
        /// </summary>
        /// <param name="amount"> Number of objects to spawn</param>
        /// <returns> A collection of <see cref="PoolObject{TSource}" /> </returns>
        public IEnumerable<PoolObject<TSource>> Spawn(int amount)
        {
            for (var i = 0; i < amount; i++) yield return Spawn();
        }

        /// <summary>
        ///     Despawns a <see cref="PoolObject{TSource}" /> to the pool and calls <see cref="DespawnEvent" />
        ///     If not already in the pool, will call <see cref="Add(TSource)" /> on the object first.
        ///     <br /> If already in the pool, but already despawned, it will do nothing.
        /// </summary>
        /// <param name="obj"> Object to despawn to the pool </param>
        /// <returns>
        ///     true - if on the available stack <br />
        ///     false - if not on the available stack (already despawned)
        /// </returns>
        public bool Despawn(PoolObject<TSource> obj)
        {
            if (InPool(obj))
            {
                if (IsAvailable(obj)) return false;
                OnDespawn(obj);
                _availablePoolObjects.Push(obj);
                return true;
            }

            Add(obj);
            OnDespawn(obj);
            _availablePoolObjects.Push(obj);
            return true;
        }

        /// <summary>
        ///     Calls <see cref="Despawn(UnityExtensions.Utilities.PoolObject{TSource})" /> for a range of
        ///     <see cref="PoolObject{TSource}" />
        /// </summary>
        /// <param name="objs"> Objects to despawn </param>
        public IEnumerable<bool> Despawn(IEnumerable<PoolObject<TSource>> objs) => objs.Select(Despawn);

        /// <summary>
        ///     Despawns the whole pool.
        /// </summary>
        /// <param name="onlyAvailable">
        ///     true - only despawns the available stack. <br />
        ///     false - clears stack and despawns all pooled objects.
        /// </param>
        public void DespawnAll(bool onlyAvailable = true)
        {
            if (onlyAvailable)
            {
                foreach (PoolObject<TSource>? poolObject in _availablePoolObjects) Despawn(poolObject);
            }
            else
            {
                _availablePoolObjects.Clear();
                foreach (PoolObject<TSource>? poolObject in _totalPoolObjects) Despawn(poolObject);
            }
        }

        /// <summary>
        ///     Adds an object instance to the pool.
        /// </summary>
        /// <param name="source"> Instance to add. </param>
        /// <returns>
        ///     true - object was added <br />
        ///     false - object already in pool
        /// </returns>
        public bool Add(TSource source)
        {
            var obj = new PoolObject<TSource>(source);

            if (InPool(obj)) return false;

            _totalPoolObjects.Add(obj);
            _availablePoolObjects.Push(obj);
            OnObjectAdded(obj);

            return true;
        }

        /// <summary>
        ///     Adds a <see cref="PoolObject{TSource}" /> to the pool.
        /// </summary>
        /// <param name="source"> Instance to add. </param>
        /// <returns>
        ///     true - object was added <br />
        ///     false - object already in pool
        /// </returns>
        public bool Add(PoolObject<TSource> source)
        {
            if (InPool(source)) return false;

            _totalPoolObjects.Add(source);
            _availablePoolObjects.Push(source);
            OnObjectAdded(source);

            return true;
        }


        /// <summary>
        ///     Calls <see cref="Add(TSource)" /> on a range of objects.
        /// </summary>
        /// <param name="sources"> Collection to add </param>
        /// <returns> Collection of successes and failures for the <see cref="Add(TSource)" /> calls </returns>
        public IEnumerable<bool> Add(IEnumerable<TSource> sources) => sources.Select(Add);

        /// <summary>
        ///     Creates and adds object instances to the pool using the provided factory.
        /// </summary>
        /// <param name="count"> Amount of objects to instantiate and add. </param>
        public void CreateObjects(int count)
        {
            // while loop in case of hash conflicts
            var i = 0;
            while (i < count)
                // only increase on successful add
                if (Add(_factory()))
                    i++;
        }

    #region Fields and Properties

        /// <summary>
        ///     A <see cref="Stack{T}" /> of unused/free objects in the pool
        /// </summary>
        private readonly Stack<PoolObject<TSource>> _availablePoolObjects;

        /// <summary>
        ///     <see cref="Func{TResult}" /> used to create new instances when no object is available in pool or when Create(int)
        ///     is used
        /// </summary>
        private readonly Func<TSource> _factory;

        /// <summary>
        ///     A <see cref="HashSet{T}" /> with all the <see cref="PoolObject{TSource}" />s in the pool
        /// </summary>
        private readonly HashSet<PoolObject<TSource>> _totalPoolObjects;

        /// <summary>
        ///     How many objects are available/free in the pool.
        /// </summary>
        public int Available => _availablePoolObjects.Count;

        /// <summary>
        ///     How many objects are in the pool in total.
        /// </summary>
        public int Total => _totalPoolObjects.Count;

        /// <summary>
        ///     Check if <see cref="PoolObject{TSource}" /> is in the <see cref="Pool{TSource}" />.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> true if in the pool <br /> false if not in the pool </returns>
        public bool InPool(PoolObject<TSource> obj) => _totalPoolObjects.Contains(obj);

        /// <summary>
        ///     Check if <see cref="PoolObject{TSource}" /> is free/available.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> true if object is free <br /> false if object is not free </returns>
        public bool IsAvailable(PoolObject<TSource> obj) => _availablePoolObjects.Contains(obj);

    #endregion

    #region Events and Handlers

        /// <summary>
        ///     Event is called whenever a <see cref="PoolObject{TSource}" /> is added with <see cref="Add(TSource)" />,<br />
        ///     when one is created due to <see cref="Spawn()" /> when <see cref="Available" /> == 0,<br />
        ///     or when one is added by <see cref="Despawn(UnityExtensions.Utilities.PoolObject{TSource})" /> due to the despawned
        ///     object not being in the pool already.
        /// </summary>
        public event Action<PoolObject<TSource>>? ObjectAddedEvent;

        /// <summary>
        ///     Event is called whenever <see cref="Spawn()" /> is called
        /// </summary>
        public event Action<PoolObject<TSource>>? SpawnEvent;

        /// <summary>
        ///     Event is called whenever <see cref="Despawn(UnityExtensions.Utilities.PoolObject{TSource})" /> is called
        /// </summary>
        public event Action<PoolObject<TSource>>? DespawnEvent;

        /// <summary>
        ///     Invokes <see cref="ObjectAddedEvent" /> with <paramref name="obj" />
        /// </summary>
        /// <param name="obj"> <see cref="PoolObject{TSource}" /> that was added </param>
        private void OnObjectAdded(PoolObject<TSource> obj) => ObjectAddedEvent?.Invoke(obj);

        /// <summary>
        ///     Invokes <see cref="SpawnEvent" /> with <paramref name="obj" />
        /// </summary>
        /// <param name="obj"> <see cref="PoolObject{TSource}" /> that was spawned </param>
        private void OnSpawn(PoolObject<TSource> obj) => SpawnEvent?.Invoke(obj);

        /// <summary>
        ///     Invokes <see cref="DespawnEvent" /> with <paramref name="obj" />
        /// </summary>
        /// <param name="obj"> <see cref="PoolObject{TSource}" /> that was despawned </param>
        private void OnDespawn(PoolObject<TSource> obj) => DespawnEvent?.Invoke(obj);

    #endregion
    }
}