using Project11.Chess;
using Project11.Chess.Boards;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;
using Project11.Chess.Pieces;

namespace Project13;

public class ChessGameTest
{
    private ChessGame _game = new(new StandardBoard());
    
    [SetUp]
    public void Setup()
    {
        _game = new ChessGame(new StandardBoard());
    }
    
    [Test]
    public void FindPieceById_WhenFound_ReturnsNotNull()
    {
        var result = _game.FindPieceById(0);
        
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void FindPieceById_WhenNotFound_ReturnsNull()
    {
        var result = _game.FindPieceById(100);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindPieceByPosition_WhenFound_ReturnsNotNull()
    {
        var position = new Position(0, 0);
        var result = _game.FindPieceByPosition(position);
        
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void FindPieceByPosition_WhenNotFound_ReturnsNull()
    {
        var position = new Position(-1, -1);
        var result = _game.FindPieceByPosition(position);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindPiecesWhoCanAttack_WhenNotFound_ReturnsEmptyList()
    {
        var position = new Position(0, 0);
        var results = _game.FindPiecesWhoCanAttack(position, PieceColor.Black);
        
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void FindActionById_WhenFound_ReturnsNotNull()
    {
        var position = new Position(0, 1);  // White Pawn
        var actions = _game.GetPossibleActions(position);
        _game.MovePiece(actions[0]);
        var result = _game.FindActionById(0);
        
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void FindActionById_WhenNotFound_ReturnsNull()
    {
        var result = _game.FindActionById(0);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void MovePiece_WhenChessActionIsValid_ReturnsTrue()
    {
        var from = new Position(0, 1);
        var to = new Position(0, 2);
        var move = new Move(0, from, to); // Move White Pawn 1 step forward
        var action = new BasicMove(_game.ActionPointerId, move);
        var result = _game.MovePiece(action);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void MovePiece_WhenChessActionIsNotValid_ReturnsFalse()
    {
        var from = new Position(0, 1);
        var to = new Position(0, 2);
        var move = new Move(-1, from, to); // Move Nothing 1 step forward
        var action = new BasicMove(_game.ActionPointerId, move);
        var result = _game.MovePiece(action);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void PromotePawn_WhenSuccessful_ReturnsTrue()
    {
        var result = _game.PromotePawn(0, PieceType.Queen);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void PromotePawn_WhenFailed_ReturnsFalse()
    {
        var result = _game.PromotePawn(0, PieceType.King);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetPossibleActions_WhenAvailable_ReturnsNotEmptyList()
    {
        var position = new Position(0, 1); // Most Left White Pawn
        var results = _game.GetPossibleActions(position);
        
        Assert.That(results, Is.Not.Empty);
    }

    [Test]
    public void GetPossibleActions_WhenNotAvailable_ReturnsEmptyList()
    {
        var position = new Position(0, 0); // Left White Rook
        var results = _game.GetPossibleActions(position);
        
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void GetToAction_WhenSuccessful_ReturnsTrue()
    {
        var from = new Position(0, 1);
        var to = new Position(0, 2);
        var move = new Move(0, from, to); // Move White Pawn 1 step forward
        var action = new BasicMove(_game.ActionPointerId + 1, move);
        
        var result1 = _game.MovePiece(action);
        var result2 = _game.GoToAction(_game.ActionPointerId - 1); // Undo 1 action backward
        
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        });
    }

    [Test]
    public void GetToAction_WhenFailed_ReturnsTrue()
    {
        var from = new Position(0, 1);
        var to = new Position(0, 2);
        var move = new Move(0, from, to); // Move White Pawn 1 step forward
        var action = new BasicMove(_game.ActionPointerId, move);
        _game.MovePiece(action);
        
        var result = _game.GoToAction(_game.ActionPointerId + 1); // Redo 1 action forward
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetActionNotation_ReturnsCorrectResult()
    {
        var from = new Position(0, 1);
        var to = new Position(0, 2);
        var move = new Move(0, from, to); // Move White Pawn 1 step forward
        var action = new BasicMove(_game.ActionPointerId, move)
        {
            IsCheck = true
        };
        
        var result = _game.GetActionNotation(action);
        var expected = "a3+";
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsInCheck_WhenNotInCheck_ReturnsFalse()
    {
        var result = _game.IsInCheck(PieceColor.White);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPieceDefendingKing_WhenNotDefending_ReturnsFalse()
    {
        var piece = _game.FindPieceById(0)!;
        var result = _game.IsPieceDefendingKing(piece, out var _, out var _);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ContainsTile_WhenTileExist_ReturnsTrue()
    {
        var position = new Position(0, 0);
        var result = _game.ContainsTile(position);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void ContainsTile_WhenTileDoesNotExist_ReturnsFalse()
    {
        var position = new Position(-1, -1);
        var result = _game.ContainsTile(position);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsTileUnderAttack_WhenUnderAttack_ReturnsTrue()
    {
        var position = new Position(2, 2);
        var result = _game.IsTileUnderAttack(position, PieceColor.White);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsTileUnderAttack_WhenNotUnderAttack_ReturnsFalse()
    {
        var position = new Position(3, 3);
        var result = _game.IsTileUnderAttack(position, PieceColor.White);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsTileOccupied_WhenOccupied_ReturnsTrue()
    {
        var position = new Position(0, 0);
        var result = _game.IsTileOccupied(position);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsTileOccupied_WhenNotOccupied_ReturnsFalse()
    {
        var position = new Position(2, 2);
        var result = _game.IsTileOccupied(position);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsTileOccupiedBy_WhenOccupied_ReturnsTrue()
    {
        var position = new Position(0, 0);
        var result = _game.IsTileOccupiedBy(position, PieceColor.White);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsTileOccupiedBy_WhenNotOccupied_ReturnsFalse()
    {
        var position = new Position(2, 2);
        var result = _game.IsTileOccupiedBy(position, PieceColor.White);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetSurroundingTiles_ReturnsCorrectListOfPosition()
    {
        var position = new Position(1, 1);
        var results = _game.GetSurroundingTiles(position);
        var expected = new List<Position>
        {
            new(0, 1),
            new(0, 2),
            new(1, 2),
            new(2, 2),
            new(2, 1),
            new(2, 0),
            new(1, 0),
            new(0, 0),
        }.OrderBy(pos => pos.X).ThenBy(pos => pos.Y);
        
        Assert.That(results, Is.EqualTo(expected));
    }

}