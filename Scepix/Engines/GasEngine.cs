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
        Vec2I.Down,
        Vec2I.Left,
        Vec2I.UpLeft,
        Vec2I.UpRight,
        Vec2I.DownRight,
        Vec2I.DownLeft,
    ];

    private readonly Random _rand = new();
    
    public override void Update(double delta, IReadOnlyList<Vec2I> positions, PixelSpace space)
    {
        foreach (var pos in positions.Shuffle())
        {
            if (space[pos] == null || _rand.Next(10) != 0)
            {
                continue;
            }

            var next = pos + Axis[_rand.Next(4)];

            if (!space.TryGet(next, out var p) || p != null)
            {
                continue;
            }
            
            space.Swap(pos, next);
        }
    }
}