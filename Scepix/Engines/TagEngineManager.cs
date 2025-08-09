using System.Collections;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class TagEngineManager : IEnumerable<TagEngine>
{
    private readonly Dictionary<string, TagEngine> _engines = new();

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
    
    private static Dictionary<string, List<Coord>> QueryTagInfo(PixelSpace space)
    {
        var dict = new Dictionary<string, List<Coord>>();
        
        foreach (var pos in space.Positions)
        {
            if (space[pos] is not {} data)
            {
                continue;
            }
            
            foreach (var tag in data.Variant.EngineTags)
            {
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

        return dict;
    }

    public IEnumerator<TagEngine> GetEnumerator() => _engines.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}