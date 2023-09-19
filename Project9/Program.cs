namespace Project9;
using Math;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine();
        
        var m1 = new Matrix<int>(2, 2)
        {
            [0, 0] = 1,
            [0, 1] = 2,
            [1, 0] = 3,
            [1, 1] = 4
        };
        
        Print("Creating m1:", m1);

        m1 += 20;
        Print("Add m1 with scalar 20:", m1);

        m1 -= 10;
        Print("Subtract m1 with scalar 10:", m1);

        var m2 = new Matrix<int>(2, 2)
        {
            [0, 0] = 0,
            [0, 1] = 0,
            [1, 0] = 0,
            [1, 1] = 0
        };
        
        Print("Creating m2:", m2);

        m2 += 20;
        Print("Add m2 with scalar 20:", m2);

        m2 -= 10;
        Print("Subtract m2 with scalar 10:", m1);
        
        var m3 = m1 + m2;
        Print("Add m1 and m2:", m3);

        var m4 = m1 * m2;
        Print("Multiply m1 and m2:", m4);

        var m5 = new Matrix<int>(3, 3)
        {
            [0,0] = 1,
            [0,1] = 1,
            [0,2] = 1,
            [1,0] = 1,
            [1,1] = 1,
            [1,2] = 1,
            [2,0] = 1,
            [2,1] = 1,
            [2,2] = 1,
        };

        try
        {
            var m6 = m1 + m5;
            Print("Add m1 and m5:", m6);
        }
        catch (Exception e)
        {
            Print("Add m1 and m5:", e.Message);
        }
        
        try
        {
            var m6 = m1 * m5;
            Print("Multiply m1 and m5:", m6);
        }
        catch (Exception e)
        {
            Print("Multiply m1 and m5:", e.Message);
        }
    }

    private static void Print<T>(string label, T value)
    {
        Console.WriteLine(label);
        Console.WriteLine(value);
        Console.WriteLine();
    }
}

