using System.Numerics;

namespace Project7.Extension;

public static class NumberExtension
{
    public static double ToDouble<T>(this T value) where T: INumber<T>
    {
        return double.CreateTruncating(value);
    }
}