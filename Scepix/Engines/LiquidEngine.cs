using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class LiquidEngine() : TagEngine("liquid")
{
    private readonly Random _rand = new();

    private const string DensityTag = "*.density";

    private const string HeadingTag = "liquid.heading";

    private const string AntiTag = "liquid.anti";

    public override void Update(double delta, IReadOnlyList<Vec2I> positions, PixelSpace space)
    {
        foreach (var pos in positions.Shuffle().OrderBy(p => -p.Y))
        {
            if (space[pos] is not {} data)
            {
                continue;
            }
            
            var anti = data.Variant.DataTags.HasTag(AntiTag);

            if (anti ? pos.Y == 0 : pos.Y == space.Height - 1)
            {
                continue;
            }

            var density = data.Variant.DataTags.GetContentOrDefault<int>(DensityTag);

            var down = anti ? Vec2I.Up : Vec2I.Down;

            if (Valid(pos + down))
            {
                space.Swap(pos, pos + down);
            }
            else
            {
                if (!data.LocalTags.TryGetContent<bool>(HeadingTag, out var hDir) || !Valid(pos + Heading(hDir)))
                {
                    hDir = _rand.NextBool();
                    data.LocalTags[HeadingTag] = hDir;
                }

                var heading =  Heading(hDir);

                var next = pos + heading;
                
                if (!Valid(next))
                {
                    continue;
                }

                var moved = false;
                for (var i = 1; i < 30; ++i)
                {
                    var move = next + down + heading * i;
                    
                    if (!Valide(move, out var p))
                    {
                        if (p != null && p.Variant != data.Variant)
                        {
                            break;
                        }
                        
                        continue;
                    }
                    
                    space.Swap(pos, move);
                    moved = true;
                    break;
                }
                
                if (!moved)
                {
                    space.Swap(pos, next);
                } 
            }

            continue;

            bool Valide(Vec2I t, out PixelData? p)
            {
                return space.TryGet(t, out p) && (p == null || (p.Variant != data.Variant && p.Variant.DataTags.GetContentOrDefault<int>(DensityTag) < density));
            }

            bool Valid(Vec2I t)
            {
                return Valide(t, out _);
            }
            
            Vec2I Heading(bool heading)
            {
                return heading ? Vec2I.Left : Vec2I.Right;
            }
        }
    }
}