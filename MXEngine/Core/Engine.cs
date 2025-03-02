using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using MeltySynth;
using MXEngine.Graphics.AppSupport;
using MXEngine.Graphics.Core;
using MXEngine.Interfacing;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace MXEngine.Core;

/// <summary>
/// The <b>core</b> part of MXEngine.
/// Requires an IWindow instance to create.
/// </summary>
public unsafe class Engine(IWindow Window)
{
    public EngineState State = EngineState.None;
    private Thread engineThread;
    internal static Logger _logger = new Logger("MXEngine");
    public readonly GL Gl = GL.GetApi(Window);
    public static Engine Instance;
    public Camera GameCamera;
    public Vector2D<int> Size;
    internal IWindow window => Window;
    
    /// <summary>
    /// Set the engine's state to Starting and create all runtime threads.
    /// </summary>
    public void Start()
    {
        Instance = this;
        if (State != EngineState.None) return;
        _logger.Info("Engine starting.");
        State = EngineState.Starting;
        new Input(window);
        GameCamera = new Camera();
        GameCamera.CameraMode = CameraMode.Freecam;
        GameCamera.Initialize();
        engineThread = new Thread(EngineThread);
        engineThread.Start();
        _logger.Info("Started engine thread.");
        ObjectInstance.Initialize();
        _logger.Info("ObjectInstance registry initialized.");
        Input.KeyUp += key =>
        {
            if (key == Key.Escape)
            {
                Stop();
            }
        };
        Interfacing.Audio.Initialize();
        State = EngineState.Running;
        Interfacing.Audio.CreateMidiPlayer(Interfacing.Audio.RetroSoundfont)
            .Play(new MidiFile(Resources.GetStream("MXEngine.Audio.DefaultResources.M_E1M1.mid")!, MidiFileLoopType.None), true);
    }

    /// <summary>
    /// Set the engine's state to Stopping.
    /// </summary>
    public void Stop()
    {
        if (State != EngineState.Running) return;
        _logger.Info("Engine stopping.");
        State = EngineState.Stopping;
        Interfacing.Audio.Dispose();
        _logger.Info("Running garbage collection.");
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        engineThread.Join();
        _logger.Info("Stopped engine.");
    }

    public void Render(double deltaTime)
    {
        Gl.Viewport(Size);
        GameCamera.Render(deltaTime);
    }

    private double _time = 0;
    private int _ticks = 0;
    private List<double> deltas = new List<double>();
    public bool Tick(double deltaTime)
    {
        if (State != EngineState.Running && State != EngineState.Starting)
        {
            return false;
        }
        _time += deltaTime;
        deltas.Add(deltaTime);
        double tic = 0;
        foreach (double tick in deltas)
        {
            tic += tick;
        }
        //_logger.Info($"Engine ticked. Time since last tick: {_time}. Ticks per second: {1/(tic/deltas.Count)}. Current Time: {_time}.");
        deltas.Clear();
        return true;
    }

    private void EngineThread()
    {
        var stopwatch = Stopwatch.StartNew();
        var lastTime = stopwatch.Elapsed.TotalMilliseconds;
        var deltaTime = stopwatch.Elapsed.TotalMilliseconds - lastTime;
        GameCamera.Update(deltaTime);
        while (State == EngineState.Running || State == EngineState.Starting)
        {
            if (State == EngineState.Starting)
            {
                Thread.Sleep(100);
            }
            else
            {
                // Calculate lastTime and deltaTime
                deltaTime = stopwatch.Elapsed.TotalMilliseconds/1000d-lastTime;
                lastTime = stopwatch.Elapsed.TotalMilliseconds/1000d;
                // Update the camera
                GameCamera.Update(deltaTime);
            }
        }
    }
}

public class EngineData
{
    public Key QuitKey = Key.Unknown;
}

/// <summary>
/// An enum for all core engine states.
/// </summary>
public enum EngineState
{
    None,
    Starting,
    Running,
    Stopping,
    Stopped
}