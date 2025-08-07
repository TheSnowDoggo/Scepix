using System;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class DecayEngine : TagEngine
{
    private readonly Random _rand = new();
    
    public DecayEngine()
        : base("decay")
    {
    }
    
    public override void Update(double delta, IReadOnlyList<Vec2I> positions, VirtualGrid2D<PixelData?> grid)
    {
        foreach (var pos in positions)
        {
            var data = grid[pos];
            
            if (data == null)
            {
                continue;
            }

            var decayRate = 0.01;
            if (data.Variant.Tags.TryGetContent(Tag, out double decay))
            {
                decayRate = decay;
            }

            if (_rand.NextDouble() < decayRate)
            {
                grid[pos] = null;
            }
        }
    }
}