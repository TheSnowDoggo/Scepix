using System;
using System.Collections;
using System.Collections.Generic;
using Scepix.Types;

namespace Scepix.Collections;

/// <summary>
/// Represents a readonly view of a <see cref="Grid2D{T}"/>
/// </summary>
/// <typeparam name="T">The type of the grid.</typeparam>
public class GridView<T>(IReadOnlyGrid<T> grid) : IEnumerable<T>,
    ICloneable
{
    /// <inheritdoc cref="Grid2D{T}.Width"/>
    public int Width => grid.Width;

    /// <inheritdoc cref="Grid2D{T}.Height"/>
    public int Height => grid.Height;

    /// <summary>
    /// Gets an item from the given coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public T this[int x, int y] => grid[x, y];
    
    /// <summary>
    /// Gets an item from the given coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate.</param>
    public T this[Vec2I coordinate] => this[coordinate.X, coordinate.Y];

    public IEnumerable<Vec2I> Enumerate(bool rowMajor = false)
    {
        return Utils.EnumerateRect(Width, Height, rowMajor);
    }

    /// <inheritdoc cref="Grid2D{T}.Clone"/>
    public object Clone() => grid.Clone();

    public IEnumerator<T> GetEnumerator() => grid.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => grid.GetEnumerator();

    public override string? ToString() => grid.ToString();
}