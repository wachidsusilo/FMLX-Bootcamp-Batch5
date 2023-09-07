using System.Text;

namespace Project4.Controller;

public class PlayerController : BaseController
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
        return true;
    }

    protected override void OnWalk(int direction)
    {
        Console.WriteLine($"Player Walk {DirectionToString(direction)} - Animation");
    }

    protected override void OnRun(int direction)
    {
        Console.WriteLine($"Player Run {DirectionToString(direction)} - Animation");
    }

    protected override void OnStop(int oldState, int direction)
    {
        Console.WriteLine($"Player Stop from {StateToString(oldState)} {DirectionToString(direction)} - Animation");
    }

    protected override void OnShoot()
    {
        Console.WriteLine("Player Shoot - Animation");
    }

    public override string ToString()
    {
        return new StringBuilder("PlayerController(")
            .Append($"State: {State} - {StateToString(State)}, ")
            .Append($"Direction: {Direction} - {DirectionToString(Direction)}, ")
            .Append($"CanWalk: {CanWalk()}, ")
            .Append($"CanRun: {CanRun()}, ")
            .Append($"CanShoot: {CanShoot()})")
            .ToString();
    }
    
}