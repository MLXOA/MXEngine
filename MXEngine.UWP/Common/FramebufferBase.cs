using Silk.NET.DXGI;
using System;

namespace MXEngine.UWP.Common;
public abstract unsafe class FramebufferBase : IDisposable
{
    public abstract int FramebufferWidth { get; protected set; }

    public abstract int FramebufferHeight { get; protected set; }

    public abstract IDXGISwapChain1* SwapChainHandle { get; protected set; }

    public abstract void Dispose();
}