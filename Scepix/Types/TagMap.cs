using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Scepix.Types;

/// <summary>
/// A class representing a string and tags.
/// </summary>
public class TagMap : IEnumerable<KeyValuePair<string, object?>>,
    ICloneable
{
    private readonly Dictionary<string, object?> _tags = new();

    public TagMap()
    {
    }

    public TagMap(IEnumerable<KeyValuePair<string, object?>> tags)
    {
        _tags = new Dictionary<string, object?>(tags);
    }
    
    /// <summary>
    /// Gets the number of tags stored.
    /// </summary>
    public int Count => _tags.Count;

    public Dictionary<string, object?>.KeyCollection Tags => _tags.Keys;
    
    public Dictionary<string, object?>.ValueCollection Contents => _tags.Values;

    /// <summary>
    /// Gets or sets the content of a specified tag.
    /// </summary>
    /// <param name="tag">The tag to get or set.</param>
    public object? this[string tag]
    {
        get => _tags[tag];
        set => _tags[tag] = value;
    }

    /// <summary>
    /// Adds a new tag and it's associated content.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <param name="value">The content of the tag.</param>
    public void Add(string tag, object value)
    {
        _tags.Add(tag, value);
    }
    
    /// <summary>
    /// Adds a new empty tag.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    public bool Add(string tag)
    {
        return _tags.TryAdd(tag, null);
    }

    /// <summary>
    /// Removes a given tag.
    /// </summary>
    /// <param name="tag">The name of the tag to remove.</param>
    /// <returns>true if the tag was successfully removed; otherwise, false.</returns>
    public bool Remove(string tag)
    {
        return _tags.Remove(tag);
    }

    /// <summary>
    /// Removes all the given tags.
    /// </summary>
    /// <param name="tags">The tags to remove.</param>
    /// <returns>true if all tags were successfully removed; otherwise, false.</returns>
    public bool Remove(params string[] tags)
    {
        var failed = false;
        foreach (var tag in tags)
        {
            if (!Remove(tag))
            {
                failed = true;
            }
        }
        return failed;
    }

    /// <summary>
    /// Clears all tags
    /// </summary>
    public void Clear()
    {
        _tags.Clear();
    }

    /// <summary>
    /// Determines whether this contains a specified tag.
    /// </summary>
    /// <param name="tag">The name of the tag to look for.</param>
    /// <returns>true if this contains the specified tag; otherwise, false</returns>
    public bool Contains(string tag)
    {
        return _tags.ContainsKey(tag);
    }

    public T Get<T>(string tag)
    {
        return (T)(_tags[tag] ?? throw new NullReferenceException("Tag has no content."));
    }

    /// <summary>
    /// Gets the content associated with the specified tag.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <param name="value">The content associated with the tag.</param>
    /// <returns>true if the specified tag exists; otherwise, false</returns>
    public bool TryGetValue(string tag, [NotNullWhen(true)] out object? value)
    {
        return _tags.TryGetValue(tag, out value);
    }

    /// <summary>
    /// Gets the content associated with the specified tag to unbox type T.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <param name="value">The content associated with the tag.</param>
    /// <typeparam name="T">The type to unbox the contents to.</typeparam>
    /// <returns>true if the specified tag exists; otherwise, false</returns>
    public bool TryGetValue<T>(string tag, [MaybeNullWhen(false)] out T value)
    {
        var res = TryGetValue(tag, out var obj);
        value = obj == null ? default : (T)obj;
        return res;
    }

    public object? GetContentOrDefault(string tag)
    {
        return TryGetValue(tag, out var obj) ? obj : null;
    }
    
    public T GetContentOrDefault<T>(string tag, T defaultValue)
    {
        return TryGetValue<T>(tag, out var t) ? t : defaultValue;
    }
    
    public T? GetContentOrDefault<T>(string tag)
    {
        return TryGetValue<T>(tag, out var t) ? t : default;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _tags.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public override string ToString() => $"{_tags.ToPretty()}";

    public object Clone()
    {
        return  new TagMap(_tags);
    }
}