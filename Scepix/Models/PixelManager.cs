using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Scepix.Collections;
using Scepix.Update;
using Scepix.Pixel;
using Scepix.Types;
using SkiaSharp;
using Tmds.DBus.Protocol;

namespace Scepix.Models;

public class PixelManager
{
    private readonly Updater _updater = new();

    private readonly Grid2D<PixelData> _grid = new(64, 64);

    private readonly Dictionary<string, PixelVariant> _variants = new()
    {
        { "sky", new PixelVariant() { Color = SKColors.Cyan } },
        { "rock", new PixelVariant() { Color = SKColors.Gray } },
    };

    private double moveTimer = 0.0;

    private Vec2I pos = Vec2I.Zero;
    
    public PixelManager()
    {
        Start();
    }
    
    public Grid2DView<PixelData> Grid => _grid;

    public event EventHandler Render;

    public void Start()
    {
        _grid.Fill(new PixelData() { Variant = _variants["sky"] });
        
        _updater.OnUpdate += Update;

        _updater.FrameCap = 60;
        
        _updater.Start();
    }

    private void Update(double delta)
    {
        Debug.WriteLine($"delta: {delta} FPS: {_updater.FPS}");
        
        if (moveTimer >= 0)
        {
            moveTimer -= delta;
        }
        else
        {
            _grid[pos] = new PixelData() { Variant = _variants["sky"] };
            
            pos += Vec2I.Right;
            
            _grid[pos] = new PixelData() { Variant = _variants["rock"] };
            
            moveTimer = 1;
        }
        
        Render?.Invoke(this, EventArgs.Empty);
    }
}