using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class TagEngineManager : IEnumerable<TagEngine>
{
    private readonly Dictionary<string, TagEngine> _engines = new();

    private readonly Random _rand = new();

    private int _lazyFrameCounter = 1;

    public int LazyFrameUpdateRate { get; set; } = 1;

    public byte InitialAwakeTime { get; set; } = 60;

    public void Add(TagEngine engine)
    {
        _engines.Add(engine.Tag, engine);
    }

    public void Update(double delta, PixelSpace grid)
    {
        var dict = QueryTagInfo(grid);
        
        foreach (var (tag, list) in dict)
        {
            if (!_engines.TryGetValue(tag, out var engine))
            {
                continue;
            }
            
            engine.Update(delta, list, grid);
        }
    }
    
    private Dictionary<string, List<Coord>> QueryTagInfo(PixelSpace space)
    {
        var dict = new Dictionary<string, List<Coord>>();

        if (LazyFrameUpdateRate > 1 && _lazyFrameCounter >= LazyFrameUpdateRate)
        {
            _lazyFrameCounter = 0;
        }
        
        foreach (var pos in space.Positions)
        {
            if (space[pos] is not {} data)
            {
                continue;
            }

            if (LazyFrameUpdateRate > 1)
            {
                switch (data.LazyCounter)
                {
                    case 1:
                    {
                        if (data.LazyRank != _lazyFrameCounter)
                        {
                            continue;
                        }

                        break;
                    }
                    case 0:
                        data.LazyCounter = InitialAwakeTime > 0 ? InitialAwakeTime : (byte)1;
                        data.LazyRank = (byte)_rand.Next(LazyFrameUpdateRate);
                        break;
                    default:
                        --data.LazyCounter;
                        break;
                }
            }
            
            foreach (var tag in data.Variant.EngineTags)
            {
                if (!_engines.ContainsKey(tag))
                {
                    continue;
                }
                
                if (dict.TryGetValue(tag, out var list))
                {
                    list.Add((Coord)pos);
                }
                else
                {
                    dict[tag] = [(Coord)pos];
                }
            }
        }

        if (LazyFrameUpdateRate > 1)
        {
            ++_lazyFrameCounter;
        }

        return dict;
    }

    public IEnumerator<TagEngine> GetEnumerator() => _engines.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}