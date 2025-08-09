using System;

namespace Scepix.Types;

public struct Coord(ushort x, ushort y) : IEquatable<Coord>
{
    public ushort X = x;
    public ushort Y = y;
    
    public static bool operator ==(Coord left, Coord right) => left.Equals(right);
    public static bool operator !=(Coord left, Coord right) => !left.Equals(right);
    
    public static implicit operator Vec2I(Coord coord) => new Vec2I(coord.X, coord.Y);
    
    public static explicit operator Coord(Vec2I vec) => new Coord((ushort)vec.X, (ushort)vec.Y);
    
    public bool Equals(Coord other) => other.X == X && other.Y == Y;

    public override bool Equals(object? obj) => obj is Coord coord && Equals(coord);
    
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"{X},{Y}";
}