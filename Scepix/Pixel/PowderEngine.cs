using System.Collections.Generic;
using System.Linq;
using Scepix.Collections;
using Scepix.Types;

namespace Scepix.Pixel;

public class PowderEngine : TagEngine
{
    public PowderEngine()
        : base("powder")
    {
    }
    
    public override void Update(double delta, List<Vec2I> positions, Grid2D<PixelData?> grid)
    {
        foreach (var pos in positions.AsEnumerable().Reverse())
        {
            if (pos.Y == grid.Height - 1 || grid[pos] == null)
            {
                continue;
            }

            if (grid[pos + Vec2I.Down] == null)
            {
                grid.Swap(pos, pos + Vec2I.Down);
            }
            else if (grid.TryGet(pos + Vec2I.DownLeft, out var p) && p == null)
            {
                grid.Swap(pos, pos + Vec2I.DownLeft);
            }
            else if (grid.TryGet(pos + Vec2I.DownRight, out p) && p == null)
            {
                grid.Swap(pos, pos + Vec2I.DownRight);
            }
        }
    }
}