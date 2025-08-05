using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace Scepix.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        using (var canvas = new SKCanvas(_skBitmap))
        {
            var paint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = false,
            };
            canvas.DrawRect(0, 0, 1, 1, paint);
        };
    }

    private readonly SKBitmap _skBitmap = CreateBitmap(64, 64, SKColors.Aqua);
    
    public Bitmap Bitmap => ToBitmap(_skBitmap);

    private static SKBitmap CreateBitmap(int width, int height, SKColor? color = null)
    {
        var bitmap = new SKBitmap(width, height);
        
        if (color is { } c)
        {
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(c);
        }
        
        return bitmap;
    }

    private static Bitmap ToBitmap(SKBitmap skBitmap)
    {
        var data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = data.AsStream();
        return new Bitmap(stream);
    }
}