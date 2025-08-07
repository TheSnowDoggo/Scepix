using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Scepix.Types;

/// <summary>
/// A class representing a string and tags.
/// </summary>
public class TagSet : IEnumerable<KeyValuePair<string, object?>>,
    ICloneable
{
    private readonly Dictionary<string, object?> _tags = new();

    public TagSet()
    {
    }

    public TagSet(IEnumerable<KeyValuePair<string, object?>> tags)
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
    /// <param name="content">The content of the tag.</param>
    public void Add(string tag, object content)
    {
        _tags.Add(tag, content);
    }
    
    /// <summary>
    /// Adds a new empty tag.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    public void Add(string tag)
    {
        _tags.Add(tag, null);
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
    public bool HasTag(string tag)
    {
        return _tags.ContainsKey(tag);
    }

    /// <summary>
    /// Returns whether the given tag has content.
    /// </summary>
    /// <param name="tag">The tag to check.</param>
    /// <returns>true if the tag has content; otherwise, false.</returns>
    public bool HasContent(string tag)
    {
        return this[tag] != null;
    }

    public T Get<T>(string tag)
    {
        return (T)(_tags[tag] ?? throw new NullReferenceException("Tag has no content."));
    }

    /// <summary>
    /// Gets the content associated with the specified tag.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <param name="content">The content associated with the tag.</param>
    /// <returns>true if the specified tag exists; otherwise, false</returns>
    public bool TryGetContent(string tag, [NotNullWhen(true)] out object? content)
    {
        return _tags.TryGetValue(tag, out content);
    }

    /// <summary>
    /// Gets the content associated with the specified tag to unbox type T.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <param name="content">The content associated with the tag.</param>
    /// <typeparam name="T">The type to unbox the contents to.</typeparam>
    /// <returns>true if the specified tag exists; otherwise, false</returns>
    public bool TryGetContent<T>(string tag, [MaybeNullWhen(false)] out T? content)
    {
        var res = TryGetContent(tag, out var obj);
        content = obj == null ? default : (T)obj;
        return res;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _tags.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public override string ToString() => $"{_tags.ToPretty()}";

    public object Clone()
    {
        return  new TagSet(_tags);
    }
}