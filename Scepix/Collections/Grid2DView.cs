using System;
using System.Collections;
using System.Collections.Generic;
using Scepix.Types;

namespace Scepix.Collections;

/// <summary>
/// Represents a readonly view of a <see cref="Grid2D{T}"/>
/// </summary>
/// <typeparam name="T">The type of the grid.</typeparam>
public class Grid2DView<T>(Grid2D<T> _grid) : IEnumerable<T>,
    ICloneable
{
    /// <inheritdoc cref="Grid2D{T}.Width"/>
    public int Width => _grid.Width;

    /// <inheritdoc cref="Grid2D{T}.Height"/>
    public int Height => _grid.Height;

    /// <inheritdoc cref="Grid2D{T}.Size"/>
    public int Size => _grid.Size;

    /// <summary>
    /// Gets an item from the given coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public T this[int x, int y] => _grid[x, y];
    
    /// <summary>
    /// Gets an item from the given coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate.</param>
    public T this[Vec2I coordinate] => _grid[coordinate];

    /// <inheritdoc cref="Grid2D{T}.Enumerate(bool)"/>
    public IEnumerable<Vec2I> Enumerate(bool rowMajor = false) => _grid.Enumerate(rowMajor);

    /// <inheritdoc cref="Grid2D{T}.Clone"/>
    public object Clone() => _grid.Clone();

    public IEnumerator<T> GetEnumerator() => _grid.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => _grid.GetEnumerator();

    public override string ToString() => _grid.ToString();
}