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
        
        _window.Run();
        
        _engine?.Stop();
        _window.Dispose();
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
        GL gl = GL.GetApi(_window);
        IInputContext inputContext = _window!.CreateInput();
        _engine = new Engine(gl, inputContext);
        _engine.Start();
    }
}