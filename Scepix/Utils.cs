using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix;

public enum FillType
{
    Left,
    Right,
    CenterLb,
    CenterRb,
}

public static class Utils
{
    private static readonly Random _rand = new();
    
    #region Math

    /// <summary>
    /// Performs a modulo operation.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="n">The divisor</param>
    /// <typeparam name="T">The number type.</typeparam>
    /// <returns>The result of <paramref name="a"/> mod <paramref name="n"/>.</returns>
    public static T Mod<T>(T a, T n)
        where T : IModulusOperators<T, T, T>, 
        IAdditionOperators<T, T, T>,
        INumberBase<T>
    {
        var mod = a % n;
        return T.IsPositive(a) ? mod : mod + n;
    }

    /// <summary>
    /// Determines whether the given number is prime.
    /// </summary>
    /// <param name="number">The number to check.</param>
    /// <returns>true if the number is prime; otherwise, false.</returns>
    public static bool IsPrime(long number)
    {
        if (number == 2L)
        {
            return true;
        }

        if (number <= 1L || long.IsEvenInteger(number))
        {
            return false;
        }

        var sqrt = (long)Math.Sqrt(number);
        
        for (var i = 3L; i <= sqrt; i+=2L)
        {
            if (number % i == 0L)
            {
                return false;
            }
        }

        return true;
    }

    /// <see cref="IsPrime(long)"/>
    public static bool IsPrime(int number)
    {
        return IsPrime((long)number);
    }

    /// <summary>
    /// Performs a linear interpolation.
    /// </summary>
    /// <param name="t">The ratio between min and max.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The interpolated result.</returns>
    public static float Lerp(float t, float min, float max)
    {
        return min + (max - min) * t;
    }
    
    /// <see cref="Lerp(float,float,float)"/>
    public static double Lerp(double t, double min, double max)
    {
        return min + (max - min) * t;
    }
    
