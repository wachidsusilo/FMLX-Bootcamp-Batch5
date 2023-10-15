using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class King : Piece
{
    public King(int id, PieceColor color, Position position, int moveCount = 0)
        : base(id, "K", color, position, moveCount)
    {
    }

    public override bool CanAttackTile(ChessGame game, Position position)
    {
        if (position == Position)
        {
            return false;
        }
        
        for (var x = Position.X - 1; x <= Position.X + 1; x++)
        {
            for (var y = Position.Y - 1; y <= Position.Y + 1; y++)
            {
                if (x == position.X && y == position.Y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override List<ChessAction> GetPossibleActions(ChessGame game)
    {
        var posY = Color == PieceColor.White ? 0 : game.BoardHeight - 1;

        var results = new List<ChessAction>();
        
        foreach (var position in game.GetSurroundingTiles(Position))
        {
            if (position == Position)
            {
                continue;
            }

            var piece = game.FindPieceByPosition(position);

            if (piece is null)
            {
                if (!game.IsTileUnderAttack(position, Color.Inverse()))
                {
                    results.Add(new BasicMove(game.ActionPointerId + 1, new Move(Id, Position, position)));
                }

                continue;
            }

            if (piece.Color == Color || game.IsTileUnderAttack(position, Color.Inverse()))
            {
                continue;
            }

            results.Add(
                new Capture(
                    game.ActionPointerId + 1,
                    new Move(piece.Id, piece.Position, Position.None),
                    new Move(Id, Position, position)
                )
            );
        }

        if (CanCastlingRight(game) &&
            game.FindPieceByPosition(new Position(game.BoardWidth - 1, posY)) is Rook rightRook)
        {
            var king = new Move(Id, Position, new Position(6, posY));
            var rook = new Move(rightRook.Id, rightRook.Position, new Position(5, posY));

            results.Add(new CastlingShort(game.ActionPointerId + 1, rook, king));
        }

        if (CanCastlingLeft(game) && game.FindPieceByPosition(new Position(0, posY)) is Rook leftRook)
        {
            var king = new Move(Id, Position, new Position(2, posY));
            var rook = new Move(leftRook.Id, leftRook.Position, new Position(3, posY));

            results.Add(new CastlingLong(game.ActionPointerId + 1, rook, king));
        }

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

    private bool CanCastlingRight(ChessGame game)
    {
        var posY = Color == PieceColor.White ? 0 : game.BoardHeight - 1;
        var piece = game.FindPieceByPosition(new Position(game.BoardWidth - 1, posY));

        if (HasMoved || piece is not Rook rook || rook.HasMoved || game.IsTileUnderAttack(Position, Color.Inverse()))
        {
            return false;
        }

        var positions = GetPathTo(rook.Position).Where(pos => pos != Position && pos != rook.Position).ToList();

        if (positions.Count < 2)
        {
            return false;
        }

        return positions.All(pos => !game.IsTileOccupied(pos)) &&
               !game.IsTileUnderAttack(Position.TranslateX(1), Color.Inverse()) &&
               !game.IsTileUnderAttack(Position.TranslateX(2), Color.Inverse());
    }

    private bool CanCastlingLeft(ChessGame game)
    {
        var posY = Color == PieceColor.White ? 0 : game.BoardHeight - 1;
        var piece = game.FindPieceByPosition(new Position(0, posY));

        if (HasMoved || piece is not Rook rook || rook.HasMoved || game.IsTileUnderAttack(Position, Color.Inverse()))
        {
            return false;
        }

        var positions = GetPathTo(rook.Position).Where(pos => pos != Position && pos != rook.Position).ToList();

        if (positions.Count < 2)
        {
            return false;
        }

        return positions.All(pos => !game.IsTileOccupied(pos)) &&
               !game.IsTileUnderAttack(Position.TranslateX(1), Color.Inverse()) &&
               !game.IsTileUnderAttack(Position.TranslateX(2), Color.Inverse());
    }

}