using Project11.Chess.Boards;

namespace Project11.Chess.Moves.Action;

public class DoubleStep : ChessAction
{
    private readonly Move _move;

    public DoubleStep(int id, Move move) : base(id)
    {
        _move = move;
    }

    public override Move GetPrimaryMove()
    {
        return _move;
    }

    public Move GetMove()
    {
        return _move;
    }

    public override IEnumerable<Move> GetMoves()
    {
        return new[] { _move };
    }

}