using System;
using System.Collections.Generic;

namespace Scepix.Types;

/// <summary>
/// Represents a 2D integer point.
/// </summary>
public struct Vec2I : IEquatable<Vec2I>
{
    public int X;
    public int Y;

    public Vec2I(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public static Vec2I Zero      => new Vec2I(+0, +0);
    public static Vec2I Left      => new Vec2I(-1, +0);
    public static Vec2I Right     => new Vec2I(+1, +0);
    public static Vec2I Up        => new Vec2I(+0, -1);
    public static Vec2I Down      => new Vec2I(+0, +1);
    public static Vec2I UpLeft    => new Vec2I(-1, -1);
    public static Vec2I UpRight   => new Vec2I(+1, -1);
    public static Vec2I DownLeft  => new Vec2I(-1, +1);
    public static Vec2I DownRight => new Vec2I(+1, +1);
    
    public static IReadOnlyList<Vec2I> StarAxis { get; } =
    [
        Vec2I.Up,
        Vec2I.Right,
        Vec2I.Down,
        Vec2I.Left,
    ];
    
    public static IReadOnlyList<Vec2I> CrossAxis { get; } =
    [
        Vec2I.UpLeft,
        Vec2I.UpRight,
        Vec2I.DownRight,
        Vec2I.DownLeft,
    ];

    public static bool operator ==(Vec2I v1, Vec2I v2) => v1.Equals(v2);
    
    public static bool operator !=(Vec2I v1, Vec2I v2) => !v1.Equals(v2);
    
    public static Vec2I operator +(Vec2I v1, Vec2I v2) => new Vec2I(v1.X + v2.X, v1.Y + v2.Y);
    public static Vec2I operator +(Vec2I v1, int num) => new Vec2I(v1.X + num, v1.Y + num);
    
    public static Vec2I operator -(Vec2I v) => new Vec2I(-v.X, -v.Y);
    
    public static Vec2I operator -(Vec2I v1, Vec2I v2) => new Vec2I(v1.X - v2.X, v1.Y - v2.Y);
    public static Vec2I operator -(Vec2I v1, int num) => new Vec2I(v1.X - num, v1.Y - num);
    
    public static Vec2I operator *(Vec2I v1, Vec2I v2) => new Vec2I(v1.X * v2.X, v1.Y * v2.Y);
    public static Vec2I operator *(Vec2I v1, int num) => new Vec2I(v1.X * num, v1.Y * num);
    
    public static Vec2I operator /(Vec2I v1, Vec2I v2) => new Vec2I(v1.X / v2.X, v1.Y / v2.Y);
    public static Vec2I operator /(Vec2I v1, int num) => new Vec2I(v1.X / num, v1.Y / num);
    
    public bool Equals(Vec2I other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vec2I v && Equals(v);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString() => $"{{ {X}, {Y} }}";
}