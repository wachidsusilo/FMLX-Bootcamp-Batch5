using System;
using System.Runtime.InteropServices;

namespace Project12;


[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X { get; set; }

    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
