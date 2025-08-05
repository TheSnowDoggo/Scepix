using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Scepix.Types;

namespace Scepix.Collections;

/// <summary>
/// Represents a 2D grid.
/// </summary>
/// <typeparam name="T">The type to store.</typeparam>
public class Grid2D<T> : IEnumerable<T>,
    ICloneable
{
    private T[,] _data;

    public Grid2D(int width, int height)
    {
        _data = new T[width, height];
    }
    
    public Grid2D(T[,] data)
    {
        _data = data;
    }
    
    public static explicit operator T[,](Grid2D<T> grid) => grid._data;
    
    public static explicit operator Grid2D<T>(T[,] data) => new Grid2D<T>(data);

    public static implicit operator Grid2DView<T>(Grid2D<T> grid) => grid.ToView();
    
    /// <summary>
    /// Gets the width of the grid.
    /// </summary>
    public int Width => _data.GetLength(0);
    
    /// <summary>
    /// Gets the height of the grid.
    /// </summary>
    public int Height => _data.GetLength(1);

    /// <summary>
    /// Gets the size of the grid.
    /// </summary>
    public int Size => _data.Length;

    /// <summary>
    /// Gets or sets an item from the given coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public T this[int x, int y]
    {
        get
        {
            if (!CoordinatesValid(x, y))
            {
                throw new ArgumentException($"{x},{y} is not a valid coordinate");
            }
            
            return _data[x, y];
        }
        set
        {
            if (!CoordinatesValid(x, y))
            {
                throw new  ArgumentException($"{x},{y} is not a valid coordinate");
            }
            
            _data[x, y] = value; 
        }
    }
    
    /// <summary>
    /// Gets or sets an item from the given coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate.</param>
    public T this[Vec2I coordinate]
    {
        get => this[coordinate.X, coordinate.Y];
        set => this[coordinate.X, coordinate.Y] = value; 
    }

    /// <summary>
    /// Resizes the grid to the given dimensions removing all previous data.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <exception cref="ArgumentException">Thrown if the width or height is negative.</exception>
    public void CleanResize(int width, int height)
    {
        if (width < 0 || height < 0)
        {
            throw new ArgumentException("Width and Height cannot be negative.");
        }
        
        _data = new T[width, height];
    }

    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_data);
    }

    /// <summary>
    /// Returns a readonly view of the grid.
    /// </summary>
    /// <returns>A readonly view of the grid.</returns>
    public Grid2DView<T> ToView()
    {
        return new Grid2DView<T>(this);
    }

    public IEnumerable<Vec2I> EnumerateRect(int x, int y, int width, int height, bool rowMajor = true)
    {
        var s1 = rowMajor ? x : y;
        var s2 = rowMajor ? y : x;
        
        var e1 = s1 + (rowMajor ? width : height);
        var e2 = s2 + (rowMajor ? height : width);

        for (var i = s1; i < e1; ++i)
        {
            for (var j = s2; j < e2; ++j)
            {
                yield return rowMajor ? new Vec2I(i, j) : new Vec2I(j, i);
            }
        }
    }
    
    public IEnumerable<Vec2I> EnumerateRect(int width, int height, bool rowMajor = true)
    {
        return EnumerateRect(0, 0, width, height, rowMajor);
    }

    public IEnumerable<Vec2I> Enumerate(bool rowMajor = true)
    {
        return EnumerateRect(Width, Height, rowMajor);
    }

    public void Fill(Func<Vec2I, T> fill, int x, int y, int width, int height)
    {
        foreach (var pos in EnumerateRect(x, y, width, height))
        {
            this[pos] = fill(pos);
        }
    }
    
    public void Fill(T value, int x, int y, int width, int height)
    {
        foreach (var pos in EnumerateRect(x, y, width, height))
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
        foreach (var pos in Enumerate())
        {
            this[pos] = value;
        }
    }
    
    public object Clone()
    {
        return new Grid2D<T>(_data);
    }

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_data.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        var sb = new StringBuilder("[ { ");

        for (var y = 0; y < Height; ++y)
        {
            if (y != 0)
            {
                sb.Append(" }, { ");
            }
                
            for (var x = 0; x < Width; ++x)
            {
                if (x != 0)
                {
                    sb.Append(", ");
                }
                sb.Append(this[x, y]);
            }
        }
        
        sb.Append(" } ]");
        return sb.ToString();
    }

    private bool CoordinatesValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}