using System;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class DecayEngine() : TagEngine("decay")
{
    private readonly Random _rand = new();

    public override void Update(double delta, IReadOnlyList<Vec2I> positions, PixelSpace space)
    {
        foreach (var pos in positions)
        {
            var data = space[pos];
            
            if (data == null)
            {
                continue;
            }

            var decayRate = 0.01;
            if (data.Variant.DataTags.TryGetContent(Tag, out double decay))
            {
                decayRate = decay;
            }

            if (_rand.NextDouble() < decayRate)
            {
                space[pos] = null;
            }
        }
    }
}