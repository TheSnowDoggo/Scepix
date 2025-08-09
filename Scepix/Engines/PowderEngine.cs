using System;
using System.Collections.Generic;
using System.Text;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class PowderEngine() : TagEngine("powder")
{
    private class VariantCache(int density, bool anti, string routing)
    {
        public int Density { get; } = density;
        public bool Anti { get; } = anti;
        public string Routing { get; } = routing;
    }
    
    private readonly struct ValidInfo(PixelSpace space, PixelVariant variant, int density, Dictionary<PixelVariant, int> densityCache)
    {
        public PixelSpace Space { get; } = space;
        public PixelVariant Variant { get; } = variant;
        public int Density { get; } = density;
        public Dictionary<PixelVariant, int> DensityCache { get; } = densityCache;
    }
    
    private readonly Random _rand = new();
    
    private const string DensityTag = "density";

    private const string RoutingTag = "powder.routing";

    private const string AntiTag = "powder.anti";

    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        var variantCache = new Dictionary<PixelVariant, VariantCache>();

        var densityCache = new Dictionary<PixelVariant, int>();
        
        positions.Shuffle();
        
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
                
                var routing = data.Variant.DataTags.GetContentOrDefault<string>(RoutingTag, "rand");

                cache = new VariantCache(density, anti, routing);
                variantCache[data.Variant] = cache;
            }

            var info = new ValidInfo(space, data.Variant, cache.Density, densityCache);
            
            if (cache.Anti ? pos.Y == 0 : pos.Y == space.Height - 1)
            {
                continue;
            }

            var down = cache.Anti ? Vec2I.Up : Vec2I.Down;
            var downLeft = cache.Anti ? Vec2I.UpLeft : Vec2I.DownLeft;
            var downRight = cache.Anti ? Vec2I.UpRight : Vec2I.DownRight;

            if (Valid(pos + down, info))
            {
                space.Swap(pos, pos + down);
            }
            else
            {
                var leftClear = Valid(pos + downLeft, info);
                var rightClear = Valid(pos + downRight, info);

                bool left;
                switch (leftClear)
                {
                    case true when !rightClear:
                        left = true;
                        break;
                    case false when rightClear:
                        left = false;
                        break;
                    case true when rightClear:
                        left = cache.Routing switch
                        {
                            "rand" => _rand.NextBool(),
                            "left" => true,
                            "right" => false,
                            _ => throw new ArgumentException("Undefined routing mode.")
                        };
                        break;
                    default:
                        continue;
                }

                if (left)
                {
                    space.Swap(pos, pos + downLeft);
                }
                else
                {
                    space.Swap(pos, pos + downRight);
                }
            }
        }
    }
    
    private static bool Valid(Vec2I move, ValidInfo info)
    {
        if (!info.Space.TryGet(move, out var p))
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