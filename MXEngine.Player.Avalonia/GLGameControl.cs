using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;

namespace MXEngine.Player.Avalonia;

public class GlGameControl : OpenGlControlBase 
{
    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
    }
       

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
    }

    protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
    {
        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
    }
}