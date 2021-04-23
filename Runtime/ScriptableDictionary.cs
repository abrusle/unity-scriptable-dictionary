using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableDictionaries.Internal;

namespace ScriptableDictionaries.Internal
{
    public abstract class ScriptableDictionaryBase : ScriptableObject
    {

    }
}

public class ScriptableDictionary<TKey, TValue> : ScriptableDictionaryBase,
    IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>
{
    [SerializeField] private List<TKey> keys;
    [SerializeField] private List<TValue> values;

    private void Awake()
    {
        if (keys == null)
        {
            keys = new List<TKey>(0);
            values = new List<TValue>(0);
        }
        else
        {
            if (values == null)
            {
                values = new List<TValue>(keys.Count);
            }
            else // Both list have values
            {
                int diff = keys.Count - values.Count;
                if (diff > 0) // more keys than values
                {
                    values.AddRange(new TValue[diff]);
                }
                else if (diff < 0) // more values than keys
                {
                    values.RemoveRange(values.Count + diff, -diff - 1);
                }
            }
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        int count = keys.Count;
        var pairs = new List<KeyValuePair<TKey, TValue>>(count);
        for (int i = 0; i < count; i++)
        {
            pairs[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }

        return pairs.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int keyIndex = keys.IndexOf(item.Key);
        if (keyIndex < 0 || keyIndex > values.Count - 1)
            return false;
        
        return values[keyIndex].Equals(item.Value);
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int startIndex)
    {
        if (array == null)
            throw new ArgumentNullException();
        if (startIndex < 0 || startIndex > array.Length)
            throw new ArgumentOutOfRangeException();
        if (array.Length - startIndex < Count)
            throw new ArgumentException("Cannot perform operation because startIndex is too small.");
        
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new KeyValuePair<TKey, TValue>(keys[startIndex + i], values[startIndex + i]);
        }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int keyIndex = keys.IndexOf(item.Key);
        if (keyIndex < 0 || keyIndex > values.Count - 1)
            return false;
        
        keys.RemoveAt(keyIndex);
        values.RemoveAt(keyIndex);
        return true;
    }

    /// <inheritdoc cref="ICollection{T}.Count" />
    public int Count => keys.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key), "Cannot add null key.");
        
        if (ContainsKey(key))
            throw new ArgumentException("Key already exist in this dictionary");
        
        keys.Add(key);
        values.Add(value);
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey" />
    public bool ContainsKey(TKey key)
    {
        return keys.Contains(key);
    }

    /// <inheritdoc />
    public bool Remove(TKey key)
    {
        return Remove(new KeyValuePair<TKey, TValue>(key, this[key]));
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue" />
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (ContainsKey(key))
        {
            value = this[key];
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.this" />
    public TValue this[TKey key]
    {
        get => values[keys.IndexOf(key)];
        set => values[keys.IndexOf(key)] = value;
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys => keys;

    /// <inheritdoc />
    public ICollection<TValue> Values => values;

    /// <inheritdoc/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    
    /// <inheritdoc/>
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
}