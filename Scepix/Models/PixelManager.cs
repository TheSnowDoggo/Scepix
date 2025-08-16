using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    private readonly PixelSpace _space = new(300, 150)
    {
        Variants = 
        [
            new PixelVariant("iron")
            {
              Color = SKColors.LightSlateGray, 
              EngineTags = ["recipe"],
              DataTags = new TagMap()
              {
                  { "recipe.recipes", new Dictionary<string, RecipeEngine.Recipe>
                      {
                          { "water", new RecipeEngine.Recipe("rust") { MinTime = 30, MaxTime = 180, Remove = false } },
                          { "saltwater", new RecipeEngine.Recipe("rust") { MinTime = 15, MaxTime = 130, Remove = false }}
                      }
                  },
              },
            },
            new PixelVariant("wood")
            {
                Color = SKColors.Brown,
                DataTags = new TagMap()
                {
                    { "flame.duration", 8.0f }
                } 
            },
            new PixelVariant("rust")
            {
                Color = SKColors.SaddleBrown,
                EngineTags = ["powder"],
                DataTags = new TagMap()
                {
                    { "density", 10 },
                }
            },
            new PixelVariant("sand")
            {
                Color = SKColors.Yellow, 
                EngineTags = ["powder", "recipe"],
                DataTags = new TagMap()
                {
                    { "recipe.recipes", new Dictionary<string, RecipeEngine.Recipe>
                        {
                            { "water", new RecipeEngine.Recipe("wetsand") { MinTime = 5, MaxTime = 20, Remove = false } },
                            { "saltwater", new RecipeEngine.Recipe("silt") { MinTime = 5, MaxTime = 20 } },
                        }
                    },
                    { "recipe.axis", RecipeEngine.AxisType.AllAxis },
                    { "density", 10 },
                }
            },
            new PixelVariant("wetsand")
            {
                Color = SKColors.SandyBrown,
                EngineTags = ["powder"],
                DataTags = new TagMap()
                {
                    { "density", 10 }
                }
            },
            new PixelVariant("silt")
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
                DataTags = new TagMap()
                {
                    { "density", 10 }
                }
            },
            new PixelVariant("salt")
            {
                Color = SKColors.GhostWhite,
                EngineTags = ["recipe", "powder"],
                DataTags = new TagMap()
                {
                    { "recipe.recipes", new Dictionary<string, RecipeEngine.Recipe>
                        {
                            { "water", new RecipeEngine.Recipe("saltwater") { MinTime = 0, MaxTime = 1 } }
                        }
                    },
                    { "density", 10 },
                },
            },
            new PixelVariant("water")
            {
                Color = SKColors.DodgerBlue, 
                EngineTags = ["liquid"],
                DataTags = new TagMap()
                {
                    { "density", 0 },
                }
            },
            new PixelVariant("saltwater")
            {
                Color = SKColors.RoyalBlue,
                EngineTags = ["liquid"],
                DataTags = new TagMap()
                {
                    { "density", 1 },
                }
            },
            new PixelVariant("oil")
            {
                Color = SKColors.DarkSlateGray, 
                EngineTags = ["liquid"],
                DataTags = new TagMap()
                {
                    { "density", -2 },
                    { "flame.duration", 5.0f }
                } 
            },
            new PixelVariant("co2")
            {
                Color = SKColors.LightGray,
                EngineTags = ["gas"],
                DataTags = new TagMap()
                {
                    "liquid.anti",
                    { "density", -4 }
                }
            },
            new PixelVariant("fire")
            {
                Color = SKColors.Orange,
                EngineTags = ["flame", "gas"],
                DataTags = new TagMap()
                {
                    { "density", -5 }
                }
            },
            new PixelVariant("extinguisher")
            {
                Color = SKColors.Aqua,
                EngineTags = ["powder"],
                DataTags = new TagMap()
                {
                    { "density", -3 }
                }
            },
        ]
    };
    
    private readonly TagEngineManager _tagEngineManager =
    [
        new RecipeEngine(),
        new FlameEngine(),
        new PowderEngine(),
        new LiquidEngine(),
        new GasEngine(),
    ];

    private bool _filling = false;

    private readonly Queue<Vec2I> _fillQueue = new();

    private string? _fill = null;

    private double _statsTimer;

    private float _brushSize = 5.0f;

    public PixelManager()
    {
        Start();
    }

    public IEnumerable<string> Variants => _space.Variants.Names;

    public string? SelectedVariant { get; set; } = null;
    
    public class RenderEventArgs(GridView<PixelData?> grid, IEnumerable<Vec2I> changes) : EventArgs
    {
        public GridView<PixelData?> Grid { get; } = grid;

        public IEnumerable<Vec2I> Changes { get; } = changes;
    }
    
    public event EventHandler<RenderEventArgs>? Render;

    private void Start()
    {
        //_space.Fill(p => _space.Make("sand"), 0, 60, 100, 30);
        
        //_space.Fill(p => _space.Make("water"), 30, 0, 80, 40);

        _tagEngineManager.LazyFrameUpdateRate = 5;
        
        _updater.OnUpdate += Update;

        _updater.FrameCap = 60;
       
        _updater.Start();
    }

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

        if (_filling && _fill != null)
        {
            var circle = EnumerateCircle((int)MathF.Round(_brushSize));

            while (_fillQueue.Count > 0)
            {
                var fillPos = _fillQueue.Count > 1 ? _fillQueue.Dequeue() :
                    _fillQueue.Peek();
                
                foreach (var pos in circle.Select(off => fillPos + off)
                    .Where(pos => _space.InRange(pos)))
                {
                    if (_fill == string.Empty)
                    {
                        _space[pos] = null;
                    }
                    else if (_space[pos] == null)
                    {
                        _space[pos] = _space.Make(_fill);
                    }
                }

                if (_fillQueue.Count == 1)
                {
                    break;
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
                _fillQueue.Clear();
                return;
            case MainWindow.PointerModify.Place:
                _filling = true;
                _fillQueue.Enqueue(TranslatePosition(sender, e));
                _fill = SelectedVariant;
                break;
            case MainWindow.PointerModify.Remove:
                _filling = true;
                _fillQueue.Enqueue(TranslatePosition(sender, e));
                _fill = string.Empty;
                break;
            case MainWindow.PointerModify.Move:
                if (_filling)
                {
                    _fillQueue.Enqueue(TranslatePosition(sender, e));
                }
                break;
            default:
                throw new ArgumentException("Unknown modify type");
        }

        
    }

    private Vec2I TranslatePosition(Control sender, PointerEventArgs e)
    {
        var rawPos = e.GetCurrentPoint(sender).Position;
        
        var x = (int)Math.Round(rawPos.X / sender.Bounds.Size.Width * _space.Width);
        var y = (int)Math.Round(rawPos.Y / sender.Bounds.Size.Height * _space.Height);
        
        return new Vec2I(x, y);
    }
    
    public void Space_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _brushSize = Math.Clamp(_brushSize + (float)e.Delta.Y, 1.0f, 50.0f);

        Console.WriteLine($"Brush changed to {_brushSize}");
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