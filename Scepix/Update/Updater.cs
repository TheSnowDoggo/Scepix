using System;
using System.Diagnostics;
using System.Threading;

namespace Scepix.Update;

/// <summary>
/// A class for calling updates.
/// </summary>
public class Updater
{
    private readonly Thread _thread;

    public Updater()
    {
        _thread = new Thread(Update);
    }

    /// <summary>
    /// Gets or sets the active state of the update thread.
    /// </summary>
    public bool Active { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the pause state of the update thread.
    /// </summary>
    public bool Paused { get; set; } = false;
    
    /// <summary>
    /// Gets the current framerate updated according to the set FPSUpdateRate.
    /// </summary>
    public int FPS { get; private set; }

    /// <summary>
    /// Gets or sets the FPS update rate in seconds.
    /// </summary>
    public double FPSUpdateRate { get; set; } = 1.0;

    public Action<double>? OnUpdate;

    /// <summary>
    /// Gets or sets the maximum number of updates per second.
    /// </summary>
    /// <remarks>
    /// By default -1 representing uncapped.
    /// </remarks>
    public int FrameCap { get; set; } = -1;

    private double DeltaCap => 1.0 / FrameCap;

    /// <summary>
    /// Starts the update thread.
    /// </summary>
    public void Start()
    {
        _thread.Start();
    }

    /// <summary>
    /// Stops the update thread.
    /// </summary>
    public void Stop()
    {
        Active = false;
    }

    /// <summary>
    /// Pauses the update thread if active.
    /// </summary>
    public void Pause()
    {
        Paused = true;
    }

    /// <summary>
    /// Resumes the update thread if active.
    /// </summary>
    public void Resume()
    {
        Paused = false;
    }
    
    private void Update()
    {
        var deltaTimer = Stopwatch.StartNew();
        var delta = 0.0;
        
        var frameTimer = Stopwatch.StartNew();
        var frameCount = 0;
        
        Active = true;
        while (Active)
        {
            if (Paused)
            {
                continue;
            }
            
            OnUpdate?.Invoke(delta);

            if (FrameCap > 0)
            {
                while (deltaTimer.Elapsed.TotalSeconds < DeltaCap)
                {}
            }
            
            ++frameCount;

            if (frameTimer.Elapsed.TotalSeconds >= FPSUpdateRate)
            {
                FPS = frameCount;
                frameCount = 0;
                
                frameTimer.Restart();
            }
            
            delta = deltaTimer.Elapsed.TotalSeconds;
            deltaTimer.Restart();
        }
    }
}