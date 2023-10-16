using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Project11.Chess.Boards;

namespace Project12.Converter;

public class LineXConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [double tileSize, Position position, double thickness])
        {
            return DependencyProperty.UnsetValue;
        }

        return tileSize / 2.0 + position.X * tileSize;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}