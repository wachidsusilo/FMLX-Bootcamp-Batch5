using System.Numerics;
using Project7.Extension;

namespace Project7.Drawing;

public class Rect<T> where T : INumber<T>
{
    public T X { get; set; }
    public T Y { get; set; }
    public T Width { get; set; }
    public T Height { get; set; }

    public T Left
    {
        get => X;
        set => X = value;
    }

    public T Right
    {
        get => X + Width;
        set => Width = value - X;
    }

    public T Top
    {
        get => Y;
        set => Y = value;
    }

    public T Bottom
    {
        get => Y + Height;
        set => Height = value - Y;
    }

    public Rect() : this(T.Zero, T.Zero, T.Zero, T.Zero)
    {
    }

    public Rect(T x, T y, T width, T height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public T GetArea()
    {
        return Width * Height;
    }

    public bool CheckCollision<T2>(Rect<T2> other) where T2: INumber<T2>
    {
        var isRightPassOtherLeft = Right.ToDouble() >= other.Left.ToDouble();
        var isLeftPassOtherRight = Left.ToDouble() <= other.Right.ToDouble();
        var isTopPassOtherBottom = Top.ToDouble() <= other.Bottom.ToDouble();
        var isBottomPassOtherTop = Bottom.ToDouble() >= other.Top.ToDouble();

        return isRightPassOtherLeft && isLeftPassOtherRight && isTopPassOtherBottom && isBottomPassOtherTop;
    }
}