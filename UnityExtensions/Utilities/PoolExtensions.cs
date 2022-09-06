namespace UnityExtensions.Utilities
{
    public static class PoolExtensions
    {
        /// <summary>
        ///     Wraps an object into a <see cref="PoolObject{TSource}" />
        /// </summary>
        /// <param name="source">Object instance. Cannot be null.</param>
        /// <typeparam name="TSource">Type of the object</typeparam>
        /// <returns>Wrapped object as a <see cref="PoolObject{TSource}" /></returns>
        public static PoolObject<TSource> MakePoolable<TSource>(this TSource source) where TSource : new() =>
            new(source);

        /// <summary>
        ///     Despawns <see cref="PoolObject{TSource}" /> to specified <see cref="Pool{TSource}" /> using
        ///     <see cref="Pool{TSource}.Despawn(UnityExtensions.Utilities.PoolObject{TSource})" />. <br />
        ///     Default pool name is "", an empty string and retrieved with <see cref="Pools{TSource}.GetOrCreatePool" />.
        /// </summary>
        /// <param name="poolObject">Object instance.</param>
        /// <param name="name">Name of the pool.</param>
        /// <typeparam name="TSource">Type of the object.</typeparam>
        /// <returns>
        ///     true - despawned successfully <br />
        ///     false - couldn't despawn
        /// </returns>
        public static bool Despawn<TSource>(this PoolObject<TSource> poolObject, string name = "")
            where TSource : new() =>
            Pools<TSource>.GetOrCreatePool(name).Despawn(poolObject);

        /// <summary>
        ///     Despawns object into given pool.
        /// </summary>
        /// <param name="poolObject"></param>
        /// <param name="pool"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static bool Despawn<TSource>(this PoolObject<TSource> poolObject, Pool<TSource> pool)
            where TSource : new() =>
            pool.Despawn(poolObject);
    }
}