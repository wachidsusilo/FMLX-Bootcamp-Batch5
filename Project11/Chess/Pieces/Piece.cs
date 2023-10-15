using Project11.Chess.Boards;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public abstract class Piece
{
    public int Id { get; }

    /// <summary>
    /// Get symbol of this instance.
    /// </summary>
    /// <returns>
    /// "K" for <see cref="King"/>,
    /// "Q" for <see cref="Queen"/>, 
    /// "R" for <see cref="Rook"/>, 
    /// "N" for <see cref="Knight"/>, 
    /// "B" for <see cref="Bishop"/>,
    /// and empty <see cref="string"/> for <see cref="Pawn"/>.
    /// </returns>
    public string Notation { get; }

    /// <summary>
    /// Get color of this instance.
    /// </summary>
    /// <returns>A <see cref="PieceColor"/> that indicates the color of this instance.</returns>
    public PieceColor Color { get; }

    /// <summary>
    /// Get or set the <see cref="Position"/> of where this instance is currently located on the <see cref="Board"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Position"/> instance that indicates where is this instance should be located on the <see cref="Board"/>
    /// </returns>
    public Position Position { get; set; }

    /// <summary>
    /// Get the <see cref="Position.X"/> value of the <see cref="Position"/> of this instance.
    /// </summary>
    /// <returns>
    /// An <see cref="int"/> value that indicates the <see cref="Position.X"/> coordinate of the <see cref="Position"/>
    /// of this instance.
    /// </returns>
    public int X => Position.X;

    /// <summary>
    /// Get the <see cref="Position.Y"/> value of the <see cref="Position"/> of this instance.
    /// </summary>
    /// <returns>
    /// An <see cref="int"/> value that indicates the <see cref="Position.Y"/> coordinate of the <see cref="Position"/>
    /// of this instance.
    /// </returns>
    public int Y => Position.Y;

    /// <summary>
    /// Get or Set the number of how many times this instance has been moved.
    /// </summary>
    /// <returns>An <see cref="int"/> that indicates how many times this instance has been moved.</returns>
    public int MoveCount { get; private set; }

    /// <summary>
    /// Get a value that indicates whether this instance was already moved.
    /// </summary>
    /// <returns>A <see cref="bool"/> that determines whether this instance was already moved.</returns>
    public bool HasMoved => MoveCount != 0;

    protected Piece(int id, string notation, PieceColor color, Position position, int moveCount)
    {
        Id = id;
        Notation = notation;
        Color = color;
        Position = position;
        MoveCount = Math.Max(0, moveCount);
    }

    /// <summary>
    /// Increment the number of move this instance made.
    /// </summary>
    public void IncrementMoveCount()
    {
        MoveCount++;
    }

    /// <summary>
    /// Decrement the number of move this instance made.
    /// </summary>
    public void DecrementMoveCount()
    {
        if (MoveCount == 0)
        {
            return;
        }
        
        MoveCount--;
    }

    /// <summary>
    /// Check if this instance can attack tile at the given <see cref="Position"/>.
    /// </summary>
    /// <param name="game">A <see cref="ChessGame"/> instance.</param>
    /// <param name="position">A <see cref="Position"/> instance to check.</param>
    /// <returns>True if this instance can attack at the given <see cref="Position"/>, False otherwise.</returns>
    public abstract bool CanAttackTile(ChessGame game, Position position);
    
    /// <summary>
    /// Get the possible actions this instance can make.
    /// </summary>
    /// <param name="game">A <see cref="ChessGame"/> instance.</param>
    /// <returns>A <see cref="List{ChessGame}"/> of <see cref="ChessGame"/> instances.</returns>
    public abstract List<ChessAction> GetPossibleActions(ChessGame game);
    
    /// <summary>
    /// Get the path this instance made in order to move from one position to another.
    /// </summary>
    /// <param name="from">A <see cref="Position"/> instance that indicates where this instance is initially located.</param>
    /// <param name="to">A <see cref="Position"/> instance that indicates where is this instance want to move to.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances that passed by this instance when moving
    /// from one position to another.</returns>
    public abstract List<Position> GetPath(Position from, Position to);
    
    /// <summary>
    /// Get the path this instance made in order to move from the current position to another.
    /// </summary>
    /// <param name="position">A <see cref="Position"/> instance that indicates where is this instance want to move to.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances that passed by this instance when moving
    /// from the current position to another.</returns>
    public abstract List<Position> GetPathTo(Position position);

    #region HashCode and Equality Check

    public override int GetHashCode()
    {
        return Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Piece piece && piece.Id == Id;
    }

    #endregion

    #region Operator Overloading

    public static bool operator ==(Piece? left, Piece? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        return left?.Equals(right) == true;
    }

    public static bool operator !=(Piece? left, Piece? right)
    {
        return !(left == right);
    }

    #endregion
}