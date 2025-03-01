using MXEngine.UWP.Common;
using MXEngine.UWP.OpenGL;
using Silk.NET.OpenGL;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace MXEngine.Player.UWP;

/// <summary>
/// Represent a non-XAML UWP app, i.e. an app without XAML content that is hosted in a <see cref="CoreWindow"/> instance.
/// </summary>
public sealed partial class App : IFrameworkViewSource, IFrameworkView
{
    /// <summary>
    /// The <see cref="CoreApplicationView"/> for the current app instance.
    /// </summary>
    private CoreApplicationView? _applicationView;

    /// <summary>
    /// The <see cref="CoreWindow"/> used to display the app content.
    /// </summary>
    private CoreWindow? _window;

    /// <summary>
    /// The entry point for the application.
    /// </summary>
    public static void Main()
    {
        CoreApplication.Run(new App());
    }

    /// <inheritdoc/>
    public IFrameworkView CreateView()
    {
        return this;
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(_applicationView))]
    public void Initialize(CoreApplicationView applicationView)
    {
        this._applicationView = applicationView;
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(_window))]
    public void SetWindow(CoreWindow window)
    {
        this._window = window;
    }

    /// <inheritdoc/>
    public void Load(string entryPoint)
    {
    }

    RenderContext ctx;
    MXEngine.UWP.OpenGL.Framebuffer buffer;

    /// <inheritdoc/>
    public unsafe void Run()
    {
        // Activate the current window
        _window!.Activate();

        Stopwatch _stopwatch = Stopwatch.StartNew();
        uint vao;
        uint shaderProgram;

        ctx = new RenderContext(new Settings
        {
            MajorVersion = 4,
            MinorVersion = 1
        });
        buffer = new MXEngine.UWP.OpenGL.Framebuffer(ctx, (int)_window.Bounds.Width, (int)_window.Bounds.Height);

        GL gl = RenderContext.GL;
        gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        float[] vertices = {
                -0.5f, -0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f
            };

        gl.GenBuffers(1, out uint vbo);
        gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
        gl.BufferData<float>(GLEnum.ArrayBuffer, (nuint)vertices.Length * sizeof(float), vertices, GLEnum.StaticDraw);

        gl.GenVertexArrays(1, out vao);
        gl.BindVertexArray(vao);
        gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), null);
        gl.EnableVertexAttribArray(0);

        string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }
            ";

        string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
                }
            "
        ;

        uint vertexShader = gl.CreateShader(GLEnum.VertexShader);
        gl.ShaderSource(vertexShader, vertexShaderSource);
        gl.CompileShader(vertexShader);

        uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
        gl.ShaderSource(fragmentShader, fragmentShaderSource);
        gl.CompileShader(fragmentShader);

        shaderProgram = gl.CreateProgram();
        gl.AttachShader(shaderProgram, vertexShader);
        gl.AttachShader(shaderProgram, fragmentShader);
        gl.LinkProgram(shaderProgram);

        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);

        _window.SizeChanged += _window_SizeChanged;

        while (true)
        {
            // Process any messages in the queue
            _window.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

            buffer.Begin();
            float hue = (float)_stopwatch.Elapsed.TotalSeconds * 0.15f % 1;

            gl.Disable(EnableCap.DepthTest);

            gl.ClearColor(SilkColor.ByDrawingColor(SilkColor.FromHsv(new Vector4(1.0f * hue, 1.0f * 0.75f, 1.0f * 0.75f, 1.0f))));
            gl.Clear(ClearBufferMask.ColorBufferBit);

            gl.UseProgram(shaderProgram);
            gl.BindVertexArray(vao);
            gl.DrawArrays(GLEnum.Triangles, 0, 3);
            buffer.End();
        }
    }

    private void _window_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
    {
        buffer.UpdateSize((int)sender.Bounds.Width, (int)sender.Bounds.Height);
    }

    /// <inheritdoc/>
    public void Uninitialize()
    {
    }
}
