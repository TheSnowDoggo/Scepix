using System.Text;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix;

public static class Utils
{
    public static T As<T>(this object obj)
    {
        return (T)obj;
    }
    
    public static T[] Copy<T>(T item, int copies)
    {
        var arr = new T[copies];
        for (var i = 0; i < copies; ++i)
        {
            arr[i] = item;
        }
        return arr;
    }

    public static string ToPretty<TKey, TResult>(this Dictionary<TKey, TResult> dict)
        where TKey : notnull
    {
        if (dict.Count == 0)
        {
            return "[]";
        }
        
        var sb = new StringBuilder("[ ");

        var first = true;
        foreach (var pair in dict)
        {
            if (!first)
            {
                sb.Append(", ");
            }
            else
            {
                first = false;
            }

            sb.Append($"{{ {pair.Key}, {pair.Value} }}");
        }

        sb.Append(" ]");

        return sb.ToString();
    }

    public static void Swap<T>(this Grid2D<T> grid, Vec2I p1, Vec2I p2)
    {
        (grid[p1], grid[p2]) = (grid[p2], grid[p1]);
    }
}