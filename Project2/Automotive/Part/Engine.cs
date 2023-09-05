namespace Project2.Automotive.Part;

public class Engine
{
    public string? Brand;
    public float Cylinder;
    
    private readonly string? _type;
    private bool _isStarted;
    private bool _isRunning;

    protected Engine(string? type)
    {
        _type = type;
    }

    public void Start()
    {
        Console.WriteLine($"{_type} Engine Started");
        _isStarted = true;
    }

    public bool Shutdown()
    {
        if (_isRunning)
        {
            Console.WriteLine($"Failed to Shutdown {_type} Engine. Stop the engine before shutting it down.");
            return false;
        }
        
        Console.WriteLine($"{_type} Engine is Shutting down");
        _isStarted = false;
        return true;
    }

    public bool Run(float speed)
    {
        if (!_isStarted)
        {
            Console.WriteLine($"Failed to Run {_type} Engine. Please start the engine before running.");
            return false;
        }
        
        _isRunning = true;
        Console.WriteLine($"{_type} Engine is Running with speed {speed} rpm");
        return true;
    }

    public void Stop()
    {
        _isRunning = false;
        Console.WriteLine($"{_type} Engine is Stopped");
    }

    public override string ToString()
    {
        return $"{_type}Engine({Brand}, {Cylinder})";
    }
}

public class GasolineEngine : Engine
{
    public GasolineEngine(string? brand = null, float cylinder = 0) : base("Gasoline")
    {
        Brand = brand;
        Cylinder = cylinder;
    }
}

public class DieselEngine : Engine
{
    public DieselEngine(string? brand = null, float cylinder = 0) : base("Diesel")
    {
        Brand = brand;
        Cylinder = cylinder;
    }
}

public class ElectricEngine : Engine
{
    public ElectricEngine(string? brand = null, float cylinder = 0) : base("Electric")
    {
        Brand = brand;
        Cylinder = cylinder;
    }
}

public class HybridEngine : Engine
{
    public HybridEngine(string? brand = null, float cylinder = 0) : base("Hybrid")
    {
        Brand = brand;
        Cylinder = cylinder;
    }
}