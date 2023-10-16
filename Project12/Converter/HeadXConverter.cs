using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Project11.Chess.Boards;

namespace Project12.Converter;

public class HeadXConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [double tileSize, Position from, Position to, double thickness, string tag] ||
            !double.TryParse(tag, out var angle))
        {
            return DependencyProperty.UnsetValue;
        }
        
        var halfTileSize = tileSize / 2.0;

        var x1 = halfTileSize + from.X * tileSize;
        var y1 = halfTileSize + from.Y * tileSize;

        var x2 = halfTileSize + to.X * tileSize;
        var y2 = halfTileSize + to.Y * tileSize;

        var theta = Math.Atan2(y2 - y1, x2 - x1) + angle * Math.PI / 180;
        var length = 15 * thickness / 8;
        var x = x2 + length * Math.Cos(theta);
        
        return double.IsNaN(x) ? x2 : x;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}