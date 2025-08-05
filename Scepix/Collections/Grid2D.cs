using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
    /// Gets or sets the item at the given position.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public T this[int x, int y]
    {
        get => _data[x, y];
        set => _data[x, y] = value; 
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
}