using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scepix.Collections;
using Scepix.Update;
using Scepix.Pixel;
using Scepix.Types;
using SkiaSharp;

namespace Scepix.Models;

public class PixelManager
{
    private readonly Updater _updater = new();

    private readonly VirtualGrid2D<PixelData?> _grid = new(100, 100);

    private readonly Dictionary<string, PixelVariant> _variants = new()
    {
        { "sand", new PixelVariant() { Color = SKColors.Yellow, Tags = new TagSet() { "powder" }, } },
        { "gravel", new PixelVariant() { Color = SKColors.Gray, Tags = new TagSet() { "powder" } } },
        { "water", new PixelVariant() { Color = SKColors.RoyalBlue, Tags = new TagSet() { "liquid" } }},
        { "oil", new PixelVariant() { Color = SKColors.DarkSlateGray, Tags = new TagSet() { "liquid" } }}
    };

    private readonly TagEngineManager _tagEngineManager =
    [
        new PowderEngine(),
        new LiquidEngine(),
    ];

    public PixelManager()
    {
        Start();
    }
    
    public GridView<PixelData?> Grid => _grid;

    public event EventHandler? Render;

    public void Start()
    {
        _grid.Fill(p => new PixelData(_variants["sand"]), 30, 50, 10, 20);
        
        _grid.Fill(p => new PixelData(_variants["water"]), 0, 0, 100, 30);
        
        //_grid.Fill(p => new PixelData(_variants["water"]), 0, 70, 50, 50);
        _updater.OnUpdate += Update;

        _updater.FrameCap = 30;
       
        _updater.Start();
    }

    private void Update(double delta)
    {
        Console.WriteLine($"delta: {delta} real: {1.0 / _updater.UpdateTime:0.00} FPS: {_updater.FPS}");

        _tagEngineManager.Update(delta, _grid);
       
        Render?.Invoke(this, EventArgs.Empty);
    }
}