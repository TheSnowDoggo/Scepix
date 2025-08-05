using Avalonia;
using System;
using System.Diagnostics;
using Scepix.Update;
using Scepix.Collections;

namespace Scepix;

sealed class Program
{
    private static readonly Updater _updater = new();
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        _updater.OnUpdate += Update;

        _updater.FrameCap = 60;
        
        _updater.Start();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void Update(double delta)
    {
        Debug.WriteLine($"Delta: {delta} FPS: {_updater.FPS}");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}