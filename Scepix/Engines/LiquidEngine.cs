using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class LiquidEngine : TagEngine
{
    private readonly Random _rand = new();

    private readonly string _heading;

    private readonly string _density;

    public LiquidEngine()
        : base("liquid")
    {
        _heading = $"{Tag}.heading";
        _density = $"{Tag}.density";
    }

    public override void Update(double delta, IReadOnlyList<Vec2I> positions, VirtualGrid2D<PixelData?> grid)
    {
        foreach (var pos in positions.Shuffle().OrderBy(p => -p.Y))
        {
            if (pos.Y == grid.Height - 1 || grid[pos] is not {} data)
            {
                continue;
            }

            var density = data.Variant.Tags.GetContentOrDefault<int>(_density);

            if (Valid(pos + Vec2I.Down))
            {
                grid.Swap(pos, pos + Vec2I.Down);
                data.LocalTags.Remove(_heading);
            }
            else
            {
                if (!data.LocalTags.TryGetContent(_heading, out bool hDir) || !Valid(pos + Heading(hDir)))
                {
                    hDir = _rand.NextBool();
                    data.LocalTags[string.Intern(_heading)] = hDir;
                }

                var heading = Heading(hDir);

                var next = pos + heading;
                
                if (!Valid(next))
                {
                    continue;
                }

                var moved = false;
                for (var i = 1; i < 30; ++i)
                {
                    var move = next + Vec2I.Down + heading * i;
                    
                    if (!Valid(move))
                    {
                        continue;
                    }
                    
                    grid.Swap(pos, move);
                    moved = true;
                    break;
                }
                
                if (!moved)
                {
                    grid.Swap(pos, next);
                } 
            }

            continue;

            bool Valid(Vec2I t)
            {
                return grid.TryGet(t, out var p) && (p == null || (p.Variant != data.Variant && p.Variant.Tags.HasTag(Tag) && p.Variant.Tags.GetContentOrDefault<int>(_density) < density));
            }

            Vec2I Heading(bool heading)
            {
                return heading ? Vec2I.Left : Vec2I.Right;
            }
        }
    }
}