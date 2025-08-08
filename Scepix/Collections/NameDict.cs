using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Scepix.Collections;

public class NameDict<T> : IEnumerable<T> where T : INameIdentifiable
{
    private readonly Dictionary<string, T> _dict = new();

    public NameDict()
    {
    }

    public NameDict(int capacity)
    {
        _dict = new Dictionary<string, T>(capacity);
    }

    public NameDict(IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public T this[string name] => _dict[name];

    /// <summary>
    /// Gets a collection containing all the items.
    /// </summary>
    public IEnumerable<T> Items => _dict.Values;

    /// <summary>
    /// Gets the number of items.
    /// </summary>
    public int Count => _dict.Count;
    
    /// <summary>
    /// Gets the current capacity.
    /// </summary>
    public int Capacity => _dict.Capacity;

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        _dict.Add(item.Name, item);
    }

    /// <summary>
    /// Attempts to add the specified item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>true if the item was successfully added; otherwise, false</returns>
    public bool TryAdd(T item)
    {
        return _dict.TryAdd(item.Name, item);
    }
    
    /// <summary>
    /// Removes the item with the specified name.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <returns>true if the item with the specified name was successfully found and removed; otherwise, false.</returns>
    public bool Remove(string name)
    {
        return _dict.Remove(name);
    }

    /// <summary>
    /// Removes all items.
    /// </summary>
    public void Clear()
    {
        _dict.Clear();
    }

    /// <summary>
    /// Determines whether an item with the given name is contained.
    /// </summary>
    /// <param name="name">The name to look for.</param>
    /// <returns>true if a matching item is found; otherwise, false.</returns>
    public bool Contains(string name)
    {
        return _dict.ContainsKey(name);
    }

    /// <summary>
    /// Gets the item associated with the given name.
    /// </summary>
    /// <param name="name">The name of the item to look for.</param>
    /// <param name="item">The item if found or default.</param>
    /// <returns>true if a matching item is found; otherwise, false.</returns>
    public bool TryGetItem(string name, [MaybeNullWhen(false)]out T item)
    {
        return _dict.TryGetValue(name, out item);
    }

    /// <summary>
    /// Tries to get the item associated with the specified name.
    /// </summary>
    /// <param name="name">The name to look for.</param>
    /// <param name="defaultValue">The value to return if the item was not found.</param>
    /// <returns>the item if found; otherwise, <paramref name="defaultValue"/></returns>
    public T GetItemOrDefault(string name, T defaultValue)
    {
        return _dict.GetValueOrDefault(name, defaultValue);
    }
    
    /// <summary>
    ///  Tries to get the item associated with the specified name.
    /// </summary>
    /// <param name="name">The name to look for.</param>
    /// <returns>the item if found; otherwise, the default value.</returns>
    public T? GetItemOrDefault(string name)
    {
        return _dict.GetValueOrDefault(name);
    }

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}