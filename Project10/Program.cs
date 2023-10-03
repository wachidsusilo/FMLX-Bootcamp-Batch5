using System.Text;

namespace Project10;

internal static class Program
{
    private static void Main()
    {
        StringTest(100_000);
        StringBuilderTest(100_000);
    }

    private static void PrintPrimeNumbers(int count)
    {
        var n = 0;
        var x = 0;
        
        while (n < count)
        {
            if (IsPrime(x))
            {
                Console.Write($"{x}, ");
                n++;
            }

            x++;
        }
    }

    private static bool IsPrime(int n)
    {
        if (n <= 1)
        {
            return false;
        }

        for (var i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }

    private static void StringTest(int count)
    {
        var text = string.Empty;
        
        for (var i = 0; i < count; i++) {
            text += "a";
            text += "b";
            text = text.Replace('a', 'c');
        }
    }

    private static void StringBuilderTest(int count)
    {
        var text = new StringBuilder();
        
        for (var i = 0; i < count; i++) {
            text.Append('a');
            text.Append('b');
            text.Replace('a', 'c');
        }
    }
}