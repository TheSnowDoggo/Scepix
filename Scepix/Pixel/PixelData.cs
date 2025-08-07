using Scepix.Types;

namespace Scepix.Pixel;

public class PixelData
{
    public PixelData(PixelVariant variant)
    {
        Variant = variant;
    }
    
    public PixelVariant Variant { get; }

    public TagSet LocalTags { get; } = new();
}