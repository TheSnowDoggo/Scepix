using System;
using System.Collections.Generic;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class GasEngine() : TagEngine("gas")
{
    private static IReadOnlyList<Vec2I> Axis { get; } =
    [
        Vec2I.Up,
        Vec2I.Right,
        Vec2I.Left,
        Vec2I.UpLeft,
        Vec2I.UpRight,
    ];

    private readonly Random _rand = new();

    private const byte AwakeTicks = 10;
    
    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        positions.Shuffle();
        
        foreach (Vec2I pos in positions)
        {
            if (space[pos] is not {} data)
            {
                continue;
            }

            var next = pos + Axis[_rand.Next(Axis.Count)];

            if (!space.TryGet(next, out var p) || p != null)
            {
                continue;
            }
            
            space.Swap(pos, next);
            data.LazyCounter = AwakeTicks;
        }
    }
}