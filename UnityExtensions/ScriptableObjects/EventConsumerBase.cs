using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects;

/// <summary>
/// Consume an incoming event and process it
/// </summary>
/// <typeparam name="TIncoming"> The type of <see cref="EventWrapper{TSource}"/> to consume </typeparam>
public abstract class EventConsumerBase<TIncoming> : ScriptableObject
{
    /// <summary>
    ///     Incoming events to consume
    /// </summary>
    [OdinSerialize, Required]
    public EventWrapper<TIncoming> IncomingEvent { get; set; }

    /// <summary>
    ///     subscribe to <see cref="IncomingEvent" />
    /// </summary>
    protected virtual void OnEnable() => IncomingEvent.Event += IncomingEventHandler;

    /// <summary>
    ///     unsubscribe from <see cref="IncomingEvent" />
    /// </summary>
    protected virtual void OnDisable() => IncomingEvent.Event -= IncomingEventHandler;

    /// <summary>
    ///     Receive the incoming event <br />
    ///     by default calls <see cref="Process" />
    /// </summary>
    /// <param name="sender"> The object that raised the event </param>
    /// <param name="incoming"> The passed in argument for the event </param>
    protected virtual void IncomingEventHandler(object sender, TIncoming incoming) => Process(incoming);

    /// <summary>
    /// Processes <typeparamref name="TIncoming"/> from subscribed event
    /// </summary>
    /// <param name="incoming"> Event args broadcasted by the event </param>
    protected abstract void Process(in TIncoming incoming);
}