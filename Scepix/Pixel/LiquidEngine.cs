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

    public override void Update(double delta, List<Vec2I> positions, Grid2D<PixelData?> grid)
    {
        foreach (var pos in positions.AsEnumerable().Reverse())
        {
            if (pos.Y == grid.Height - 1 || grid[pos] is not {} data)
            {
                continue;
            }

            if (grid[pos + Vec2I.Down] == null)
            {
                grid.Swap(pos, pos  + Vec2I.Down);
            }
            else
            {
                if (!data.LocalTags.TryGetContent("heading", out Vec2I heading) || !grid.TryGet(pos + heading, out var p) || p != null)
                {
                    heading = _rand.Next(2) == 0 ? Vec2I.Left : Vec2I.Right;
                    data.LocalTags["heading"] = heading;
                }
                
                if (!grid.TryGet(pos + heading, out p) || p != null)
                {
                    continue;
                }
                    
                grid.Swap(pos, pos + heading);
            }
        }
    }
}