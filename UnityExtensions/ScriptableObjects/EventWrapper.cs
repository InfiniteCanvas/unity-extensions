using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects
{
    /// <summary>
    ///     Wraps events for <typeparamref name="TSource" /> in a <see cref="ScriptableObject" />.
    /// </summary>
    /// <typeparam name="TSource"> Event type </typeparam>
    public abstract class EventWrapper<TSource> : ScriptableObject
    {
        /// <summary>
        ///     true - use the data defined in <see cref="FixedData" /> <br />
        ///     false - use the data passed in by <see cref="Raise" />
        /// </summary>
        [ToggleGroup("UseFixedData")]
        public bool UseFixedData;

        /// <summary>
        ///     Data to be used in a <see cref="Raise" /> call when <see cref="UseFixedData" /> is true
        /// </summary>
        [HideLabel, ToggleGroup("UseFixedData"), InlineEditor]
        public TSource FixedData;

        /// <summary>
        ///     <see cref="EventHandler{TEventArgs}" />. Subscribe/Unsubscribe here.
        /// </summary>
        public event EventHandler<TSource>? Event;

        /// <summary>
        ///     Raise the event using <paramref name="source" /> or <see cref="FixedData" /> depending on
        ///     <see cref="UseFixedData" />
        /// </summary>
        /// <param name="source"> Value to pass into the raised event </param>
        public virtual void Raise(in TSource source) => Event?.Invoke(this, UseFixedData ? FixedData : source);
    }
}