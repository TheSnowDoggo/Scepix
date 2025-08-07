using System;
using System.Text;
using System.Collections.Generic;

namespace Scepix.Collections;

public interface IReadOnlyGrid<T>  : IEnumerable<T>,
    ICloneable
{
    int Width { get; }
    int Height { get; }
    T this[int x, int y] { get; }

    string? ToString()
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