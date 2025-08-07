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

    private readonly Grid2D<PixelData?> _grid = new(64, 64);

    private readonly Dictionary<string, PixelVariant> _variants = new()
    {
        { "sand", new PixelVariant() { Color = SKColors.Yellow, Tags = new TagSet() { "powder" } } },
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
    
    public Grid2DView<PixelData?> Grid => _grid;

    public event EventHandler Render;

    public void Start()
    {
        _grid.Fill(p => new PixelData(_variants["sand"]), 0, 0, 13, 10);
        
        _grid.Fill(p => new PixelData(_variants["sand"]), 50, 40, 10, 20);
        
        _grid.Fill(p => new PixelData(_variants["gravel"]), 8, 17, 10, 10);

        _grid.Fill(p => new PixelData(_variants["water"]), 20, 5, 40, 20);
        _updater.OnUpdate += Update;

        _updater.FrameCap = 20;
       
        _updater.Start();
    }

    private void Update(double delta)
    {
        Debug.WriteLine($"delta: {delta} FPS: {_updater.FPS}");

        _tagEngineManager.Update(delta, _grid);
       
        Render?.Invoke(this, EventArgs.Empty);
    }
}