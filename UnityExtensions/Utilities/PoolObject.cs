using System;

namespace UnityExtensions.Utilities;

/// <summary>
///     Wraps an instance to make it a <see cref="PoolObject{TSource}" />.
/// </summary>
/// <typeparam name="TSource"> Type of the instance to wrap. </typeparam>
public class PoolObject<TSource> where TSource : new()
{
    /// <summary>
    ///     Create a new <see cref="PoolObject{TSource}" />. <paramref name="value" /> cannot be null.
    /// </summary>
    /// <param name="value"> Instance of <typeparamref name="TSource" />. </param>
    /// <exception cref="ArgumentNullException"> Thrown when a null is passed as argument into the constructor. </exception>
    public PoolObject(TSource value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "It is not allowed to make a null into a PoolObject");
        Value = value;
    }

    /// <summary>
    ///     The wrapped instance.
    /// </summary>
    public TSource Value { get; }

    public static implicit operator TSource(PoolObject<TSource> poolObject) => poolObject.Value;
}