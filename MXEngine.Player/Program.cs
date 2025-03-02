using System.Numerics;
using MXEngine.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MXEngine.Player;

class Program
{
    private static IWindow? _window;
    private static Engine? _engine;
    static void Main(string[] args)
    {
        // uses OpenGL 4.3+ (most compatible)
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "MXEngine";
        _window = Window.Create(options);
        
        _window.Load += WindowOnLoad;
        _window.Update += WindowOnUpdate;
        _window.Render += WindowOnRender;
        _window.FramebufferResize += WindowOnFramebufferResize;
        
        _window.Run();
        
        _engine?.Stop();
        _window.Dispose();
    }

    private static void WindowOnFramebufferResize(Vector2D<int> obj)
    {
        _engine!.Size = obj;
    }

    private static void WindowOnRender(double deltaTime)
    {
        _engine?.Render(deltaTime);
    }

    private static void WindowOnUpdate(double deltaTime)
    {
        if (!_engine!.Tick(deltaTime))
        {
            _window!.Close();
        }
    }

    private static void WindowOnLoad()
    {
        _engine = new Engine(_window);
        _engine.Start();
    }
}