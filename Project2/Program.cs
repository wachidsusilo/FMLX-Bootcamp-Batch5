namespace Project2;

using Automotive;
using Automotive.Part;

internal static class Program
{
    private static void Main()
    {
        var jazz = new Car(
            "Honda",
            "Jazz",
            new GasolineEngine("RR", 8),
            new Tire("BS", 16, "Racing"),
            new HalogenLamp()
        );

        Console.WriteLine(jazz);

        jazz.Lamp?.TurnOn();    // Turning on the head lamp
        jazz.Lamp?.TurnOff();   // Turning off the head lamp

        jazz.Engine?.Run(300); // Will fail because engine is not started
        jazz.Engine?.Start();       // Starting the engine
        jazz.Engine?.Run(300); // Running the engine at 300 RPM
        jazz.Engine?.Shutdown();    // Will fail because engine is still running
        jazz.Engine?.Stop();        // Stopping the engine
        jazz.Engine?.Shutdown();    // Shutting down the engine

        jazz.Lamp = new IncandescentLamp();             // Change the head lamp to Incandescent Lamp
        jazz.Engine = new ElectricEngine("Tesla"); // Change the engine to Electric Engine
        Console.WriteLine(jazz);
       
        jazz.Lamp = new FluorescentLamp();                        // Change the head lamp to Fluorescent Lamp
        jazz.Engine = new DieselEngine("Volvo", 10);   // Change the engine to Diesel Engine
        Console.WriteLine(jazz);
        
    }
}