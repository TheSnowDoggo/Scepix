using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Scepix.Collections;
using Scepix.Update;
using Scepix.Pixel;
using Scepix.Types;
using SkiaSharp;
using Scepix.Engines;
using Scepix.Views;

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

    private bool _filling = false;

    private string _fill = "sand";

    private Vec2I _mousePos = Vec2I.Zero;

    private double _statsTimer;

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
        _space.Fill(p => _space.Make("sand"), 0, 60, 100, 30);
        
        _space.Fill(p => _space.Make("water"), 30, 0, 80, 40);
        
        _updater.OnUpdate += Update;

        _updater.FrameCap = 60;
       
        _updater.Start();
    }

    private readonly Stopwatch sw = new();

    private void Update(double delta)
    {
        if (_statsTimer > 0)
        {
            _statsTimer -= delta;
        }
        else
        {
            Console.WriteLine($"delta: {delta} real: {1.0 / _updater.UpdateTime:0.00} FPS: {_updater.FPS}");
            _statsTimer = 0.5;
        }

        if (_filling)
        {
            foreach (var off in EnumerateCircle(5))
            {
                var pos = _mousePos + off;
                
                if (_space.InRange(pos))
                {
                    _space[pos] = _fill == string.Empty ? null : _space.Make(_fill);
                }
            }
        }

        _tagEngineManager.Update(delta, _space);

        Render?.Invoke(this, new RenderEventArgs(_space, _space.Changes));

        _space.ClearChanges();
    }
    
    public void Space_PointerModify(MainWindow.PointerModify modify, Control sender, PointerEventArgs e)
    {
        switch (modify)
        {
            case MainWindow.PointerModify.Release:
                _filling = false;
                return;
            case MainWindow.PointerModify.Place:
                _filling = true;
                _fill = "sand";
                break;
            case MainWindow.PointerModify.Remove:
                _filling = true;
                _fill = string.Empty;
                break;
            case MainWindow.PointerModify.Move:
                if (!_filling)
                {
                    return;
                }
                break;
            default:
                throw new ArgumentException("Unknown modify type");
        }

        var point = e.GetCurrentPoint(sender);
        
        var rawPos = point.Position;
        
        var x = (int)Math.Round(rawPos.X / sender.Bounds.Size.Width * _space.Width);
        var y = (int)Math.Round(rawPos.Y / sender.Bounds.Size.Height * _space.Height);
        
        _mousePos = new Vec2I(x, y);
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