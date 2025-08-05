using System;
using System.Diagnostics;
using Scepix.Models;
using SkiaSharp;
using Avalonia.Media.Imaging;

namespace Scepix.ViewModels;

public class PixelViewModel : ViewModelBase
{
    private readonly PixelManager _manager;

    private readonly SKBitmap _skBitmap = CreateBitmap(64, 64, SKColors.Aqua);

    public PixelViewModel(PixelManager manager)
    {
        _manager = manager;
        _manager.Render += Manager_OnRender;
    }

    public Bitmap Bitmap => ToBitmap(_skBitmap);

    private void Manager_OnRender(object? sender, EventArgs e)
    {
        var paint = new SKPaint()
        {
            IsAntialias = false,
        };

        using var canvas = new SKCanvas(_skBitmap);
        foreach (var pos in _manager.Grid.Enumerate())
        {
            var pixelData = _manager.Grid[pos];
            
            paint.Color = pixelData.Variant.Color;
            
            canvas.DrawPoint(pos.X, pos.Y, paint);
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
}