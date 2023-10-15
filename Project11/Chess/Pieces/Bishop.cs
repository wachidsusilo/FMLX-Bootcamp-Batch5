using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class Bishop : Piece
{
    public Bishop(int id, PieceColor color, Position position, int moveCount = 0)
        : base(id, "B", color, position, moveCount)
    {
    }

    public override bool CanAttackTile(ChessGame game, Position position)
    {
        var xDiff = position.X - Position.X;
        var yDiff = position.Y - Position.Y;

        if (position == Position || !game.ContainsTile(position) || xDiff.Abs() != yDiff.Abs())
        {
            return false;
        }

        var direction = xDiff switch
        {
            > 0 when yDiff > 0 => DiagonalDirection.TopRight,
            > 0 when yDiff < 0 => DiagonalDirection.BottomRight,
            < 0 when yDiff < 0 => DiagonalDirection.BottomLeft,
            _ => DiagonalDirection.TopLeft
        };

        return game.GetDiagonalTiles(Position, direction).Exists(pos => pos == position);
    }

    public override List<ChessAction> GetPossibleActions(ChessGame game)
    {
        var results = new List<ChessAction>();

        var topLeftTiles = game.GetDiagonalTiles(Position, DiagonalDirection.TopLeft);
        var topRightTiles = game.GetDiagonalTiles(Position, DiagonalDirection.TopRight);
        var bottomRightTiles = game.GetDiagonalTiles(Position, DiagonalDirection.BottomRight);
        var bottomLeftTiles = game.GetDiagonalTiles(Position, DiagonalDirection.BottomLeft);

        PopulateActions(in game, ref results, in topLeftTiles);
        PopulateActions(in game, ref results, in topRightTiles);
        PopulateActions(in game, ref results, in bottomRightTiles);
        PopulateActions(in game, ref results, in bottomLeftTiles);

        return results;
    }

    public override List<Position> GetPath(Position from, Position to)
    {
        var xDiff = to.X - from.X;
        var yDiff = to.Y - from.Y;
        var xSign = xDiff.Sign();
        var ySign = yDiff.Sign();

        if (to == from || xDiff.Abs() != yDiff.Abs())
        {
            return new List<Position>();
        }

        var results = new List<Position>();

        for (int x = from.X, y = from.Y;
             (xSign < 0 ? x >= to.X : x <= to.X) && (ySign < 0 ? y >= to.Y : y <= to.Y);
             x += xSign, y += ySign)
        {
            results.Add(new Position(x, y));
        }

        return results;
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