using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using MXEngine.Core;
using MXEngine.Graphics.AppSupport;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace MXEngine.Player.Avalonia;

public class GlGameControl : OpenGlControlBase
{
    private Engine? _engine;
    
    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
        
        // Get the real OpenGL interface.
        GL gL = GL.GetApi(gl.GetProcAddress);
        // Create the AvaloniaGl class (derivative of IGraphics).
        AvaloniaGl interfaceGl = new AvaloniaGl(gL);
        // Create an engine instance and start it.
        _engine = new Engine(interfaceGl);
        _engine.Start();
    }
       

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
        _engine?.Stop();
    }

    protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
    {
        _engine?.Render();
        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
    }
}

public class AvaloniaGl(GL baseGl) : IGraphics
{
    private GL Gl => baseGl;
    
    public GL GetGlInterface()
    {
        return Gl;
    }
}