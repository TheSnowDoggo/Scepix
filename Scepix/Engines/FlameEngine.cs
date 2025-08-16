using System;
using System.Collections.Generic;
using Scepix.Pixel;
using Scepix.Types;

namespace Scepix.Engines;

public class FlameEngine() : TagEngine("flame")
{
    private readonly Random _rand = new();
    
    private const string FlammableTag = "flame.duration";
    
    private const string TimerTag = "flame.timer";

    private const string DeathTimerTag = "flame.deathtimer";
    
    private const string LifetimeTag = "flame.lifetime";
    
    public override void Update(double delta, List<Coord> positions, PixelSpace space)
    {
        foreach (Vec2I pos in positions)
        {
            if (space[pos] is not { } data)
            {
                continue;
            }

            foreach (var axis in Vec2I.AllAxis)
            {
                var next = pos + axis;

                if (!space.TryGet(next, out var p) || p == null || 
                    !p.Variant.DataTags.TryGetValue(FlammableTag, out float duration) || duration <= 0)
                {
                    continue;
                }

                if (!p.LocalTags.TryGetValue(TimerTag, out float timer))
                {
                    timer = duration;
                    p.LocalTags[TimerTag] = timer;
                }

                if (timer > 0)
                {
                    p.LocalTags[TimerTag] = timer - (float)delta;
                    continue;
                }

                space[next] = null;
            }

            foreach (var axis in Vec2I.AllAxis)
            {
                var next = pos + axis;

                if (!space.TryGet(next, out var p) || p != null)
                {
                    continue;
                }

                if (Math.Abs(axis.X) != 0)
                {
                    var next2 = next + new Vec2I(axis.X, 0);

                    if (space.TryGet(next2, out p) && p != null &&
                        p.Variant.DataTags.TryGetValue(FlammableTag, out float duration) && duration > 0)
                    {
                        space[next] = space.Make("fire");
                        continue;
                    }
                }
                if (Math.Abs(axis.Y) != 0)
                {
                    var next2 = next + new Vec2I(0, axis.Y);
                        
                    if (space.TryGet(next2, out p) && p != null &&
                        p.Variant.DataTags.TryGetValue(FlammableTag, out float duration) && duration > 0)
                    {
                        space[next] = space.Make("fire");
                    }
                }
            }
            
            if (!data.LocalTags.TryGetValue(DeathTimerTag, out float deathTimer))
            {
                deathTimer = MUtils.Lerp((float)_rand.NextDouble(), 0.5f, 1.0f);
                data.LocalTags[DeathTimerTag] = deathTimer;
            }

            if (deathTimer > 0)
            {
                data.LocalTags[DeathTimerTag] = deathTimer - (float)delta;
                continue;
            }

            space[pos] = null;
            
            if (_rand.Next(50) == 0)
            {
                space[pos] = space.Make("co2");
            }
        }
    }
}