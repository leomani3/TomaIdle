using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class DataVariable<T> : ScriptableObject
{
    [SerializeField] private T value;

    public Action OnValueChanged;

    public T Value
    {
        get => value;
        set => SetValue(value);
    }

    [Button]
    public void SetValue(T newValue)
    {
        if (!Equals(this.value, newValue))
        {
            this.value = newValue;
            OnValueChanged.Invoke();
        }
    }

    public void ForceNotify() => OnValueChanged.Invoke();
}