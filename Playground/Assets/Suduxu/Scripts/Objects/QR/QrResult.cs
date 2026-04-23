using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct QrResult {
    public IntPtr ptr;
    public uint width;
    public uint size;
}