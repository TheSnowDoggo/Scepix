using System.Collections.Generic;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class FlammableEngine() : TagEngine("flammable")
{
    private const string FireTag = "flammable.onfire";
    
    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        positions.Shuffle();
        
        foreach (Vec2I pos in positions)
        {
            if (space[pos] is not { } data)
            {
                continue;
            }

            foreach (var axis in Vec2I.AllAxis)
            {
                var next = pos + axis;

                if (!space.TryGet(next, out var p) || p == null || 
                    !p.IsEngine(Tag) || !p.LocalTags.Contains(FireTag))
                {
                    continue;
                }

                data.LocalTags.Add(FireTag);
            }
        }
    }
}