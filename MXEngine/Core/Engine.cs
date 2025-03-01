using System.Drawing;
using System.Numerics;
using MXEngine.Graphics.AppSupport;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace MXEngine.Core;

/// <summary>
/// The <b>core</b> part of MXEngine.
/// Requires a GL and IInputContext instance to create.
/// </summary>
public class Engine(GL gl, IInputContext inputContext)
{
    public EngineState State = EngineState.None;
    private Thread engineThread;
    private Logger _logger = new Logger("MXEngine");
    private IKeyboard? _keyboard;
    
    /// <summary>
    /// Set the engine's state to Starting and create all runtime threads.
    /// </summary>
    public void Start()
    {
        if (State != EngineState.None) return;
        _logger.Info("Engine starting.");
        State = EngineState.Starting;
        engineThread = new Thread(EngineThread);
        engineThread.Start();
        _logger.Info("Started engine thread.");
        ObjectInstance.Initialize();
        _logger.Info("ObjectInstance registry initialized.");
        _keyboard = inputContext.Keyboards.FirstOrDefault();
    }

    /// <summary>
    /// Set the engine's state to Stopping.
    /// </summary>
    public void Stop()
    {
        State = EngineState.Stopping;
        _logger.Info("Engine stopping.");
        engineThread.Join();
    }

    public void Render(double deltaTime)
    {
        gl.Disable(EnableCap.DepthTest);

        gl.ClearColor(Color.MediumPurple);
        gl.Clear(ClearBufferMask.ColorBufferBit);
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

        if (_keyboard != null && _keyboard.IsKeyPressed(Key.Escape))
        {
            _logger.Info("Escape key is down, stopping engine.");
            return false;
        }
        _time += deltaTime;
        deltas.Add(deltaTime);
        double tic = 0;
        foreach (double tick in deltas)
        {
            tic += tick;
        }
        _logger.Info($"Engine ticked. Time since last tick: {_time}. Ticks per second: {1/(tic/deltas.Count)}. Current Time: {_time}.");
        deltas.Clear();
        return true;
    }

    private void EngineThread()
    {
        while (State == EngineState.Running || State == EngineState.Starting)
        {
            if (State == EngineState.Starting)
            {
                Thread.Sleep(100);
            }
            else
            {
                
            }
        }
        _logger.Info("Engine stopped.");
    }
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