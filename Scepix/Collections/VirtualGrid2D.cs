using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Scepix.Types;

namespace Scepix.Collections;

/// <summary>
/// A class representing a virtual grid that only allocates memory for stored positions.
/// </summary>
public class VirtualGrid2D<T> : IReadOnlyGrid<T>
{
    private readonly Dictionary<Vec2I, T> _data = new();

    public VirtualGrid2D(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public VirtualGrid2D(int width, int height, IEnumerable<KeyValuePair<Vec2I, T>> collection)
        : this(width, height)
    {
        _data = new Dictionary<Vec2I, T>(collection);
    }
    
    public int Width { get; }
    
    public int Height { get; }
    
    public int Count => _data.Count;

    public T this[Vec2I coordinate]
    {
        get => (InRange(coordinate) ? _data.GetValueOrDefault(coordinate) : 
            throw new ArgumentException($"{coordinate} is not a valid coordinate"))!;
        set => Set(coordinate, value);
    }

    public T this[int x, int y]
    {
        get => this[new Vec2I(x, y)];
        set => this[new Vec2I(x, y)] = value;
    }
    
    public static implicit operator GridView<T>(VirtualGrid2D<T> grid) => grid.ToView();

    protected virtual void Set(Vec2I coordinate, T value)
    {
        if (!InRange(coordinate))
        {
            throw new ArgumentException($"{coordinate} is not a valid coordinate");
        }

        if (value != null)
        {
            _data[coordinate] = value;
        }
        else
        {
            _data.Remove(coordinate);
        }
    }
    
    /// <summary>
    /// Determines whether the given coordinates are contained within the grid.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>true if the given coordinates are in range; otherwise, false</returns>
    public bool InRange(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
    
    /// <summary>
    /// Determines whether the given coordinate is contained within the grid.
    /// </summary>
    /// <param name="pos">The coordinate.</param>
    /// <returns>true if the given coordinates are in range; otherwise, false</returns>
    public bool InRange(Vec2I pos)
    {
        return InRange(pos.X, pos.Y);
    }
    
    /// <summary>
    /// Gets the item at the given coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="value">The item at the coordinate or default if coordinates are invalid.</param>
    /// <returns>true if the given coordinates are valid; otherwise, false</returns>
    public bool TryGet(int x, int y, [MaybeNullWhen(false)] out T value)
    {
        if (!InRange(x, y))
        {
            value = default;
            return false;
        }

        value = this[x, y];
        return true;
    }
    
    /// <summary>
    /// Gets the item at the given coordinate.
    /// </summary>
    /// <param name="pos">The coordinates.</param>
    /// <param name="value">The item at the coordinate or default if coordinates are invalid.</param>
    /// <returns>true if the given coordinates are valid; otherwise, false</returns>
    public bool TryGet(Vec2I pos, [MaybeNullWhen(false)] out T value)
    {
        return TryGet(pos.X, pos.Y, out value);
    }
    
    /// <summary>
    /// Returns a readonly view of the grid.
    /// </summary>
    /// <returns>A readonly view of the grid.</returns>
    public GridView<T> ToView()
    {
        return new GridView<T>(this);
    }

    public IEnumerable<Vec2I> EnumerateFilled() => from pos in _data.Keys select pos;
    
    public void Fill(Func<Vec2I, T> fill, int x, int y, int width, int height)
    {
        foreach (var pos in Utils.EnumerateRect(x, y, width, height))
        {
            this[pos] = fill(pos);
        }
    }
    
    public void Fill(T value, int x, int y, int width, int height)
    {
        foreach (var pos in Utils.EnumerateRect(x, y, width, height))
        {
            this[pos] = value;
        }
    }

    /// <summary>
    /// Fills the grid with the given value.
    /// </summary>
    /// <param name="value">The value to fill.</param>
    public void Fill(T value)
    {
        Fill(value, 0, 0, Width, Height);
    }
    
    public object Clone() =>  new VirtualGrid2D<T>(Width, Height, _data);

    public IEnumerator<T> GetEnumerator() => (from item in _data.Values select item).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}