using System;
using System.Diagnostics;
using Avalonia.Input;
using Scepix.Models;
using SkiaSharp;
using Avalonia.Media.Imaging;

namespace Scepix.ViewModels;

public class PixelViewModel : ViewModelBase
{
    private readonly PixelManager _manager;

    private readonly SKPaint _paint = new SKPaint();

    private SKBitmap _skBitmap =  CreateBitmap(64, 64, SKColors.Black);

    public PixelViewModel(PixelManager manager)
    {
        _manager = manager;
        _manager.Render += Manager_OnRender;
    }

    public Bitmap Bitmap => ToBitmap(_skBitmap);

    private void Manager_OnRender(object? sender, PixelManager.RenderEventArgs e)
    {
        var resized = false;
        
        if (_skBitmap.Width != e.Grid.Width || 
            _skBitmap.Height != e.Grid.Height)
        {
            _skBitmap = CreateBitmap(e.Grid.Width, e.Grid.Height, SKColors.Black);
            resized = true;
        }
        
        using var canvas = new SKCanvas(_skBitmap);
        foreach (var pos in resized ? e.Grid.Enumerate() : e.Changes)
        { 
            var pixelData = e.Grid[pos];

            _paint.Color = pixelData == null ? SKColors.Black : pixelData.Variant.Color;
            
            canvas.DrawPoint(pos.X, pos.Y, _paint);
        }

        OnPropertyChanged(nameof(Bitmap));
    }

    private static SKBitmap CreateBitmap(int width, int height, SKColor? color = null)
    {
        var bitmap = new SKBitmap(width, height);

        if (color is not { } c)
        {
            return bitmap;
        }
        
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(c);

        return bitmap;
    }

    private static Bitmap ToBitmap(SKBitmap skBitmap)
    {
        var data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = data.AsStream();
        return new Bitmap(stream);
    }
    
    public void Space_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _manager.Space_OnPointerPressed(sender, e);
    }
}