using System;
using System.Runtime.InteropServices;

namespace Project12;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct WindowPlacement
{
    public int Length { get; set; }

    public int Flags { get; set; }

    public int ShowCmd { get; set; }

    public Point MinPosition { get; set; }

    public Point MaxPosition { get; set; }

    public Rect NormalPosition { get; set; }

}