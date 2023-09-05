namespace Project2.Automotive.Part;

public interface ILamp
{
    public void TurnOn();
    public void TurnOff();
}

public class HalogenLamp : ILamp
{
    public void TurnOn()
    {
        Console.WriteLine("Halogen Lamp Turned On");
    }

    public void TurnOff()
    {
        Console.WriteLine("Halogen Lamp Turned Off");
    }

    public override string ToString()
    {
        return "HalogenLamp";
    }
}

public class FluorescentLamp : ILamp
{
    public void TurnOn()
    {
        Console.WriteLine("Fluorescent Lamp Turned On");
    }

    public void TurnOff()
    {
        Console.WriteLine("Fluorescent Lamp Turned Off");
    }

    public override string ToString()
    {
        return "FluorescentLamp";
    }
}

public class IncandescentLamp : ILamp
{
    public void TurnOn()
    {
        Console.WriteLine("Incandescent Lamp Turned On");
    }

    public void TurnOff()
    {
        Console.WriteLine("Incandescent Lamp Turned Off");
    }

    public override string ToString()
    {
        return "IncandescentLamp";
    }
}