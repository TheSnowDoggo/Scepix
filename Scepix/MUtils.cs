using System;
using System.Numerics;

namespace Scepix;

/// <summary>
/// A static class containing math utility functions.
/// </summary>
public static class MUtils
{
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

    public static double CircleSolveX(double r, double y, double a, double b)
    {
        return Math.Sqrt(r*r - y*y + 2*b*y - b*b) + a;
    }
    
    public static double CircleSolveX(double r, double y)
    {
        return Math.Sqrt(r * r - y * y);
    }
    
    public static double CircleSolveY(double r, double x, double a, double b)
    {
        return Math.Sqrt(r*r - x*x + 2*a*x - a*a) + b;
    }
    
    public static double CircleSolveY(double r, double x)
    {
        return Math.Sqrt(r * r - x * x);
    }
}