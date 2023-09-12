using System.Numerics;
using Project7.Extension;

namespace Project7.Drawing;

public class Point<T> where T : INumber<T>
{
    public T X { get; set; }
    public T Y { get; set; }

    public Point() : this(T.Zero, T.Zero)
    {
    }

    public Point(T x, T y)
    {
        X = x;
        Y = y;
    }

    public T DistanceTo<T2>(Point<T2> other) where T2 : INumber<T2>
    {
        var dx = other.X.ToDouble() - X.ToDouble();
        var dy = other.Y.ToDouble() - Y.ToDouble();

        return T.CreateTruncating(Math.Sqrt(dx * dx + dy * dy));
    }

    public Point<T3> As<T3>() where T3 : INumber<T3>
    {
        return new Point<T3>(T3.CreateTruncating(X), T3.CreateTruncating(Y));
    }
}