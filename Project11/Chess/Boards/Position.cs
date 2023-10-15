namespace Project11.Chess.Boards;

public readonly struct Position
{
    public int X { get; }
    public int Y { get; }

    public static readonly Position None = new(-1, -1);

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public Position Translate(int x, int y)
    {
        return new Position(X + x, Y + y);
    }

    public Position TranslateX(int x)
    {
        return new Position(X + x, Y);
    }

    public Position TranslateY(int y)
    {
        return new Position(X, Y + y);
    }

    public bool IsAdjacentTo(Position other, Direction direction)
    {
        return direction switch
        {
            Direction.Left => X - other.X == 1 && Y == other.Y,
            Direction.Top => X == other.X && Y - other.Y == -1,
            Direction.Right => X - other.X == -1 && Y == other.Y,
            Direction.Bottom => X == other.X && Y - other.Y == 1,
            _ => false
        };
    }

    public bool IsAdjacentTo(Position other, DiagonalDirection direction)
    {
        return direction switch
        {
            DiagonalDirection.TopLeft => X - other.X == 1 && Y - other.Y == -1,
            DiagonalDirection.TopRight => X - other.X == -1 && Y - other.Y == -1,
            DiagonalDirection.BottomRight => X - other.X == -1 && Y - other.Y == 1,
            DiagonalDirection.BottomLeft => X - other.X == 1 && Y - other.Y == 1,
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && X == other.X && Y == other.Y;
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    
}