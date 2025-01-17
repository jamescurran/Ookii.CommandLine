﻿using System;

namespace Ookii.CommandLine.Terminal;

/// <summary>
/// Handles the lifetime of virtual terminal support.
/// </summary>
/// <remarks>
/// On Windows, this restores the terminal mode to its previous value when disposed or
/// destructed. On other platforms, this does nothing.
/// </remarks>
/// <threadsafety static="true" instance="false"/>
public sealed class VirtualTerminalSupport : IDisposable
{
    private readonly bool _supported;
    private IntPtr _handle;
    private readonly NativeMethods.ConsoleModes _previousMode;

    internal VirtualTerminalSupport(bool supported)
    {
        _supported = supported;
        GC.SuppressFinalize(this);
    }

    internal VirtualTerminalSupport(IntPtr handle, NativeMethods.ConsoleModes previousMode)
    {
        _supported = true;
        _handle = handle;
        _previousMode = previousMode;
    }

    /// <summary>
    /// Cleans up resources for the <see cref="VirtualTerminalSupport"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This method will disable VT support on Windows if it was enabled by the call to
    ///   <see cref="VirtualTerminal.EnableColor" qualifyHint="true"/> or
    ///   <see cref="VirtualTerminal.EnableVirtualTerminalSequences" qualifyHint="true"/> that
    ///   created this instance.
    /// </para>
    /// </remarks>
    ~VirtualTerminalSupport()
    {
        ResetConsoleMode();
    }

    /// <summary>
    /// Gets a value that indicates whether virtual terminal sequences are supported.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if virtual terminal sequences are supported; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsSupported => _supported;

    /// <summary>
    /// Cleans up resources for the <see cref="VirtualTerminalSupport"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This method will disable VT support on Windows if it was enabled by the call to
    ///   <see cref="VirtualTerminal.EnableColor" qualifyHint="true"/> or
    ///   <see cref="VirtualTerminal.EnableVirtualTerminalSequences" qualifyHint="true"/> that
    ///   created this instance.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        ResetConsoleMode();
        GC.SuppressFinalize(this);
    }

    private void ResetConsoleMode()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMethods.SetConsoleMode(_handle, _previousMode);
            _handle = IntPtr.Zero;
        }
    }
}
