namespace UnityExtensions.Utilities;

/// <summary>
///     Wraps an instance to make it a PoolObject.
/// </summary>
/// <typeparam name="TSource"> Type of the instance to wrap. </typeparam>
public class PoolObject<TSource> where TSource : new()
{
    public PoolObject(TSource value) => Value = value;
    
    public TSource Value { get; }

    public static implicit operator TSource(PoolObject<TSource> poolObject) => poolObject.Value;
}