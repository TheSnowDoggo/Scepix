using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Scepix.Collections;
using Scepix.Update;
using Scepix.Pixel;
using Scepix.Types;
using SkiaSharp;
using Scepix.Engines;

namespace Scepix.Models;

public class PixelManager
{
    private readonly Updater _updater = new();

    private readonly PixelSpace _space = new(220, 100)
    {
        Variants = 
        [
            new PixelVariant("sand")
            {
                Color = SKColors.Yellow, 
                EngineTags = ["powder", "wet"],
                DataTags = new TagMap()
                {
                    { "wet.recipes", new Dictionary<string, WetEngine.Recipe>
                        {
                            { "water", new WetEngine.Recipe("wetsand") { MinTime = 5, MaxTime = 20 } },
                        }
                    },
                    { "wet.axis", WetEngine.AxisType.AllAxis },
                    { "density", 10 },
                }
            },
            new PixelVariant("wetsand")
            {
                Color = SKColors.BurlyWood,
                EngineTags = ["powder"],
                DataTags = new TagMap()
                {
                    { "density", 10 }
                }
            },
            new PixelVariant("gravel")
            {
                Color = SKColors.Gray, 
                EngineTags = ["powder"],
            },
            new PixelVariant("water")
            {
                Color = SKColors.RoyalBlue, 
                EngineTags = ["liquid"],
                DataTags = new TagMap()
                {
                    { "density", 0 },
                }
            },
            new PixelVariant("oil")
            {
                Color = SKColors.DarkSlateGray, 
                EngineTags = ["liquid"],
                DataTags = new TagMap()
                {
                    { "density", 3 },
                } 
            },
            new PixelVariant("co2")
            {
                Color = SKColors.LightGray,
                EngineTags = ["liquid", "gas"],
                DataTags = new TagMap()
                {
                    "liquid.anti",
                    { "density", -3 }
                }
            }
        ]
    };
    
    private readonly TagEngineManager _tagEngineManager =
    [
        new PowderEngine(),
        new LiquidEngine(),
        new GasEngine(),
        new WetEngine(),
    ];
    
    private readonly Queue<Vec2I> _fillQueue = new();

    public PixelManager()
    {
        Start();
    }
    
    public class RenderEventArgs(GridView<PixelData?> grid, IEnumerable<Vec2I> changes) : EventArgs
    {
        public GridView<PixelData?> Grid { get; } = grid;

        public IEnumerable<Vec2I> Changes { get; } = changes;
    }
    
    public event EventHandler<RenderEventArgs>? Render;

    private void Start()
    {
        _space.Fill(p => new PixelData(_space.Variants["sand"]), 0, 0, 100, 30);
        
        _space.Fill(p => new PixelData(_space.Variants["water"]), 30, 0, 80, 40);
        
        //_space.Fill(p => new PixelData(_space.Variants["co2"]), 80, 40, 60, 30);
        
        //_space.Fill(p => new PixelData(_space.Variants["oil"]), 160, 50, 50, 20);
        
        _updater.OnUpdate += Update;

        _updater.FrameCap = 60;
       
        _updater.Start();
    }

    private void Update(double delta)
    {
        Console.WriteLine($"delta: {delta} real: {1.0 / _updater.UpdateTime:0.00} FPS: {_updater.FPS}");

        while (_fillQueue.Count > 0)
        {
            var pixelPos = _fillQueue.Dequeue();
            
            foreach (var off in EnumerateCircle(5))
            {
                var pos = pixelPos + off;
                if (_space.InRange(pos))
                {
                    _space[pos] = new PixelData(_space.Variants["sand"]);
                }
            }
        }

        _tagEngineManager.Update(delta, _space);
       
        Render?.Invoke(this, new RenderEventArgs(_space, _space.Changes));
        
        _space.ClearChanges();
    }

    public void Space_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }
        
        var point = e.GetCurrentPoint(control);
        
        var rawPos = point.Position;
        
        var x = (int)Math.Round(rawPos.X / control.Bounds.Size.Width * _space.Width);
        var y = (int)Math.Round(rawPos.Y / control.Bounds.Size.Height * _space.Height);
        
        var pixelPos = new Vec2I(x, y);
        
        _fillQueue.Enqueue(pixelPos);
    }
    
    private static HashSet<Vec2I> EnumerateCircle(double radius)
    {
        var set = new HashSet<Vec2I>();
        
        var end = (int)Math.Round(radius);
        for (var y = -end; y <= end; ++y)
        {
            var s = (int)Math.Round(MUtils.CircleSolveX(radius, y));

            if (s == 0)
            {
                set.Add(new Vec2I(s, y));
                continue;
            }

            for (var x = -s; x <= s; ++x)
            {
                set.Add(new Vec2I(x, y));
            }
        }

        for (var x = -end; x <= end; ++x)
        {
            var s = (int)Math.Round(MUtils.CircleSolveY(radius, x));

            set.Add(new Vec2I(x, s));
            
            if (s != 0)
            {
                set.Add(new Vec2I(x, -s));
            }
        }
        
        return set;
    }
}