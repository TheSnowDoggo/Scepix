using System;
using System.Collections.Generic;
using System.Linq;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class WetEngine() : TagEngine("wet")
{
    public enum AxisType
    {
        StarAxis,
        CrossAxis,
        AllAxis,
    }
    
    public class Recipe(string result)
    {
        public string Result { get; } = result;

        public double MinTime { get; init; } = 0;

        public double MaxTime { get; init; } = 0;
    }

    private class VariantCache(Dictionary<string, Recipe> recipes, IReadOnlyList<Vec2I> neighbors)
    {
        public Dictionary<string, Recipe> Recipes { get; } = recipes;

        public IReadOnlyList<Vec2I> Neighbors { get; } = neighbors;
    }

    private readonly Random _rand = new();
    
    private const string RecipesTag = "wet.recipes";

    private const string AxisTag = "wet.axis";

    private const string DurationTag = "wet.duration";
    
    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        var variantCache = new Dictionary<PixelVariant, VariantCache?>();
        
        foreach (Vec2I pos in positions)
        {
            if (space[pos] is not {} data || (variantCache.TryGetValue(data.Variant, out var cache) && cache == null))
            {
                continue;
            }

            if (cache == null)
            {
                if (!data.Variant.DataTags.TryGetContent<Dictionary<string, Recipe>>(RecipesTag, out var recipes))
                {
                    variantCache[data.Variant] = null;
                    continue;
                }
                
                var neighbors = data.Variant.DataTags.GetContentOrDefault<AxisType>(AxisTag) switch
                {
                    AxisType.StarAxis => Vec2I.StarAxis,
                    AxisType.CrossAxis => Vec2I.CrossAxis,
                    AxisType.AllAxis => Vec2I.StarAxis.Concat(Vec2I.CrossAxis).ToList(),
                    _ => throw new ArgumentException("Unknown axis type")
                };
                
                cache = new VariantCache(recipes, neighbors);
                variantCache[data.Variant] = cache;
            }
            
            foreach (var axis in cache.Neighbors)
            {
                if (!space.TryGet(pos + axis, out var p) || p == null || 
                    !cache.Recipes.TryGetValue(p.Variant.Name, out var result))
                {
                    continue;
                }
                
                if (!space.Variants.TryGetItem(result.Result, out var variant))
                {
                    throw new ArgumentException("Variant contained invalid recipe.");
                }

                if (result is { MinTime: >= 0, MaxTime: >= 0 } and not { MinTime: 0, MaxTime: 0 })
                {
                    if (!data.LocalTags.TryGetContent<double>(DurationTag, out var duration))
                    {
                        var time = result.MinTime > result.MaxTime ? result.MinTime : 
                            Utils.Lerp(_rand.NextDouble(), result.MinTime, result.MaxTime);
                        
                        data.LocalTags.Add(DurationTag, time);
                        continue;
                    }

                    if (duration > 0)
                    {
                        data.LocalTags[DurationTag] = duration - delta;
                        continue;
                    }

                    data.LocalTags.Remove(DurationTag);
                }
                
                space[pos] = new PixelData(variant);

                break;
            }
        }
    }
}