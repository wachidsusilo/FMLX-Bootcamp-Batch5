using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class Queen : Piece
{
    public Queen(int id, PieceColor color, Position position, int moveCount = 0) 
        : base(id, "Q", color, position, moveCount)
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

        var xDiff = position.X - Position.X;
        var yDiff = position.Y - Position.Y;

        if (xDiff.Abs() != yDiff.Abs())
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

        var leftTiles = game.GetOrthogonalTiles(Position, Direction.Left);
        var topTiles = game.GetOrthogonalTiles(Position, Direction.Top);
        var rightTiles = game.GetOrthogonalTiles(Position, Direction.Right);
        var bottomTiles = game.GetOrthogonalTiles(Position, Direction.Bottom);
        
        var topLeftTiles = game.GetDiagonalTiles(Position, DiagonalDirection.TopLeft);
        var topRightTiles = game.GetDiagonalTiles(Position, DiagonalDirection.TopRight);
        var bottomRightTiles = game.GetDiagonalTiles(Position, DiagonalDirection.BottomRight);
        var bottomLeftTiles = game.GetDiagonalTiles(Position, DiagonalDirection.BottomLeft);
        
        PopulateActions(in game, ref results, in leftTiles);
        PopulateActions(in game, ref results, in topTiles);
        PopulateActions(in game, ref results, in rightTiles);
        PopulateActions(in game, ref results, in bottomTiles);

        PopulateActions(in game, ref results, in topLeftTiles);
        PopulateActions(in game, ref results, in topRightTiles);
        PopulateActions(in game, ref results, in bottomRightTiles);
        PopulateActions(in game, ref results, in bottomLeftTiles);

        return results;
    }

    public override List<Position> GetPath(Position from, Position to)
    {
        if (to == from)
        {
            return new List<Position>();
        }
        
        List<Position> results;
        
        if (to.Y == from.Y)
        {
            var fromPos = to.X - from.X > 0 ? from.X : to.X;
            var count = (to.X - from.X).Abs();

            results = Enumerable.Range(fromPos, count + 1).Select(x => new Position(x, from.Y)).ToList();

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

            results = Enumerable.Range(fromPos, count + 1).Select(y => new Position(from.X, y)).ToList();

            if (to.Y - from.Y < 0)
            {
                results.Reverse();
            }
            
            return results;
        }

        var xDiff = to.X - from.X;
        var yDiff = to.Y - from.Y;
        var xSign = xDiff.Sign();
        var ySign = yDiff.Sign();

        if (xDiff.Abs() != yDiff.Abs())
        {
            return new List<Position>();
        }

        results = new List<Position>();

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