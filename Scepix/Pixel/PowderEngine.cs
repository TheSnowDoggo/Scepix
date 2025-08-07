using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public class PowderEngine : TagEngine
{
    private readonly Random _rand = new();
    
    public PowderEngine()
        : base("powder")
    {
    }
    
    public override void Update(double delta, IReadOnlyList<Vec2I> positions, VirtualGrid2D<PixelData?> grid)
    {
        var moveDict = new Dictionary<Vec2I, Vec2I>();
        
        foreach (var pos in positions.Shuffle())
        {
            if (pos.Y == grid.Height - 1 || grid[pos] is not {} data)
            {
                continue;
            }

            if (grid[pos + Vec2I.Down] is not {} p || p.Variant.Tags.HasTag("liquid"))
            {
                grid.Swap(pos, pos + Vec2I.Down);
            }
            else
            {
                var leftClear = grid.TryGet(pos + Vec2I.DownLeft, out p) && (p == null || p.Variant.Tags.HasTag("liquid"));
                var rightClear = grid.TryGet(pos + Vec2I.DownRight, out p) && (p == null || p.Variant.Tags.HasTag("liquid"));

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
                        if (!data.Variant.Tags.TryGetContent<string>($"{Tag}.routing", out var routing))
                        {
                            routing = "rand";
                        }
                        
                        left = routing switch
                        {
                            "rand" => _rand.Next(2) == 0,
                            "left" => true,
                            "right" => false,
                            _ => throw new NotImplementedException("Undefined routing mode.")
                        };
                        break;
                    default:
                        continue;
                }

                if (left)
                {
                    grid.Swap(pos, pos + Vec2I.DownLeft);
                }
                else
                {
                    grid.Swap(pos, pos + Vec2I.DownRight);
                }
            }
            
        }
    }
}