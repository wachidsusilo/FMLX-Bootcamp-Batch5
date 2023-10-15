using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class Knight : Piece
{
    public Knight(int id, PieceColor color, Position position, int moveCount = 0) 
        : base(id, "N", color, position, moveCount)
    {
    }

    public override bool CanAttackTile(ChessGame game, Position position)
    {
        return game.GetLShapeTiles(Position).Exists(pos => pos == position);
    }

    public override List<ChessAction> GetPossibleActions(ChessGame game)
    {
        return game.GetLShapeTiles(Position)
            .Where(pos => !game.IsTileOccupiedBy(pos, Color))
            .Select<Position, ChessAction>(pos =>
            {
                var piece = game.FindPieceByPosition(pos);

                if (piece is null)
                {
                    return new BasicMove(game.ActionPointerId + 1, new Move(Id, Position, pos));
                }

                return new Capture(
                    game.ActionPointerId + 1,
                    new Move(piece.Id, piece.Position, Position.None),
                    new Move(Id, Position, pos)
                );
            }).ToList();
    }

    public override List<Position> GetPath(Position from, Position to)
    {
        var diffX = (to.X - from.X).Abs();
        var diffY = (to.Y - from.Y).Abs();

        if ((diffX == 2 && diffY == 1) || (diffX == 1 && diffY == 2))
        {
            return new List<Position> { from, to };
        }

        return new List<Position>();
    }

    public override List<Position> GetPathTo(Position position)
    {
        return GetPath(Position, position);
    }
}