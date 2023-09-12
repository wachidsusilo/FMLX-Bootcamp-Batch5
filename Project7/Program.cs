namespace Project7;

using Drawing;

internal static class Program
{
    private static void Main()
    {
        var p1 = new Point<int>(3, 4);
        var p2 = new Point<float>(6f, 8f);

        Console.WriteLine($"p1 to p2 distance is {p1.DistanceTo(p2)}");

        var rect1 = new Rect<int>(0, 0, 10, 10);
        var rect2 = new Rect<float>(5f, 5f, 10f, 10f);
        var rect3 = new Rect<decimal>(15M, 15M, 10M, 10M);

        Console.WriteLine($"rect1 area is {rect1.GetArea()}");
        Console.WriteLine($"rect2 area is {rect2.GetArea()}");
        Console.WriteLine($"rect3 area is {rect3.GetArea()}");
        
        Console.WriteLine($"rect1 and rect2 are {(rect1.CheckCollision(rect2) ? "colliding" : "not colliding")}");
        Console.WriteLine($"rect1 and rect3 are {(rect1.CheckCollision(rect3) ? "colliding" : "not colliding")}");
        Console.WriteLine($"rect2 and rect3 are {(rect2.CheckCollision(rect3) ? "colliding" : "not colliding")}");
    }
}