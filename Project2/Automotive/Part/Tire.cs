namespace Project2.Automotive.Part;

public class Tire
{
    public string? Manufacturer;
    public float Size;
    public string? Type;

    public Tire(string? manufacturer = null, float size = 0, string? type = null)
    {
        Manufacturer = manufacturer;
        Size = size;
        Type = type;
    }

    public override string ToString()
    {
        return $"Tire({Manufacturer}, {Size}, {Type})";
    }
}