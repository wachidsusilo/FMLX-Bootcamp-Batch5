using System;
using System.Runtime.InteropServices;

namespace Project12;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left { get; set; }

    public int Top { get; set; }

    public int Right { get; set; }

    public int Bottom { get; set; }

    public Rect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}
