namespace Project11.Chess.Moves.Action;

public class Capture : ChessAction
{
    private readonly Move _defender;
    private readonly Move _attacker;

    public Capture(
        int id,
        Move defender,
        Move attacker,
        bool isPromotion = false
    ) : base(id, isPromotion)
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