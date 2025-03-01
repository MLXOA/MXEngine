using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MXEngine.UWP.Common;
using MXEngine.UWP.OpenGL;
using WinRT;
using Silk.NET.Core.Native;

namespace MXEngine.UWP.OpenGL;

public unsafe partial class GameControl : GameBase<Framebuffer>
{
    private RenderContext _context;
    private SwapChainPanel _swapChainPanel;

    public Settings Setting { get; set; } = new Settings();

    public override event Action Ready;
    public override event Action<TimeSpan> Render;
    public override event Action<object, TimeSpan> UpdateFrame;

    protected override void OnStart()
    {
        if (_context == null)
        {
            _context = new RenderContext(Setting);
            _swapChainPanel = new SwapChainPanel();

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            Content = _swapChainPanel;

            Ready?.Invoke();
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs sizeInfo)
    {
        if (_context != null && sizeInfo.NewSize.Width > 0 && sizeInfo.NewSize.Height > 0)
        {
            if (Framebuffer == null)
            {
                Framebuffer = new Framebuffer(_context, (int)ActualWidth, (int)ActualHeight);

                TransformGroup transformGroup = new();
                transformGroup.Children.Add(Framebuffer.FlipYTransform);
                transformGroup.Children.Add(Framebuffer.TranslateTransform);
                _swapChainPanel.RenderTransform = transformGroup;

                //WinRT.IInspectable nativeInspectable;
                //WinRT.IInspectable* nativePointer;

                //nativeInspectable = _swapChainPanel.As<WinRT.IInspectable>();
                //nativePointer = &nativeInspectable;
                //Guid g = new Guid("63aad0b8-7c24-40ff-85a8-640d944cc325");
                //ComObject.FromPtr((IUnknown*)nativePointer).QueryInterface(ref g, out ComObject nativeSwapChainPanel);
                //nativeSwapChainPanel.As<ISwapChainPanelNative>().SetSwapChain(Framebuffer.SwapChainHandle);

                _swapChainPanel.As<ISwapChainPanelNative>().SetSwapChain(Framebuffer.SwapChainHandle);
            }
            else
            {
                Framebuffer?.UpdateSize((int)ActualWidth, (int)ActualHeight);
            }
        }
    }

    protected override void OnDraw()
    {
        Framebuffer.Begin();

        Render?.Invoke(_stopwatch.Elapsed - _lastFrameStamp);

        Framebuffer.End();

        UpdateFrame?.Invoke(this, _stopwatch.Elapsed - _lastFrameStamp);
    }
}
