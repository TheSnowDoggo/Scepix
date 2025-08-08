using System.Collections.Generic;
using SkiaSharp;
using Scepix.Types;
using Scepix.Collections;

namespace Scepix.Pixel;

public class PixelVariant : INameIdentifiable
{
    public PixelVariant(string name)
    {
        Name = name;
    }
    
    public string Name { get; }
    
    public SKColor Color { get; init; } = SKColors.Black;

    public HashSet<string> EngineTags { get; init; } = [];
    
    public TagMap DataTags { get; init; } = new();
}