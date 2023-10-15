namespace Project11.Chess.Moves.Action;

public class BasicMove : ChessAction
{
    private readonly Move _move;

    public BasicMove(
        int id,
        Move move,
        bool isPromotion = false
    ) 
        : base(id, isPromotion)
    {
        _move = move;
    }

    public override Move GetPrimaryMove()
    {
        return _move;
    }

    public override IEnumerable<Move> GetMoves()
    {
        return new[] { _move };
    }
}