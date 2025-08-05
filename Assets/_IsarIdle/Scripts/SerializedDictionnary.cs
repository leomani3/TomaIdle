using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> Dictionary => dictionary;

    // Unity calls this before serialization
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    // Unity calls this after deserialization
    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        int count = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            if (!dictionary.ContainsKey(keys[i]))
                dictionary.Add(keys[i], values[i]);
        }
    }

    // Optional helper methods
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public void Add(TKey key, TValue value) => dictionary.Add(key, value);

    public bool Remove(TKey key) => dictionary.Remove(key);

    public void Clear()
    {
        dictionary.Clear();
        keys.Clear();
        values.Clear();
    }

    public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;

    public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
}