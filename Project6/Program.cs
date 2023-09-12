namespace Project6;

internal static class Program
{
    private static void Main()
    {
        var fooBar = new FooBar.FooBar(15)
        {
            { 3, "foo" },
            { 5, "bar" }
        };

        Console.WriteLine(fooBar.Execute()); 
        // Output: 0, 1, 2, foo, 4, bar, foo, 7, 8, foo, bar, 11, foo, 13, 14, foobar

        fooBar.Add(7, "moo");
        fooBar.SetCount(30);

        Console.WriteLine(fooBar.Execute()); 
        // Output: 0, 1, 2, foo, 4, bar, foo, moo, 8, foo, bar, 11, foo, 13, moo, foobar, 16, 17, foo, 19, bar, foomoo, 22, 23, foo, bar, 26, foo, moo, 29, foobar
    }
}