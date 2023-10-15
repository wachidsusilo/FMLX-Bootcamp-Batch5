using Project11.Chess.Extension;
using Project11.Chess.Pieces;

namespace Project11.Chess.Boards;

public abstract class Board
{
    /// <summary>
    /// Get the width of this instance.
    /// </summary>
    public readonly int Width;
    
    /// <summary>
    /// Get the height of this instance.
    /// </summary>
    public readonly int Height;

    protected Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Check whether the given <see cref="Position"/> instance is inside this board.
    /// </summary>
    /// <param name="position">a <see cref="Position"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether the given <see cref="Position"/>
    /// is inside this board.
    /// </returns>
    public bool Contains(Position position)
    {
        return position.X.In(0, Width - 1) && position.Y.In(0, Height - 1);
    }

    /// <summary>
    /// Get the symbol of the given <see cref="Position"/> instance.
    /// </summary>
    /// <param name="position">A <see cref="Position"/> instance.</param>
    /// <returns>
    /// A <see cref="string"/> value which represent the symbol of the given <see cref="Position"/> instance.
    /// </returns>
    public string GetNotation(Position position)
    {
        return $"{GetNotationX(position.X)}{GetNotationY(position.Y)}";
    }

    /// <summary>
    /// Get the symbol of the given <see cref="Position.X"/> value.
    /// </summary>
    /// <param name="x">An <see cref="int"/> value.</param>
    /// <returns>
    /// A <see cref="string"/> value which represent the symbol of the given <see cref="Position.X"/> value.
    /// </returns>
    public abstract string GetNotationX(int x);
    
    /// <summary>
    /// Get the symbol of the given <see cref="Position.Y"/> value.
    /// </summary>
    /// <param name="y">An <see cref="int"/> value.</param>
    /// <returns>
    /// A <see cref="string"/> value which represent the symbol of the given <see cref="Position.Y"/> value.
    /// </returns>
    public abstract string GetNotationY(int y);
    
    /// <summary>
    /// Get a list of <see cref="Position"/> instances for where the <see cref="Pawn"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="Pawn"/>.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances for where the <see cref="Pawn"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract List<Position> GetPawnPositions(PieceColor color);
    
    /// <summary>
    /// Get a list of <see cref="Position"/> instances for where the <see cref="Rook"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="Rook"/>.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances for where the <see cref="Rook"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract List<Position> GetRookPositions(PieceColor color);
    
    /// <summary>
    /// Get a list of <see cref="Position"/> instances for where the <see cref="Knight"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="Knight"/>.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances for where the <see cref="Knight"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract List<Position> GetKnightPositions(PieceColor color);
    
    /// <summary>
    /// Get a list of <see cref="Position"/> instances for where the <see cref="Bishop"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="Bishop"/>.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of <see cref="Position"/> instances for where the <see cref="Bishop"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract List<Position> GetBishopPositions(PieceColor color);
    
    /// <summary>
    /// Get a <see cref="Position"/> instance for where the <see cref="Queen"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="Queen"/>.</param>
    /// <returns>
    /// A <see cref="Position"/> instance for where the <see cref="Queen"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract Position GetQueenPosition(PieceColor color);
    
    /// <summary>
    /// Get a <see cref="Position"/> instance for where the <see cref="King"/> should be initially
    /// located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> of the <see cref="King"/>.</param>
    /// <returns>
    /// A <see cref="Position"/> instance for where the <see cref="King"/>
    /// should be initially located on the <see cref="Board"/>.
    /// </returns>
    public abstract Position GetKingPosition(PieceColor color);
    
    /// <summary>
    /// Get the number of <see cref="Piece"/> this instance contains for the given <see cref="PieceColor"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> instance.</param>
    /// <returns>
    /// An <see cref="int"/> instance that the determines the number of <see cref="Piece"/>
    /// this instance contains for the given <see cref="PieceColor"/>.
    /// </returns>
    public abstract int GetPieceCount(PieceColor color);
}