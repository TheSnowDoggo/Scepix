using SkiaSharp;

namespace Scepix.Pixel;

public class PixelVariant
{
    public static PixelVariant Empty { get; } = new PixelVariant();

    public SKColor Color { get; set; } = SKColors.Black;
}