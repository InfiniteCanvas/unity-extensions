using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityExtensions
{
    /// <summary>
    ///     Extends IEnumerable with useful methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Picks a random element from a collection.
        /// </summary>
        /// <param name="collection"> The collection to pick from. </param>
        /// <typeparam name="TSource"> The type of the elements in the collection. </typeparam>
        /// <returns> A random element from the collection. </returns>
        public static TSource RandomChoice<TSource>(this IEnumerable<TSource> collection)
        {
            TSource[] enumerable = collection as TSource[] ?? collection.ToArray();
            var random = new Random();
            return enumerable.ElementAt(random.Next(enumerable.Length));
        }

        /// <summary>
        ///     Picks multiple random elements from a collection.
        /// </summary>
        /// <param name="collection"> The collection to pick from. </param>
        /// <param name="amount"> The amount of elements to pick. </param>
        /// <param name="duplicates"> Whether or not to allow duplicates. </param>
        /// <typeparam name="TSource"> The type of the elements in the collection. </typeparam>
        /// <returns> Multiple random elements from the collection. </returns>
        public static IEnumerable<TSource> RandomChoice<TSource>(this IEnumerable<TSource> collection,
                                                                 int                       amount,
                                                                 bool                      duplicates = false)
        {
            TSource[] enumerable = collection as TSource[] ?? collection.ToArray();
            int[] indexList;
            var random = new Random();

            if (duplicates)
            {
                var indices = new List<int>();
                while (indices.Count < amount) indices.Add(random.Next(enumerable.Length));

                indexList = indices.ToArray();
            }
            else
            {
                var indices = new HashSet<int>();
                while (indices.Count < amount) indices.Add(random.Next(enumerable.Length));
                indexList = indices.ToArray();
            }

            for (var i = 0; i < amount; i++) yield return enumerable[indexList[i]];
        }

        /// <summary>
        ///     Applies a function to each element in a collection.
        /// </summary>
        /// <param name="collection"> The collection to apply the function to. </param>
        /// <param name="action"> The function to apply. </param>
        /// <typeparam name="TSource"> The type of the elements in the collection. </typeparam>
        public static void ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource> action)
        {
            foreach (var item in collection) action(item);
        }

        /// <summary>
        ///     Reduces the collection to a single value using the given function by applying the function to the previous result
        ///     and the current item. Basically, Linq's Aggregate, but from the left.
        /// </summary>
        /// <param name="collection"> The collection to reduce. </param>
        /// <param name="reduce"> The function to use to reduce the collection. </param>
        /// <param name="initial"> The initial value to use with the first item in the collection. </param>
        /// <typeparam name="TSource"> The type of the items in the collection. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <returns> The result of the reduction. </returns>
        private static TResult ReduceLeft<TSource, TResult>(this IEnumerable<TSource>       collection,
                                                            Func<TSource, TResult, TResult> reduce,
                                                            TResult                         initial)
        {
            var result = initial;
            foreach (var item in collection) result = reduce(item, result);
            return result;
        }

        /// <summary>
        ///     Reduces the collection to a single value using the given function by applying the function to the previous result
        ///     and the current item. Basically, Linq's Aggregate, but from the right (reverse).
        /// </summary>
        /// <param name="collection"> The collection to reduce. </param>
        /// <param name="reduce"> The function to use to reduce the collection. </param>
        /// <param name="initial"> The initial value to use with the first item in the collection. </param>
        /// <typeparam name="TSource"> The type of the items in the collection. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <returns> The result of the reduction. </returns>
        private static TResult ReduceRight<TSource, TResult>(this IEnumerable<TSource>       collection,
                                                             Func<TSource, TResult, TResult> reduce,
                                                             TResult                         initial)
        {
            var result = initial;
            foreach (var item in collection.Reverse()) result = reduce(item, result);
            return result;
        }

        /// <summary>
        ///     Reduces the collection to a single value using the given function by applying the function to the previous result
        ///     and the current item. Basically, Linq's Aggregate, but with direction specified.
        /// </summary>
        /// <param name="collection"> The collection to reduce. </param>
        /// <param name="reduce"> The function to use to reduce the collection. </param>
        /// <param name="initial"> The initial value to use with the first item in the collection. </param>
        /// <param name="fromLeft"> The direction to reduce the collection. Reduces in reverse (from right) if false. </param>
        /// <typeparam name="TSource"> The type of the items in the collection. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <returns> The result of the reduction. </returns>
        public static TResult Reduce<TSource, TResult>(this IEnumerable<TSource>       collection,
                                                       Func<TSource, TResult, TResult> reduce,
                                                       TResult                         initial,
                                                       bool                            fromLeft = true) =>
            fromLeft ? ReduceLeft(collection, reduce, initial) : ReduceRight(collection, reduce, initial);

        /// <summary>
        ///     Divides the collection into batches of the given size.
        /// </summary>
        /// <param name="collection"> The collection to divide. </param>
        /// <param name="batchSize"> The size of each batch. </param>
        /// <typeparam name="TSource"> The type of the items in the collection. </typeparam>
        /// <returns> The collection divided into batches. Last batch of remainders might be smaller. </returns>
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> collection,
                                                                       int                       batchSize)
        {
            ArraySegment<TSource> batch;
            TSource[] source = collection.ToArray();
            var batchCount = source.Length / batchSize;
            var remainder = source.Length  % batchSize;

            for (var currentBatch = 0; currentBatch < batchCount; currentBatch++)
            {
                batch = new ArraySegment<TSource>(source, currentBatch * batchSize, batchSize);
                yield return batch;
            }

            if (remainder > 0)
            {
                batch = new ArraySegment<TSource>(source, batchCount * batchSize, remainder);
                yield return batch;
            }
        }
    }
}