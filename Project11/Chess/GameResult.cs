using Project11.Chess.Pieces;

namespace Project11.Chess;

public enum GameResult
{
    /// <summary>
    /// Indicates that the game was won by the <see cref="PieceColor.White"/> side.
    /// </summary>
    WhiteWin,
    
    /// <summary>
    /// Indicates that the game was won by the <see cref="PieceColor.Black"/> side.
    /// </summary>
    BlackWin,
    
    /// <summary>
    /// Indicates that the result of the game was a draw because of a stalemate.
    /// A stalemate is a condition where there is no valid move for the pieces while the king
    /// is not currently in check.
    /// </summary>
    DrawByStalemate,
    
    /// <summary>
    /// Indicates that the result of the game was a draw because of a threefold-repetition.
    /// A threefold-repetition is a condition where the pieces appear at the same <see cref="Boards.Position"/>
    /// for 3 times on the <see cref="Boards.Board"/> throughout the game history. The appearance doesn't have to be
    /// occurred consecutively.
    /// </summary>
    DrawByThreefoldRepetition,
    
    /// <summary>
    /// Indicates that the result of the game was a draw because of a fifty-move-rule.
    /// A fifty-move-rule is a condition where there is no <see cref="Pawn"/> has been moved and
    /// there is no <see cref="Piece"/> has been captured for 50 consecutive moves.
    /// A <see cref="PieceColor.White"/> move followed by a <see cref="PieceColor.Black"/> move
    /// is considered as 1 move. In that case, the total number of individual moves
    /// is actually 100 times.
    /// </summary>
    DrawByFiftyMoveRule
}