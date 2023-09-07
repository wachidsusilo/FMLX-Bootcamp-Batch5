using System.Text;

namespace Project4.Controller;

public class PetController : BaseController
{
    public override bool CanWalk()
    {
        return true;
    }

    public override bool CanRun()
    {
        return true;
    }

    public override bool CanShoot()
    {
        return false;
    }

    protected override void OnWalk(int direction)
    {
        Console.WriteLine($"Pet Walk {DirectionToString(direction)} - Animation");
    }

    protected override void OnRun(int direction)
    {
        Console.WriteLine($"Pet Run {DirectionToString(direction)} - Animation");
    }

    protected override void OnStop(int oldState, int direction)
    {
        Console.WriteLine($"Pet Stop from {StateToString(oldState)} {DirectionToString(direction)} - Animation");
    }

    protected override void OnShoot()
    {
        throw new Exception("IllegalStateException: Pet cannot shoot.");
    }
    
    public override string ToString()
    {
        return new StringBuilder("PetController(")
            .Append($"State: {State} - {StateToString(State)}, ")
            .Append($"Direction: {Direction} - {DirectionToString(Direction)}, ")
            .Append($"CanWalk: {CanWalk()}, ")
            .Append($"CanRun: {CanRun()}, ")
            .Append($"CanShoot: {CanShoot()})")
            .ToString();
    }
    
}