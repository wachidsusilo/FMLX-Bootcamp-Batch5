using System.Text;

namespace Project4.Controller;

public class TowerController : BaseController
{
    public override bool CanWalk()
    {
        return false;
    }

    public override bool CanRun()
    {
        return false;
    }

    public override bool CanShoot()
    {
        return true;
    }

    protected override void OnWalk(int direction)
    {
        throw new Exception("IllegalStateException: Tower cannot walk.");
    }

    protected override void OnRun(int direction)
    {
        throw new Exception("IllegalStateException: Tower cannot run.");
    }

    protected override void OnStop(int oldState, int direction)
    {
        Console.WriteLine($"Tower Stop from {StateToString(oldState)} {DirectionToString(direction)} - Animation");
    }

    protected override void OnShoot()
    {
        Console.WriteLine("Tower Shoot - Animation");
    }

    public override string ToString()
    {
        return new StringBuilder("TowerController(")
            .Append($"State: {State} - {StateToString(State)}, ")
            .Append($"Direction: {Direction} - {DirectionToString(Direction)}, ")
            .Append($"CanWalk: {CanWalk()}, ")
            .Append($"CanRun: {CanRun()}, ")
            .Append($"CanShoot: {CanShoot()})")
            .ToString();
    }

}