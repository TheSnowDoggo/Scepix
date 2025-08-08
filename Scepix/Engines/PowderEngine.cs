using System;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class PowderEngine() : TagEngine("powder")
{
    private readonly Random _rand = new();
    
    private const string DensityTag = "*.density";

    private const string RoutingTag = "powder.routing";

    private const string AntiTag = "powder.anti";

    public override void Update(double delta, IReadOnlyList<Vec2I> positions, PixelSpace space)
    {
        foreach (var pos in positions.Shuffle())
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
            var downLeft = anti ? Vec2I.UpLeft : Vec2I.DownLeft;
            var downRight = anti ? Vec2I.UpRight : Vec2I.DownRight;

            if (Valid(pos + down))
            {
                space.Swap(pos, pos + down);
            }
            else
            {
                var leftClear = Valid(pos + downLeft);
                var rightClear = Valid(pos + downRight);

                bool left;
                switch (leftClear)
                {
                    case true when !rightClear:
                        left = true;
                        break;
                    case false when rightClear:
                        left = false;
                        break;
                    case true when rightClear:
                        var routing = data.Variant.DataTags.GetContentOrDefault<string>(RoutingTag, "rand");
                        
                        left = routing switch
                        {
                            "rand" => _rand.NextBool(),
                            "left" => true,
                            "right" => false,
                            _ => throw new ArgumentException("Undefined routing mode.")
                        };
                        break;
                    default:
                        continue;
                }

                if (left)
                {
                    space.Swap(pos, pos + downLeft);
                }
                else
                {
                    space.Swap(pos, pos + downRight);
                }
            }

            continue;

            bool Valid(Vec2I move)
            {
                return space.TryGet(move, out var p) &&
                       (p == null || density > p.Variant.DataTags.GetContentOrDefault<int>("*.density"));
            }
        }
    }
}