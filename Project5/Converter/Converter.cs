using System.Globalization;

namespace Project5.Converter;

public static class Converter
{
    public static bool TryParseDigit(in string value, out int result)
    {
        result = 0;

        if (value.Length == 0)
        {
            return false;
        }

        foreach (var c in value.Where(char.IsDigit))
        {
            result *= 10;
            result += c - '0';
        }

        return result != 0 || value.Contains('0');
    }

    public static float? ParseNumberOrNull(object any)
    {
        switch (any)
        {
            case int:
            case long:
            case double:
            case float:
            case decimal:
                return (float)any;
            case string s:
                return float.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : null;
            default:
                return null;
        }
    }
    
}