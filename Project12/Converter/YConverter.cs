using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Project12.Converter;

public class YConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int xy)
        {
            return 7 - xy;
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}