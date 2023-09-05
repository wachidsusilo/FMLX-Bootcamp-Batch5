namespace Project2.Automotive;
using Part;

public class Car
{
    public string? Manufacturer;
    public string? Name;
    public Engine? Engine;
    public Tire? Tire;
    public ILamp? Lamp;

    public Car(
        string? manufacturer = null, 
        string? name = null, 
        Engine? engine = null, 
        Tire? tire = null, 
        ILamp? lamp = null)
    {
        Manufacturer = manufacturer;
        Name = name;
        Engine = engine;
        Tire = tire;
        Lamp = lamp;
    }

    public override string ToString()
    {
        return $"Car({Manufacturer}, {Name}, {Engine}, {Tire}, {Lamp})";
    }
}