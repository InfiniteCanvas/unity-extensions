using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects;

/// <summary>
///     Consumes an <see cref="EventWrapper{TSource}" /> event and raises another one
/// </summary>
/// <typeparam name="TIncoming"> The type of <see cref="EventWrapper{TSource}" /> to consume </typeparam>
/// <typeparam name="TOutgoing"> The type of <see cref="EventWrapper{TSource}" /> to raise </typeparam>
public abstract class EventReactionBase<TIncoming, TOutgoing> : ScriptableObject
{
    /// <summary>
    ///     Incoming events to consume
    /// </summary>
    [OdinSerialize, Required]
    public EventWrapper<TIncoming> IncomingEvent { get; set; }

    /// <summary>
    ///     Event to raise in response to <see cref="IncomingEvent" />
    /// </summary>
    [OdinSerialize, Required]
    public EventWrapper<TOutgoing> OutgoingEvent { get; set; }

    /// <summary>
    ///     subscribe to <see cref="IncomingEvent" />
    /// </summary>
    protected virtual void OnEnable() => IncomingEvent.Event += IncomingEventHandler;

    /// <summary>
    ///     unsubscribe from <see cref="IncomingEvent" />
    /// </summary>
    protected virtual void OnDisable() => IncomingEvent.Event -= IncomingEventHandler;

    /// <summary>
    ///     Process <typeparamref name="TIncoming" /> from incoming event and produce <typeparamref name="TOutgoing" /> for the
    ///     outgoing event
    /// </summary>
    /// <param name="incoming"> incoming value from the event </param>
    /// <returns> instance of <typeparamref name="TOutgoing" /> </returns>
    protected abstract TOutgoing Process(in TIncoming incoming);

    /// <summary>
    ///     Receive the incoming event <br />
    ///     by default calls <see cref="EventWrapper{TSource}.Raise" /> on <see cref="OutgoingEvent" /> with the return value
    ///     of <see cref="Process" />
    /// </summary>
    /// <param name="sender"> The object that raised the event </param>
    /// <param name="incoming"> The passed in argument for the event </param>
    protected virtual void IncomingEventHandler(object sender, TIncoming incoming) =>
        OutgoingEvent.Raise(Process(incoming));
}