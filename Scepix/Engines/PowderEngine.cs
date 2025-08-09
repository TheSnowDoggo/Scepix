using System;
using System.Collections.Generic;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class PowderEngine() : TagEngine("powder")
{
    public enum RoutingMode
    {
        Random,
        Left,
        Right,
    }
    
    private class VariantCache(int density, RoutingMode routing, bool anti)
    {
        public int Density { get; } = density;
        public RoutingMode Routing { get; } = routing;
        public bool Anti { get; } = anti;
        public Vec2I Down { get; } = anti ? Vec2I.Up : Vec2I.Down;
        public Vec2I DownLeft { get; } = anti ? Vec2I.UpLeft : Vec2I.DownLeft;
        public Vec2I DownRight { get; } = anti ? Vec2I.UpRight : Vec2I.DownRight;
    }
    
    private readonly struct ValidInfo(PixelSpace space, PixelVariant variant, int density, Dictionary<PixelVariant, int> densityCache)
    {
        public PixelSpace Space { get; } = space;
        public PixelVariant Variant { get; } = variant;
        public int Density { get; } = density;
        public Dictionary<PixelVariant, int> DensityCache { get; } = densityCache;
    }

    private readonly Random _rand = new();
    
    private const byte AwakeTicks = 10;

    private const RoutingMode DefaultRouting = RoutingMode.Random;
    
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
                
                var routing = data.Variant.DataTags.GetContentOrDefault(RoutingTag, DefaultRouting);
                
                var anti = data.Variant.DataTags.Contains(AntiTag);

                cache = new VariantCache(density, routing, anti);
                variantCache[data.Variant] = cache;
            }

            if (cache.Anti ? pos.Y == 0 : pos.Y == space.Height - 1)
            {
                continue;
            }
            
            var info = new ValidInfo(space, data.Variant, cache.Density, densityCache);

            var next = pos + cache.Down;
            
            if (Valid(next, info))
            {
                space.Swap(pos, next);
                data.LazyCounter = AwakeTicks;
                continue;
            }

            var leftMove = pos + cache.DownLeft;
            var leftClear = Valid(leftMove, info);
            
            var rightMove = pos + cache.DownRight;
            var rightClear = Valid(rightMove, info);

            bool goLeft;
            switch (leftClear)
            {
                case true when !rightClear:
                    goLeft = true;
                    break;
                case false when rightClear:
                    goLeft = false;
                    break;
                case true when rightClear:
                    goLeft = cache.Routing switch
                    {
                        RoutingMode.Random => _rand.NextBool(),
                        RoutingMode.Left => true,
                        RoutingMode.Right => false,
                        _ => throw new ArgumentException("Undefined routing mode.")
                    };
                    break;
                default:
                    continue;
            }

            space.Swap(pos, goLeft ? leftMove : rightMove);
            data.LazyCounter = AwakeTicks;
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