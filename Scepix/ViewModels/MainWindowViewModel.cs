using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Scepix.Models;
using Scepix.Views;
using SkiaSharp;

namespace Scepix.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PixelViewModel PixelViewModel { get; } = new PixelViewModel(new PixelManager());
    
    public void Space_PointerModify(MainWindow.PointerModify modify, Control sender,  PointerEventArgs e)
    {
        PixelViewModel.Space_PointerModify(modify, sender, e);
    }
    
    public void Space_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        PixelViewModel.Space_OnPointerWheelChanged(sender, e);
    }
}