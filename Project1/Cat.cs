namespace Animal;

public class Cat
{
    public string? Name;
    public string? Color;
    public string? Gender;
    public int? Age;
    public string? Race;
    public bool? IsStripe;

    public void Meow()
    {
        Console.WriteLine("Meow");
    }

    public void Eat()
    {
        Console.WriteLine("Eat");
    }

    public void Jump()
    {
        Console.WriteLine("Jump");
    }
}