    /// <summary>
    /// Performs an inverse linear interpolation.
    /// </summary>
    /// <param name="value">The interpolated value.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The ratio between min and max of the value.</returns>
    public static float Unlerp(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
    
    /// <see cref="Unlerp(float,float,float)"/>
    public static double Unlerp(double value, double min, double max)
    {
        return (value - min) / (max - min);
    }
    
    private const float FRadDegFactor = 180 / MathF.PI;

    private const double DRadDegFactor = 180 / Math.PI;

    /// <summary>
    /// Converts from radians to degrees.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The resulting angle in degrees.</returns>
    public static float RadToDeg(float angle)
    {
        return angle * FRadDegFactor;
    }

    /// <summary>
    /// Converts from radians to degrees.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The resulting angle in degrees.</returns>
    public static double RadToDeg(double angle)
    {
        return angle * DRadDegFactor;
    }

    /// <summary>
    /// Converts from degrees to radians.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>The resulting angle in radians.</returns>
    public static float DegToRad(float angle)
    {
        return angle / FRadDegFactor;
    }

    /// <summary>
    /// Converts from degrees to radians.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>The resulting angle in radians.</returns>
    public static double DegToRad(double angle)
    {
        return angle / DRadDegFactor;
    }
    
    /// <summary>
    /// Gets the minimum and maximum of two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <param name="min">The minimum (preceding) number.</param>
    /// <param name="max">The maximum (following) number.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>true if the two numbers are equal; otherwise, false.</returns>
    public static bool NumMinMax<T>(T a, T b, out T min, out T max)
        where T : IComparisonOperators<T, T, bool>
    {
        if (a < b)
        {
            min = a;
            max = b;
        }
        else
        {
            min = b;
            max = a;
        }

        return a == b;
    }

    #endregion
    
    #region Copy
    
    /// <summary>
    /// Creates a new array of a given length filled with the item. .
    /// </summary>
    /// <param name="item">The item to fill.</param>
    /// <param name="length">The length of the array.</param>
    /// <typeparam name="T">The type of the new array.</typeparam>
    /// <returns>A new array of a given length filled with the item.</returns>
    public static T[] Copy<T>(T item, int length)
    {
        var arr = new T[length];
        for (var i = 0; i < length; ++i)
        {
            arr[i] = item;
        }
        return arr;
    }

    /// <summary>
    /// Creates a new array of a given length filled with the list of items in a repeating pattern.
    /// </summary>
    /// <param name="items">The items to fill with.</param>
    /// <param name="length">The length of the array.</param>
    /// <typeparam name="T">The type of the new array.</typeparam>
    /// <returns>A new array of a given length filled with the list of items in a repeating pattern.</returns>
    public static T[] CopyList<T>(IReadOnlyList<T> items, int length)
    {
        var arr = new T[length];
        var j = 0;
        for (var i = 0; i < arr.Length; ++i)
        {
            arr[i] = items[j++];

            if (j >= items.Count)
            {
                j = 0;
            }
        }
        return arr;
    }

    /// <summary>
    /// Creates a new string of a given length filled with the given char.
    /// </summary>
    /// <param name="character">The character to fill</param>
    /// <param name="length">The length of the new string</param>
    /// <returns>A new string of a given length filled with the given char.</returns>
    public static string CopyStr(char character, int length)
    {
        return new string(Copy(character, length));
    }

    /// <summary>
    /// Creates a new string of a given length with the string in a repeating pattern.
    /// </summary>
    /// <param name="str">The string to fill from.</param>
    /// <param name="length">The length of the new string.</param>
    /// <returns>A new string of a given length with the string in a repeating pattern.</returns>
    public static string CopyStr(string str, int length)
    {
        return new string(CopyList(str.ToCharArray(), length));
    }
    
    #endregion
    
    #region StringExtensions

    /// <summary>
    /// Extension method for <see cref="string.IsNullOrEmpty"/>.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
    public static bool IsNullOrEmpty(this string? source)
    {
        return string.IsNullOrEmpty(source);
    }

    /// <summary>
    /// Extension method for <see cref="string.IsNullOrWhiteSpace"/>
    /// </summary>
    /// <param name="source">The source string</param>
    /// <returns>true if the value parameter is null or Empty, or if value consists exclusively of white-space characters.</returns>
    public static bool IsNullOrWhitespace(this string?  source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    /// <summary>
    /// Determines whether the given string is a palindrome.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>true if the string is palindromic; otherwise, false.</returns>
    public static bool IsPalindrome(this string source)
    {
        var end = source.Length / 2;
        for (var i = 0; i < end; ++i)
        {
            if (source[i] != source[^(i + 1)])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Reverses the characters in the string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>A reversed string.</returns>
    public static string Reverse(this string source)
    {
        var arr = new char[source.Length];
        for (var i = 0; i < arr.Length; ++i)
        {
            arr[i] = source[^(i + 1)];
        }
        return new string(arr);
    }
    
    /// <summary>
    /// Fits the source string to the desired length.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="length">The desired length.</param>
    /// <param name="fill">The fill character.</param>
    /// <param name="mode">The fill mode.</param>
    /// <returns>The source string fit to the given length.</returns>
    public static string FitToLen(this string? source, int length, char fill = ' ', FillType mode = FillType.Right)
    {
        if (source == null)
        {
            return CopyStr(fill, length);
        }
        
        var dif = length - source.Length;

        if (dif >= 0)
        {
            return source[..length];
        }
        
        switch (mode)
        {
            case FillType.Left:
                return CopyStr(fill, dif) + source;
            case FillType.Right:
                return source + CopyStr(fill, dif);
            case FillType.CenterLb:
            case FillType.CenterRb:
                var first = mode == FillType.CenterRb || dif % 2 == 0 ? dif / 2 : dif / 2 + 1;
                return string.Join("", CopyStr(fill, first), source, CopyStr(fill, dif - first));
            default:
                throw new NotImplementedException("Undefined fill type.");
        }
    }

    /// <inheritdoc cref="FitToLen(string,int,char,Scepix.FillType)"/>    
    public static string FitToLen(this string? source, int length, FillType mode)
    {
        return FitToLen(source, length, ' ', mode);
    }
    
    #endregion
    
    #region IComparable
    
    /// <summary>
    /// Returns the minimum of the two values.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The value which precedes the other.</returns>
    public static T Min<T>(T a, T b)
        where T : IComparable<T>
    {
        return a.CompareTo(b) >= 0 ? b : a;
    }
    
    /// <summary>
    /// Returns the maximum of the two values.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The value which follows the other.</returns>
    public static T Max<T>(T a, T b)
        where T : IComparable<T>
    {
        return a.CompareTo(b) >= 0 ? a : b;
    }

    /// <summary>
    /// Gets the minimum and maximum of two values.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <param name="min">The minimum (preceding) value.</param>
    /// <param name="max">The maximum (following) value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>true if the two values neither precede nor follow each-other; otherwise, false.</returns>
    public static bool MinMax<T>(T a, T b, out T min, out T max)
        where T : IComparable<T>
    {
        switch (a.CompareTo(b))
        {
            case <0:
                min = a;
                max = b;
                return false;
            case 0:
                min = a;
                max = b;
                return true;
            case >0:
                min = b;
                max = a;
                return false;
        }
    }

    /// <summary>
    /// Clamps the given value between the minimum and maximum.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The clamped value.</returns>
    public static T Clamp<T>(T value, T min, T max)
        where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
        {
            throw new ArgumentException("Minimum value cannot proceed maximum.");
        }

        return Max(Min(value, max), min);
    }
    
    #endregion
    
    #region CollectionExtensions

    public static string ToPretty<T>(this IEnumerable<T> collection)
    {
        var sb = new StringBuilder("{ ");

        var first = true;
        
        foreach (var item in collection)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(", ");
            }

            sb.Append(item);
        }

        sb.Append(" }");

        return sb.ToString();
    }
    
    public static T Mean<T>(this IEnumerable<T> collection)
        where T : IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        INumberBase<T>
    {
        var sum = T.Zero;
        var count = T.Zero;
        
        foreach (var item in collection)
        {
            sum += item;
            ++count;
        }

        return sum / count;
    }
    
    /// <summary>
    /// Swaps the elements at the given indexes.
    /// </summary>
    /// <param name="list">The list to swap from.</param>
    /// <param name="index1">The first index.</param>
    /// <param name="index2">The second index.</param>
    /// <typeparam name="T">The type of the list.</typeparam>
    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }

    public static IEnumerable<Vec2I> EnumerateRect(int x, int y, int width, int height, bool rowMajor = false)
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
    
    public static IEnumerable<Vec2I> EnumerateRect(int width, int height, bool rowMajor = false)
    {
        return EnumerateRect(0, 0, width, height, rowMajor);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => _rand.NextDouble());
    }
    
    #endregion
}