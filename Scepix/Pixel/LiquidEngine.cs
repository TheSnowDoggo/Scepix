using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public class LiquidEngine : TagEngine
{
    private readonly Random _rand = new();

    public LiquidEngine()
        : base("liquid")
    {
    }

    public override void Update(double delta, IReadOnlyList<Vec2I> positions, VirtualGrid2D<PixelData?> grid)
    {
        foreach (var pos in positions.Shuffle().OrderBy(p => -p.Y))
        {
            if (pos.Y == grid.Height - 1 || grid[pos] is not {} data)
            {
                continue;
            }

            if (grid[pos + Vec2I.Down] == null)
            {
                grid.Swap(pos, pos + Vec2I.Down);
            }
            else
            {
                if (!data.LocalTags.TryGetContent($"{Tag}.heading", out Vec2I heading) || !grid.TryGet(pos + heading, out var p) || p != null)
                {
                    heading = _rand.Next(2) == 0 ? Vec2I.Left : Vec2I.Right;
                    data.LocalTags[$"{Tag}.heading"] = heading;
                }

                var next = pos + heading;
                
                if (!grid.TryGet(next, out p) || p != null)
                {
                    continue;
                }

                if (!grid.TryGet(next + Vec2I.Down, out p) || p == null)
                {
                    grid.Swap(pos, next + Vec2I.Down);
                }
                else
                {
                    grid.Swap(pos, next);
                } 
            }
        }
    }
}