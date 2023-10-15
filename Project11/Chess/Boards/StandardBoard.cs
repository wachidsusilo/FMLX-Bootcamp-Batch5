using Project11.Chess.Extension;
using Project11.Chess.Pieces;

namespace Project11.Chess.Boards;

public class StandardBoard : Board
{
    public StandardBoard() : base(8, 8)
    {
    }

    public override string GetNotationX(int x)
    {
        return x >= 0 && x < XNotations.Length ? XNotations[x].ToString() : "";
    }

    public override string GetNotationY(int y)
    {
        return y >= 0 && y < YNotations.Length ? YNotations[y].ToString() : "";
    }

    public override List<Position> GetPawnPositions(PieceColor color)
    {
        var results = new List<Position>();
        var posY = color.IsWhite() ? 1 : 6;

        for (var i = 0; i < Width; i++)
        {
            results.Add(new Position(i, posY));
        }

        return results;
    }

    public override List<Position> GetRookPositions(PieceColor color)
    {
        var posY = color.IsWhite() ? 0 : 7;
        
        return new List<Position>
        {
            new(0, posY),
            new(7, posY)
        };
    }

    public override List<Position> GetKnightPositions(PieceColor color)
    {
        var posY = color.IsWhite() ? 0 : 7;
        
        return new List<Position>
        {
            new(1, posY),
            new(6, posY)
        };
    }

    public override List<Position> GetBishopPositions(PieceColor color)
    {
        var posY = color.IsWhite() ? 0 : 7;

        return new List<Position>
        {
            new(2, posY),
            new(5, posY)
        };
    }

    public override Position GetQueenPosition(PieceColor color)
    {
        return new Position(3, color.IsWhite() ? 0 : 7);
    }

    public override Position GetKingPosition(PieceColor color)
    {
        return new Position(4, color.IsWhite() ? 0 : 7);
    }

    public override int GetPieceCount(PieceColor color)
    {
        return 16;
    }
    
    private const string XNotations = "abcdefgh";
    private const string YNotations = "12345678";
}