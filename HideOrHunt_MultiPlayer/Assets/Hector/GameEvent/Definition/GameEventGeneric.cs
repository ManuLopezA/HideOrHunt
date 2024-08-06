// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------
// Modifying for generic parameter use

using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<T> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener<T>> eventListeners =
        new List<GameEventListener<T>>();

    public void Raise(T parameter)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter);
    }

    public void RegisterListener(GameEventListener<T> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener<T0, T1>> eventListeners =
        new List<GameEventListener<T0, T1>>();

    public void Raise(T0 parameter0, T1 parameter1)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter0, parameter1);
    }

    public void RegisterListener(GameEventListener<T0, T1> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T0, T1> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1, T2> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener<T0, T1, T2>> eventListeners =
        new List<GameEventListener<T0, T1, T2>>();

    public void Raise(T0 parameter0, T1 parameter1, T2 parameter2)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter0, parameter1, parameter2);
    }

    public void RegisterListener(GameEventListener<T0, T1, T2> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T0, T1, T2> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1, T2, T3> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener<T0, T1, T2, T3>> eventListeners =
        new List<GameEventListener<T0, T1, T2, T3>>();

    public void Raise(T0 parameter0, T1 parameter1, T2 parameter2, T3 parameter3)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter0, parameter1, parameter2, parameter3);
    }

    public void RegisterListener(GameEventListener<T0, T1, T2, T3> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T0, T1, T2, T3> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
