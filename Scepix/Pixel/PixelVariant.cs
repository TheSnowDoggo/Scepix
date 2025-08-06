using SkiaSharp;
using Scepix.Types;

namespace Scepix.Pixel;

public class PixelVariant
{
    public SKColor Color { get; set; } = SKColors.Black;

    public TagSet Tags { get; init; } = new();
}