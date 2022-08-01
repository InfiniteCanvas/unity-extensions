using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects;

public abstract class Wrapper<TSource> : ScriptableObject, IEquatable<Wrapper<TSource>>
{
    [OdinSerialize]
    public TSource Value { get; set; }

    public Wrapper(TSource value) { Value = value; }

    public static implicit operator TSource(Wrapper<TSource> source) => source.Value;

    private static bool Equals(Wrapper<TSource>? a, Wrapper<TSource>? b) =>
        (a, b) switch
        {
            (null, _) => false,
            (_, null) => false,
            (_, _) => ReferenceEquals(a, b)
                   || EqualityComparer<TSource>.Default
                                               .Equals(a.Value, b.Value),
        };

    public bool Equals(Wrapper<TSource>? other) =>
        (this, other) switch
        {
            (null, _) => false,
            (_, null) => false,
            (_, _) => ReferenceEquals(this, other)
                   || EqualityComparer<TSource>.Default
                                               .Equals(Value, other.Value),
        };

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Wrapper<TSource>)obj);
    }

    public override int GetHashCode()
    {
        unchecked { return (base.GetHashCode() * 397) ^ EqualityComparer<TSource>.Default.GetHashCode(Value); }
    }

    public static bool operator ==(Wrapper<TSource>? left, Wrapper<TSource>? right) => Equals(left, right);

    public static bool operator !=(Wrapper<TSource>? left, Wrapper<TSource>? right) => !Equals(left, right);
}