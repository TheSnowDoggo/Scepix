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
            var data = grid[pos];
            
            if (pos.Y == grid.Height - 1 || data == null)
            {
                continue;
            }

            if (grid[pos.X, pos.Y + 1] == null)
            {
                grid[pos] = null;
                grid[pos + Vec2I.Down] = data;
            }
            else if (pos.X > 0 && grid[pos.X - 1, pos.Y + 1] == null)
            {
                grid[pos] = null;
                grid[pos + Vec2I.DownLeft] = data;
            }
            else if (pos.X < grid.Width - 1 && grid[pos.X + 1, pos.Y + 1] == null)
            {
                grid[pos] = null;
                grid[pos + Vec2I.DownRight] = data;
            }
        }
    }
}