using Project11.Chess.Boards;

namespace Project11.Chess.Moves.Action;

public class EnPassant : ChessAction
{
    private readonly Move _defender;
    private readonly Move _attacker;

    public EnPassant(int id, Move defender, Move attacker) : base(id)
    {
        _defender = defender;
        _attacker = attacker;
    }

    public override Move GetPrimaryMove()
    {
        return _attacker;
    }

    public override IEnumerable<Move> GetMoves()
    {
        return new[] { _defender, _attacker };
    }

}