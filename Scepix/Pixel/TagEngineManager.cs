using System.Collections;
using System.Collections.Generic;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public class TagEngineManager : IEnumerable<TagEngine>
{
    private readonly Dictionary<string, TagEngine> _engines = new();

    public void Add(TagEngine engine)
    {
        _engines.Add(engine.Tag, engine);
    }

    public void Update(double delta, VirtualGrid2D<PixelData?> grid)
    {
        var dict = QueryTagInfo(grid);
        
        foreach (var (tag, list) in dict)
        {
            if (!_engines.TryGetValue(tag, out var engine))
            {
                continue;
            }
            
            engine.Update(delta, list.AsReadOnly(), grid);
        }
    }
    
    private static Dictionary<string, List<Vec2I>> QueryTagInfo(VirtualGrid2D<PixelData?> grid)
    {
        var dict = new Dictionary<string, List<Vec2I>>();
        
        foreach (var pos in grid.EnumerateFilled())
        {
            if (grid[pos] is not {} data)
            {
                continue;
            }
            
            foreach (var tag in data.Variant.Tags.Tags)
            {
                if (dict.TryGetValue(tag, out var list))
                {
                    list.Add(pos);
                }
                else
                {
                    dict[tag] = [ pos ];
                }
            }
        }

        return dict;
    }

    public IEnumerator<TagEngine> GetEnumerator() => _engines.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}