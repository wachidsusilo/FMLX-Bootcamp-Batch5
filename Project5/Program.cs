using System.Globalization;

namespace Project5;

using Extension;
using static Converter.Converter;

internal static class Program
{
    private static void Main()
    {
        var isSucceed = TryParseDigit("12h4j520dkk0", out var result);
        $"Parse digit {(isSucceed ? "success" : "failed")} with result: {result}".Print();
        
        isSucceed = TryParseDigit("abc", out result);
        $"Parse digit {(isSucceed ? "succeeded" : "failed")} with result: {result}".Print();

        var number = ParseNumberOrNull("abc");
        $"Parse number: {number?.ToString(CultureInfo.InvariantCulture) ?? "null"}".Print();
        
        number = ParseNumberOrNull("12.45");
        $"Parse number: {number?.ToString(CultureInfo.InvariantCulture) ?? "null"}".Print();
    }
}