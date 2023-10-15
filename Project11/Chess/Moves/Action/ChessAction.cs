using Project11.Chess.Pieces;

namespace Project11.Chess.Moves.Action;

public abstract class ChessAction
{
    public int Id { get; }
    
    /// <summary>
    /// Get or set a value that indicates whether this action caused a check.
    /// This value is set to False by default. The <see cref="ChessGame.MovePiece"/> method is responsible to set this value.
    /// </summary>
    /// <returns>A <see cref="bool"/> that determines whether this action caused a check.</returns>
    public bool IsCheck { get; set; }
    
    /// <summary>
    /// Get or set a value that indicates whether this action caused a checkmate.
    /// This value is set to False by default. The <see cref="ChessGame.MovePiece"/> method is responsible to set this value.
    /// </summary>
    /// <returns>A <see cref="bool"/> that determines whether this action caused a checkmate.</returns>
    public bool IsCheckmate { get; set; }
    
    /// <summary>
    /// Get a value that indicates whether this action is a <see cref="Pawn"/> promotion.
    /// If this value is set to True, the <see cref="PromotionPieceType"/> should also be set.
    /// </summary>
    /// <returns>A <see cref="bool"/> that determines whether this action is a <see cref="Pawn"/> promotion.</returns>
    public bool IsPromotion { get; }
    
    /// <summary>
    /// Get the <see cref="PieceType"/> of a piece after it got promoted.
    /// This value should be set if <see cref="IsPromotion"/> value is set to True.
    /// </summary>
    /// <returns>A <see cref="PieceType"/> that determines the type of piece after promotion.</returns>
    public PieceType? PromotionPieceType { get; set; }

    protected ChessAction(int id, bool isPromotion = false)
    {
        Id = id;
        IsCheck = false;
        IsCheckmate = false;
        IsPromotion = isPromotion;
    }

    /// <summary>
    /// Get <see cref="Move"/> instance which corresponds to <see cref="Piece"/> who makes this action.
    /// </summary>
    /// <returns>An instance of <see cref="Move"/> which corresponds to <see cref="Piece"/> who makes this action.</returns>
    public abstract Move GetPrimaryMove();

    /// <summary>
    /// Get list of <see cref="Move"/> instances who got involved in this action.
    /// </summary>
    /// <returns>A <see cref="List{Move}"/> whose elements are the <see cref="Move"/> instance who got involved in this action.</returns>
    public abstract IEnumerable<Move> GetMoves();

    #region HashCode and Equals Implementation

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is ChessAction other && other.Id == Id;
    }

    #endregion
}