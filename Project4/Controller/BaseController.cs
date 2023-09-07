namespace Project4.Controller;

public abstract class BaseController
{
    public const int StateIdle = 0;
    public const int StateWalking = 1;
    public const int StateRunning = 2;

    public const int DirectionForward = 0;
    public const int DirectionBackward = 1;
    public const int DirectionLeft = 2;
    public const int DirectionRight = 3;

    private int _state = StateIdle;
    private int _direction = DirectionForward;

    public int State => _state;
    public int Direction => _direction;

    public abstract bool CanWalk();
    public abstract bool CanRun();
    public abstract bool CanShoot();

    protected abstract void OnWalk(int direction);
    protected abstract void OnRun(int direction);
    protected abstract void OnStop(int oldState, int direction);
    protected abstract void OnShoot();
    
    public void Walk(int direction)
    {
        if (!CanWalk())
        {
            return;
        }
        
        if (direction is < 0 or > 3)
        {
            throw new Exception($"IllegalArgumentException: {direction} is not a valid direction.");
        }

        _state = StateWalking;
        _direction = direction;
        
        OnWalk(_direction);
    }

    public void Run(int direction)
    {
        if (!CanRun())
        {
            return;
        }
        
        if (direction is < 0 or > 3)
        {
            throw new Exception($"IllegalArgumentException: {direction} is not a valid direction.");
        }

        _state = StateRunning;
        _direction = direction;
        
        OnRun(_direction);
    }

    public void Stop()
    {
        if (_state == StateIdle)
        {
            return;
        }

        var oldState = _state;
        _state = StateIdle;

        OnStop(oldState, _direction);
    }

    public void Shoot()
    {
        if (!CanShoot())
        {
            return;
        }
        
        OnShoot();
    }

    public static string StateToString(int state)
    {
        return state switch
        {
            StateIdle => "Idle",
            StateWalking => "Walking",
            StateRunning => "Running",
            _ => "Unknown"
        };
    }

    public static string DirectionToString(int direction)
    {
        return direction switch
        {
            DirectionForward => "Forward",
            DirectionBackward => "Backward",
            DirectionLeft => "Left",
            DirectionRight => "Right",
            _ => "Unknown"
        };
    }
}