using Project11.Chess.Boards;

namespace Project11.Chess.Moves.Action;

public class CastlingShort : ChessAction
{
    private readonly Move _rook;
    private readonly Move _king;

    public CastlingShort(int id, Move rook, Move king) : base(id)
    {
        _rook = rook;
        _king = king;
    }

    public override Move GetPrimaryMove()
    {
        return _king;
    }

    public override IEnumerable<Move> GetMoves()
    {
        return new[] { _rook, _king };
    }

}