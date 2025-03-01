using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.DXGI;
using Silk.NET.Vulkan;
using WinRT;

namespace MXEngine.UWP.Common;

[ComImport, Guid("63aad0b8-7c24-40ff-85a8-640d944cc325"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
unsafe partial interface ISwapChainPanelNative
{
    [PreserveSig]
    HResult SetSwapChain(IDXGISwapChain1* swapchain);
}