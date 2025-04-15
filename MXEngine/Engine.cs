using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using MeltySynth;
using MXEngine.Graphics.Core;
using MXEngine.Interfacing;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace MXEngine;

/// <summary>
/// The <b>core</b> part of MXEngine.
/// Requires an IWindow instance to create.
/// </summary>
public class Engine
{
    bool _running = false;
    Thread? _engineThread; 
    static readonly Logger Logger = new Logger("MXEngine");
    public static Engine? Instance;
    
    /// <summary>
    /// Set the engine's state to Starting and create all runtime threads.
    /// </summary>
    public void Start()
    {
        Instance = this;
        if (_running) return;
        _running = true;
        Logger.Info("Engine starting.");
        _engineThread = new Thread(EngineThread);
        _engineThread.Start();
        Logger.Info("Started engine thread.");
        Entity.InitializeRegistry();
        Logger.Info("Entity registry initialized.");
    }

    /// <summary>
    /// Set the engine's state to Stopping.
    /// </summary>
    public void Stop()
    {
        if (!_running) return;
        Logger.Info("Engine stopping.");
        Interfacing.Audio.Dispose();
        Logger.Info("Stopped engine.");
    }

    private double _time = 0;
    private int _ticks = 0;
    private List<double> deltas = new List<double>();

    private void EngineThread()
    {
        var stopwatch = Stopwatch.StartNew();
        var lastTime = (float)stopwatch.Elapsed.TotalMilliseconds;
        while (_running)
        {
            
        }
    }
}

public class EngineData
{
    public Key QuitKey = Key.Unknown;
}