using System;
using Avalonia.Controls;
using Avalonia.Input;
using Scepix.ViewModels;

namespace Scepix.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Space_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var mw = DataContext as MainWindowViewModel ?? throw new NullReferenceException("DataContext is null");
        
        mw.Space_OnPointerPressed(sender, e);
    }
}