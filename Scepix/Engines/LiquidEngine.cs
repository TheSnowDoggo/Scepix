using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class LiquidEngine() : TagEngine("liquid")
{
    private class VariantCache(int density, bool anti, int spill)
    {
        public int Density { get; } = density;
        public bool Anti { get; } = anti;
        public int Spill { get; } = spill;
    }

    private readonly struct ValidInfo(PixelSpace space, PixelVariant variant, int density, Dictionary<PixelVariant, int> densityCache)
    {
        public PixelSpace Space { get; } = space;
        public PixelVariant Variant { get; } = variant;
        public int Density { get; } = density;
        public Dictionary<PixelVariant, int> DensityCache { get; } = densityCache;
    }
    
    private readonly Random _rand = new();
    
    public const int DefaultSpill = 30;

    private const string DensityTag = "density";

    private const string HeadingTag = "liquid.heading";

    private const string AntiTag = "liquid.anti";

    private const string SpillTag = "liquid.spill";

    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        var variantCache = new Dictionary<PixelVariant, VariantCache>();

        var densityCache = new Dictionary<PixelVariant, int>();
        
        positions.Shuffle();
        positions.Sort((a, b) => b.Y - a.Y);

        foreach (Vec2I pos in positions)
        {
            if (space[pos] is not {} data)
            {
                continue;
            }

            if (!variantCache.TryGetValue(data.Variant, out var cache))
            {
                var density = data.Variant.DataTags.GetContentOrDefault<int>(DensityTag);
                
                var anti = data.Variant.DataTags.HasTag(AntiTag);
                
                var spill = data.Variant.DataTags.GetContentOrDefault(SpillTag, DefaultSpill);
                
                cache = new VariantCache(density, anti, spill);
                variantCache[data.Variant] = cache;
            }

            var info = new ValidInfo(space, data.Variant, cache.Density, densityCache);
            
            if (cache.Anti ? pos.Y == 0 : pos.Y == space.Height - 1)
            {
                continue;
            }

            var down = cache.Anti ? Vec2I.Up : Vec2I.Down;

            if (Valid(pos + down, info, out _))
            {
                space.Swap(pos, pos + down);
                data.LocalTags.Remove(HeadingTag);
            }
            else
            {
                if (!data.LocalTags.TryGetContent<bool>(HeadingTag, out var hDir) ||
                    !Valid(pos + (hDir ? Vec2I.Left : Vec2I.Right), info, out _))
                {
                    hDir = _rand.NextBool();
                    data.LocalTags[HeadingTag] = hDir;
                }

                var heading = hDir ? Vec2I.Left : Vec2I.Right;

                var next = pos + heading;
                
                if (!Valid(next, info, out _))
                {
                    continue;
                }
                
                var moved = false;
                for (var i = 1; i < cache.Spill; ++i)
                {
                    var move = next + down + heading * i;
                    
                    if (!Valid(move, info, out var p))
                    {
                        if (p != null && p.Variant != data.Variant)
                        {
                            break;
                        }
                        
                        continue;
                    }
                    
                    space.Swap(pos, move);
                    moved = true;
                    break;
                }
                
                if (!moved)
                {
                    space.Swap(pos, next);
                } 
            }
        }
    }
    
    private static bool Valid(Vec2I t, ValidInfo info, out PixelData? p)
    {
        if (!info.Space.TryGet(t, out p))
        {
            return false;
        }

        if (p == null)
        {
            return true;
        }

        if (p.Variant == info.Variant)
        {
            return false;
        }

        if (info.DensityCache.TryGetValue(p.Variant, out var density))
        {
            return density < info.Density;
        }
        
        density = p.Variant.DataTags.GetContentOrDefault<int>(DensityTag);
        info.DensityCache[p.Variant] = density;

        return density < info.Density;
    }
}