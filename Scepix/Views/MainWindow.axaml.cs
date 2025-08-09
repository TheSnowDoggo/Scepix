using System;
using Avalonia.Controls;
using Avalonia.Input;
using Scepix.ViewModels;

namespace Scepix.Views;

public partial class MainWindow : Window
{
    public enum PointerModify
    {
        Place,
        Remove,
        Move,
        Release,
    }
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private MainWindowViewModel MainWindowViewModel => DataContext as MainWindowViewModel 
        ?? throw new NullReferenceException("DataContext is null");

    private void Space_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }
        
        if (e.Properties.IsLeftButtonPressed)
        {
            MainWindowViewModel.Space_PointerModify(PointerModify.Place, control, e);
        }
        else if (e.Properties.IsRightButtonPressed)
        {
            MainWindowViewModel.Space_PointerModify(PointerModify.Remove, control, e);
        }
    }
    
    private void Space_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }
        
        MainWindowViewModel.Space_PointerModify(PointerModify.Move, control, e);
    }

    private void Space_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }
        
        MainWindowViewModel.Space_PointerModify(PointerModify.Release, control, e);
    }
}