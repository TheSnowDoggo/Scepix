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
        { "sand", new PixelVariant() { Color = SKColors.Yellow, Tags = new TagSet() { "powder", "rust" } } },
        { "gravel", new PixelVariant() { Color = SKColors.Gray, Tags = new TagSet() { "powder" } } },
        { "steel", new PixelVariant() { Color = SKColors.DarkGray, Tags = new TagSet() { "rust" } } },
    };

    private readonly TagEngineManager _tagEngineManager = new()
    {
        new PowderEngine()
    };

    public PixelManager()
    {
        Start();
    }
    
    public Grid2DView<PixelData?> Grid => _grid;

    public event EventHandler Render;

    public void Start()
    {
        _grid.Fill(p => new PixelData()
        {
            Variant = _variants["sand"],
        }, 0, 0, 10, 10);
        
        _grid.Fill(p => new PixelData()
        {
            Variant = _variants["gravel"],
        }, 8, 17, 10, 10);

        _updater.OnUpdate += Update;

        _updater.FrameCap = 20;
        
        _updater.Start();
    }

    private void Update(double delta)
    {
        try
        {
            Debug.WriteLine($"delta: {delta} FPS: {_updater.FPS}");

            _tagEngineManager.Update(delta, _grid);
       
            Render?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }
    }
}