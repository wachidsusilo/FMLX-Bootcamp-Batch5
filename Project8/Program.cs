using Project8.Collection;

namespace Project8;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine();
        
        var intVector = new Vector<int> { 1, 2, 3 };
        Console.WriteLine(intVector);

        intVector.OnItemAdded += OnAdded;
        intVector.OnItemRemoved += OnRemoved;

        intVector.Add(1, 2, 3);
        Console.WriteLine(intVector);

        intVector.RemoveIf(value => value > 2);
        Console.WriteLine(intVector);

        var stringVector = intVector.Map(value =>
        {
            return value switch
            {
                1 => "One",
                2 => "Two",
                _ => "None"
            };
        });
        Console.WriteLine(stringVector);
        
        intVector.Clear();
        Console.WriteLine(intVector);
    }

    private static void OnAdded<T>(int index, T value)
    {
        Console.WriteLine($"Item added: {{index: {index}, value: {value}}}");
    }

    private static void OnRemoved<T>(int index, T value)
    {
        Console.WriteLine($"Item removed: {{index: {index}, value: {value}}}");
    }
}