using System;

namespace UnityExtensions
{
    /// <summary>
    ///     Extends booleans with some useful methods.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        ///     Decides which function to return based on the value of the boolean.
        /// </summary>
        /// <param name="either"> The boolean used to decide. </param>
        /// <param name="onTrue"> The function to return if the boolean is true. </param>
        /// <param name="onFalse"> The function to return if the boolean is false. </param>
        /// <typeparam name="TSource"> The type of the input for the function. </typeparam>
        /// <typeparam name="TResult"> The type of the output for the function. </typeparam>
        /// <returns> The function to return. </returns>
        public static Func<TSource, TResult> Either<TSource, TResult>(this bool              either,
                                                                      Func<TSource, TResult> onTrue,
                                                                      Func<TSource, TResult> onFalse) =>
            either switch
            {
                true  => onTrue,
                false => onFalse,
            };

        /// <summary>
        ///     Decides which function to execute based on the value of the boolean.
        /// </summary>
        /// <param name="either"> The boolean used to decide. </param>
        /// <param name="onTrue"> The function to use if the boolean is true. </param>
        /// <param name="onFalse"> The function to use if the boolean is false. </param>
        /// <typeparam name="TResult"> The type of the output for the function. </typeparam>
        /// <returns> Output of chosen function. </returns>
        public static TResult Either<TResult>(this bool     either,
                                              Func<TResult> onTrue,
                                              Func<TResult> onFalse) =>
            either switch
            {
                true  => onTrue(),
                false => onFalse(),
            };

        /// <summary>
        ///     Decides which action to execute based on the value of the boolean.
        /// </summary>
        /// <param name="either"> The boolean used to decide. </param>
        /// <param name="onTrue"> The action to use if the boolean is true. </param>
        /// <param name="onFalse"> The action to use if the boolean is false. </param>
        public static void Either(this bool either, Action onTrue, Action onFalse)
        {
            if (either) onTrue();
            else onFalse();
        }
    }
}