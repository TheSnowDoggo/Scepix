using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public class PixelSpace : VirtualGrid2D<PixelData?>
{
    private readonly HashSet<Vec2I> _changes = [];
    
    public PixelSpace(int width, int height)
        : base(width, height)
    {
    }

    public bool LogChanges { get; set; } = true;

    public NameDict<PixelVariant> Variants { get; init; } = [];

    protected override void Set(Vec2I coordinate, PixelData? value)
    {
        base.Set(coordinate, value);

        if (LogChanges)
        {
            _changes.Add(coordinate);
        }
    }
    
    public IEnumerable<Vec2I> Changes => _changes;

    public void ClearChanges()
    {
        _changes.Clear();
    }

    public PixelData Make(string variant)
    {
        return new PixelData(Variants[variant]);
    }
}