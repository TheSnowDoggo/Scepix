using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public abstract class TagEngine
{
    public TagEngine(string tag)
    {
        Tag = tag;
    }
    
    public string Tag { get; }
    
    public abstract void Update(double delta, IReadOnlyList<Vec2I> positions, VirtualGrid2D<PixelData?> grid);
}