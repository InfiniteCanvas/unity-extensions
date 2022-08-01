using System;

namespace UnityExtensions;

/// <summary>
///     Extends some Actions with some useful methods.
/// </summary>
public static class ActionExtensions
{
    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <returns> The wrapped action. </returns>
    public static Action Decorate(this Action action, Action<Action> decorator) => () => decorator(action);

    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <returns> The wrapped action. </returns>
    public static Action<TSource> Decorate<TSource>(this Action<TSource> action, Action<Action<TSource>> decorator) =>
        input => decorator(_ => action(input));

    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <typeparam name="T1"> The type of the first parameter of the action. </typeparam>
    /// <typeparam name="T2"> The type of the second parameter of the action. </typeparam>
    /// <returns> The wrapped action. </returns>
    public static Action<T1, T2> Decorate<T1, T2>(this Action<T1, T2> action, Action<Action<T1, T2>> decorator) =>
        (input1, input2) => decorator((_, _) => action(input1, input2));

    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <typeparam name="T1"> The type of the first parameter of the action. </typeparam>
    /// <typeparam name="T2"> The type of the second parameter of the action. </typeparam>
    /// <typeparam name="T3"> The type of the third parameter of the action. </typeparam>
    /// <returns> The wrapped action. </returns>
    public static Action<T1, T2, T3> Decorate<T1, T2, T3>(this Action<T1, T2, T3>    action,
                                                          Action<Action<T1, T2, T3>> decorator) =>
        (input1, input2, input3) => decorator((_, _, _) => action(input1, input2, input3));

    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <typeparam name="T1"> The type of the first parameter of the action. </typeparam>
    /// <typeparam name="T2"> The type of the second parameter of the action. </typeparam>
    /// <typeparam name="T3"> The type of the third parameter of the action. </typeparam>
    /// <typeparam name="T4"> The type of the fourth parameter of the action. </typeparam>
    /// <returns> The wrapped action. </returns>
    public static Action<T1, T2, T3, T4> Decorate<T1, T2, T3, T4>(this Action<T1, T2, T3, T4>    action,
                                                                  Action<Action<T1, T2, T3, T4>> decorator) =>
        (input1, input2, input3, input4) => decorator((_, _, _, _) => action(input1, input2, input3, input4));

    /// <summary>
    ///     Wraps an action in another action.
    /// </summary>
    /// <param name="action"> The action to wrap. </param>
    /// <param name="decorator"> The action to wrap the original action in. </param>
    /// <typeparam name="T1"> The type of the first parameter of the action. </typeparam>
    /// <typeparam name="T2"> The type of the second parameter of the action. </typeparam>
    /// <typeparam name="T3"> The type of the third parameter of the action. </typeparam>
    /// <typeparam name="T4"> The type of the fourth parameter of the action. </typeparam>
    /// <typeparam name="T5"> The type of the fifth parameter of the action. </typeparam>
    /// <returns> The wrapped action. </returns>
    public static Action<T1, T2, T3, T4, T5> Decorate<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action,
                                                                          Action<Action<T1, T2, T3, T4, T5>>
                                                                              decorator) =>
        (input1, input2, input3, input4, input5) =>
            decorator((_, _, _, _, _) => action(input1, input2, input3, input4, input5));
}