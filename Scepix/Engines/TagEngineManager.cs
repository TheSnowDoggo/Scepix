using System;
using System.Collections;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class TagEngineManager : IEnumerable<TagEngine>
{
    private readonly Dictionary<string, TagEngine> _engines = new();

    public int LazyFrameUpdateRate { get; set; } = 3;

    private int _lazyFrameCounter = 1;
    
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

        if (_lazyFrameCounter >= LazyFrameUpdateRate)
        {
            _lazyFrameCounter = 0;
        }
        
        foreach (var pos in space.Positions)
        {
            if (space[pos] is not {} data)
            {
                continue;
            }

            switch (data.LazyCounter)
            {
                case 1:
                {
                    if (_lazyFrameCounter != 0)
                    {
                        continue;
                    }

                    break;
                }
                case 0:
                    data.LazyCounter = 1;
                    break;
                default:
                    --data.LazyCounter;
                    break;
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
        
        ++_lazyFrameCounter;

        return dict;
    }

    public IEnumerator<TagEngine> GetEnumerator() => _engines.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}