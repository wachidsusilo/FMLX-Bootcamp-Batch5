using Project11.Chess.Boards;

namespace Project11.Chess.Moves;

public readonly struct Move
{
    /// <summary>
    /// Get the id of the piece who make the move.
    /// </summary>
    /// <returns>An <see cref="int"/> that indicates the id of the piece.</returns>
    public readonly int PieceId;
    
    public readonly Position From;
    public readonly Position To;

    public Move(int pieceId, Position from, Position to)
    {
        PieceId = pieceId;
        From = from;
        To = to;
    }

    public override int GetHashCode()
    {
        return PieceId.GetHashCode() ^ From.GetHashCode() ^ To.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Move other && other.PieceId == PieceId && other.From == From && other.To == To;
    }

    public static bool operator ==(Move left, Move right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Move left, Move right)
    {
        return !(left == right);
    }
    
}