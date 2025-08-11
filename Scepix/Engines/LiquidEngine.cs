using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class LiquidEngine() : TagEngine("liquid")
{
    private class VariantCache(int density, int spill, bool anti)
    {
        public int Density { get; } = density;
        public int Spill { get; } = spill;
        public bool Anti { get; } = anti;
        public Vec2I Down { get; } = anti ? Vec2I.Up : Vec2I.Down;
    }

    private readonly struct ValidInfo(PixelSpace space, PixelVariant variant, int density, Dictionary<PixelVariant, int?> densityCache)
    {
        public PixelSpace Space { get; } = space;
        public PixelVariant Variant { get; } = variant;
        public int Density { get; } = density;
        public Dictionary<PixelVariant, int?> DensityCache { get; } = densityCache;
    }
    
    private readonly Random _rand = new();
    
    public const int DefaultSpill = 30;
    
    private const byte AwakeTicks = byte.MaxValue;

    private const string DensityTag = "density";

    private const string HeadingTag = "liquid.heading";

    private const string HeadingLeftTag = "liquid.heading.left";

    public const string AntiTag = "liquid.anti";

    public const string SpillTag = "liquid.spill";

    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        var variantCache = new Dictionary<PixelVariant, VariantCache>();

        var densityCache = new Dictionary<PixelVariant, int?>();
        
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
                
                var spill = data.Variant.DataTags.GetContentOrDefault(SpillTag, DefaultSpill);
                
                var anti = data.Variant.DataTags.Contains(AntiTag);
                
                cache = new VariantCache(density, spill, anti);
                variantCache[data.Variant] = cache;
            }

            var info = new ValidInfo(space, data.Variant, cache.Density, densityCache);

            var next = pos + cache.Down;
            
            if (Valid(next, info, out _))
            {
                space.Swap(pos, next);
                data.LocalTags.Remove(HeadingTag, HeadingLeftTag);
                data.LazyCounter = AwakeTicks;
                continue;
            }
            
            if (!data.LocalTags.Contains(HeadingTag) ||
                !Valid(pos + (data.LocalTags.Contains(HeadingLeftTag) ? Vec2I.Left : Vec2I.Right), info, out _))
            {
                if (_rand.NextBool())
                {
                    data.LocalTags.Add(HeadingLeftTag);
                }
                else
                {
                    data.LocalTags.Remove(HeadingLeftTag);
                }
            }

            var heading = data.LocalTags.Contains(HeadingLeftTag) ? Vec2I.Left : Vec2I.Right;

            next = pos + heading;
                
            if (!Valid(next, info, out _))
            {
                continue;
            }
                
            var moved = false;

            for (var i = 1; i < cache.Spill; ++i)
            {
                var move = next + cache.Down + heading * i;

                if (!Valid(move, info, out var p))
                {
                    if (p != null && p.Variant != data.Variant)
                    {
                        break;
                    }
                        
                    continue;
                }
                    
                space.Swap(pos, move);
                data.LazyCounter = AwakeTicks;
                moved = true;
                break;
            }

            if (moved)
            {
                continue;
            }
            
            space.Swap(pos, next);
            data.LazyCounter = AwakeTicks;
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

        if (!info.DensityCache.TryGetValue(p.Variant, out var density))
        {
            density = p.Variant.DataTags.GetContentOrDefault<int?>(DensityTag, null);
            info.DensityCache[p.Variant] = density;
        }
        
        if (density == null)
        {
            return false;
        }

        return density < info.Density;
    }
}