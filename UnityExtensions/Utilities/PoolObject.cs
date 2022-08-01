namespace UnityExtensions.Utilities;

public class PoolObject<TSource> where TSource : new()
{
    public PoolObject(TSource value) => Value = value;

    public TSource Value { get; }

    public static implicit operator TSource(PoolObject<TSource> poolObject) => poolObject.Value;
}