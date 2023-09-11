namespace Project5.Extension;

public static class Extension
{
    public static void Print(this object? obj)
    {
        Console.WriteLine(obj ?? "null");
    }
}