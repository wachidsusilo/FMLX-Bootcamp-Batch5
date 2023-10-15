using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class Rook : Piece
{
    public Rook(int id, PieceColor color, Position position, int moveCount = 0)
        : base(id, "R", color, position, moveCount)
    {
    }

    public override bool CanAttackTile(ChessGame game, Position position)
    {
        if (position == Position || !game.ContainsTile(position))
        {
            return false;
        }

        if (position.Y == Position.Y)
        {
            var tiles = game.GetOrthogonalTiles(Position, position.X - Position.X > 0 ? Direction.Right : Direction.Left);
            return tiles.Count != 0 && tiles.Exists(pos => pos == position);
        }

        if (position.X == Position.X)
        {
            var tiles = game.GetOrthogonalTiles(Position, position.Y - Position.Y > 0 ? Direction.Top : Direction.Bottom);
            return tiles.Count != 0 && tiles.Exists(pos => pos == position);
        }

        return false;
    }

    public override List<ChessAction> GetPossibleActions(ChessGame game)
    {
        var results = new List<ChessAction>();

        var leftTiles = game.GetOrthogonalTiles(Position, Direction.Left);
        var topTiles = game.GetOrthogonalTiles(Position, Direction.Top);
        var rightTiles = game.GetOrthogonalTiles(Position, Direction.Right);
        var bottomTiles = game.GetOrthogonalTiles(Position, Direction.Bottom);

        PopulateActions(in game, ref results, in leftTiles);
        PopulateActions(in game, ref results, in topTiles);
        PopulateActions(in game, ref results, in rightTiles);
        PopulateActions(in game, ref results, in bottomTiles);

        return results;
    }

    public override List<Position> GetPath(Position from, Position to)
    {
        if (to.Y == from.Y)
        {
            var fromPos = to.X - from.X > 0 ? from.X : to.X;
            var count = (to.X - from.X).Abs();

            var results = Enumerable.Range(fromPos, count + 1).Select(x => new Position(x, from.Y)).ToList();

            if (to.X - from.X < 0)
            {
                results.Reverse();
            }
            
            return results;
        }

        if (to.X == from.X)
        {
            var fromPos = to.Y - from.Y > 0 ? from.Y : to.Y;
            var count = (to.Y - from.Y).Abs();

            var results = Enumerable.Range(fromPos, count + 1).Select(y => new Position(from.X, y)).ToList();

            if (to.Y - from.Y < 0)
            {
                results.Reverse();
            }
            
            return results;
        }

        return new List<Position>();
    }

    public override List<Position> GetPathTo(Position position)
    {
        return GetPath(Position, position);
    }

    private void PopulateActions(in ChessGame game, ref List<ChessAction> list, in List<Position> positions)
    {
        if (positions.Count == 0)
        {
            return;
        }

        for (var i = 0; i < positions.Count - 1; i++)
        {
            list.Add(new BasicMove(game.ActionPointerId + 1, new Move(Id, Position, positions[i])));
        }

        var piece = game.FindPieceByPosition(positions.Last());

        if (piece is null)
        {
            list.Add(new BasicMove(game.ActionPointerId + 1, new Move(Id, Position, positions.Last())));
            return;
        }

        if (piece.Color != Color)
        {
            list.Add(
                new Capture(
                    game.ActionPointerId + 1,
                    new Move(piece.Id, piece.Position, Position.None),
                    new Move(Id, Position, positions.Last())
                )
            );
        }
    }
    
}