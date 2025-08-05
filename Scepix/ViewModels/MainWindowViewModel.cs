using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Scepix.Models;
using SkiaSharp;

namespace Scepix.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PixelViewModel PixelViewModel { get; } = new PixelViewModel(new PixelManager());
}