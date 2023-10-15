using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;

namespace Project11.Chess.Pieces;

public class Pawn : Piece
{
    public Pawn(int id, PieceColor color, Position position, int moveCount = 0) 
        : base(id, "", color, position, moveCount)
    {
    }

    public override bool CanAttackTile(ChessGame game, Position position)
    {
        var diagonalLeft = Position.Translate(-1, Color.IsWhite() ? 1 : -1);
        var diagonalRight = Position.Translate(1, Color.IsWhite() ? 1 : -1);

        return position == diagonalLeft || position == diagonalRight;
    }

    public override List<ChessAction> GetPossibleActions(ChessGame game)
    {
        var frontPosition = Position.TranslateY(Color.IsWhite() ? 1 : -1);
        var doubleAdvance = Position.TranslateY(Color.IsWhite() ? 2 : -2);
        var diagonalLeft = Position.Translate(-1, Color.IsWhite() ? 1 : -1);
        var diagonalRight = Position.Translate(1, Color.IsWhite() ? 1 : -1);

        var diagonalLeftPiece = game.FindPieceByPosition(diagonalLeft);
        var diagonalRightPiece = game.FindPieceByPosition(diagonalRight);

        var isPromotion = Color.IsWhite()
            ? frontPosition.Y == game.BoardHeight - 1
            : frontPosition.Y == 0;

        var results = new List<ChessAction>();

        if (!game.IsTileOccupied(frontPosition))
        {
            results.Add(
                new BasicMove(
                    game.ActionPointerId + 1,
                    new Move(Id, Position, frontPosition),
                    isPromotion: isPromotion
                )
            );

            if (!HasMoved && !game.IsTileOccupied(doubleAdvance))
            {
                results.Add(new DoubleStep(game.ActionPointerId + 1, new Move(Id, Position, doubleAdvance)));
            }
        }

        if (diagonalLeftPiece != null && diagonalLeftPiece.Color != Color)
        {
            results.Add(
                new Capture(
                    game.ActionPointerId + 1,
                    new Move(diagonalLeftPiece.Id, diagonalLeftPiece.Position, Position.None),
                    new Move(Id, Position, diagonalLeft),
                    isPromotion: isPromotion
                )
            );
        }

        if (diagonalRightPiece != null && diagonalRightPiece.Color != Color)
        {
            results.Add(
                new Capture(
                    game.ActionPointerId + 1,
                    new Move(diagonalRightPiece.Id, diagonalRightPiece.Position, Position.None),
                    new Move(Id, Position, diagonalRight),
                    isPromotion: isPromotion
                )
            );
        }

        if (game.LastAction is not DoubleStep doubleStep)
        {
            return results;
        }

        var doubleStepPiece = game.FindPieceById(doubleStep.GetPrimaryMove().PieceId);

        if (doubleStepPiece is null || doubleStepPiece.Color == Color)
        {
            return results;
        }

        if (diagonalLeftPiece is null && Position.IsAdjacentTo(doubleStep.GetMove().To, Direction.Left))
        {
            results.Add(
                new EnPassant(
                    game.ActionPointerId + 1,
                    new Move(doubleStepPiece.Id, doubleStepPiece.Position, Position.None),
                    new Move(Id, Position, diagonalLeft)
                )
            );
        }

        if (diagonalRightPiece is null && Position.IsAdjacentTo(doubleStep.GetMove().To, Direction.Right))
        {
            results.Add(
                new EnPassant(
                    game.ActionPointerId + 1,
                    new Move(doubleStepPiece.Id, doubleStepPiece.Position, Position.None),
                    new Move(Id, Position, diagonalRight)
                )
            );
        }

        return results;
    }

    public override List<Position> GetPath(Position from, Position to)
    {
        var frontPosition = from.TranslateY(Color.IsWhite() ? 1 : -1);
        var doubleAdvance = from.TranslateY(Color.IsWhite() ? 2 : -2);
        var diagonalLeft = from.Translate(-1, Color.IsWhite() ? 1 : -1);
        var diagonalRight = from.Translate(1, Color.IsWhite() ? 1 : -1);

        if (to == frontPosition)
        {
            return new List<Position> { from, frontPosition };
        }

        if (to == doubleAdvance)
        {
            return new List<Position> { from, frontPosition, doubleAdvance };
        }

        if (to == diagonalLeft)
        {
            return new List<Position> { from, diagonalLeft };
        }

        if (to == diagonalRight)
        {
            return new List<Position> { from, diagonalRight };
        }

        return new List<Position>();
    }

    public override List<Position> GetPathTo(Position position)
    {
        return GetPath(Position, position);
    }
}