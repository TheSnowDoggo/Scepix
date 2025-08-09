using System.Collections.Generic;
using Scepix.Types;

namespace Scepix.Pixel;

public class PixelData
{
    public PixelData(PixelVariant variant)
    {
        Variant = variant;
    }
    
    public PixelVariant Variant { get; }

    public TagMap LocalTags { get; } = new TagMap();

    public byte LazyCounter = 0;
    
    public bool IsEngine(string tag)
    {
        return Variant.EngineTags.Contains(tag);
    }
}