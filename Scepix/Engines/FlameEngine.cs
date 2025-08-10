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

            var fueled = false;
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
                    fueled = true;
                    continue;
                }

                space[next] = null;
                
                if (_rand.NextBool())
                {
                    space[next] = space.Make("co2");
                }
            }

            if (fueled)
            {
                continue;
            }
            
            if (!data.LocalTags.TryGetValue(DeathTimerTag, out float deathTimer))
            {
                deathTimer = data.Variant.DataTags.GetContentOrDefault(LifetimeTag, 3f);
                data.LocalTags[DeathTimerTag] = deathTimer;
            }

            if (deathTimer > 0)
            {
                data.LocalTags[DeathTimerTag] = deathTimer - (float)delta;
                continue;
            }

            space[pos] = null;
        }
    }
}