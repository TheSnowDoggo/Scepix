using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix;

public static class ScepixExtensions
{
    /// <summary>
    /// Swaps the elements at the given positions in the grid.
    /// </summary>
    /// <param name="grid">The grid to swap from.</param>
    /// <param name="p1">The first index.</param>
    /// <param name="p2">The second index.</param>
    /// <typeparam name="T">The type of the grid.</typeparam>
    public static void Swap<T>(this Grid2D<T> grid, Vec2I p1, Vec2I p2)
    {
        (grid[p1], grid[p2]) = (grid[p2], grid[p1]);
    }
    
    /// <summary>
    /// Swaps the elements at the given positions in the grid.
    /// </summary>
    /// <param name="grid">The grid to swap from.</param>
    /// <param name="p1">The first index.</param>
    /// <param name="p2">The second index.</param>
    /// <typeparam name="T">The type of the grid.</typeparam>
    public static void Swap<T>(this VirtualGrid2D<T> grid, Vec2I p1, Vec2I p2)
    {
        (grid[p1], grid[p2]) = (grid[p2], grid[p1]);
    }
}