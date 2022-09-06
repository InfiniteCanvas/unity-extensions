using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects
{
    [Serializable]
    public abstract class Wrapper<TSource> : ScriptableObject
    {
        public Wrapper(TSource value) => Value = value;

        [OdinSerialize, InlineEditor] public TSource Value { get; set; }

        public static implicit operator TSource(Wrapper<TSource> source) => source.Value;
    }
}