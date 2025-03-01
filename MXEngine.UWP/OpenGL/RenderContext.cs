using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using MXEngine.UWP.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.OpenGL;
using Silk.NET.WGL.Extensions.NV;
using Silk.NET.Windowing;
using Windows.UI.Popups;

namespace MXEngine.UWP.OpenGL;

public unsafe class RenderContext
{
    private static Settings _sharedContextSettings;
    private static int _sharedContextReferenceCount;

    public static GL GL { get; private set; }

    public static NVDXInterop NVDXInterop { get; private set; }

    public Format Format { get; }

    public IntPtr DxDeviceFactory { get; }

    public IntPtr DxDeviceHandle { get; }

    public IntPtr DxDeviceContext { get; }

    public IntPtr GlDeviceHandle { get; }

    public static IWindow GlWindow { get; private set; }

    public RenderContext(Settings settings)
    {
        IDXGIFactory2* factory;
        ID3D11Device* device;
        ID3D11DeviceContext* devCtx;

        var dxgi = DXGI.GetApi();

        HResult result = dxgi.CreateDXGIFactory2(0, SilkMarshal.GuidPtrOf<IDXGIFactory2>(), (void**)&factory);

        // Device
        {
            D3D11.GetApi().CreateDevice(null, D3DDriverType.Hardware, 0, 0, null, 0, D3D11.SdkVersion, &device, null, &devCtx);
        }

        DxDeviceFactory = (IntPtr)factory;
        DxDeviceHandle = (IntPtr)device;
        DxDeviceContext = (IntPtr)devCtx;

        GetOrCreateSharedOpenGLContext(settings);

        GlDeviceHandle = NVDXInterop.DxopenDevice(device);
    }

    private static void GetOrCreateSharedOpenGLContext(Settings settings)
    {
        if (_sharedContextSettings == null)
        {
            WindowOptions options = WindowOptions.Default;

            options.API = new GraphicsAPI(ContextAPI.OpenGL, settings.GraphicsProfile, settings.GraphicsContextFlags, new APIVersion(settings.MajorVersion, settings.MinorVersion));
            options.IsVisible = false;

            GlWindow = Window.Create(options);

            GlWindow.Initialize();

            GL = GlWindow.CreateOpenGL();
            NVDXInterop = new(GL.Context);

            _sharedContextSettings = settings;
        }

        Interlocked.Increment(ref _sharedContextReferenceCount);
    }
}