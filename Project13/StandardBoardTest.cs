using Project11.Chess.Boards;

namespace Project13;

public class StandardBoardTest
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public void StandardBoard_GetNotationX_ReturnsCorrectResult()
    {
        var position = new Position(1,2);
        var board = new StandardBoard();
        var result = board.GetNotationX(position.X);
        
        Assert.That(result, Is.EqualTo("b"));
    }

    [Test]
    public void StandardBoard_GetNotationY_ReturnsCorrectResult()
    {
        var position = new Position(1,2);
        var board = new StandardBoard();
        var result = board.GetNotationY(position.Y);
        
        Assert.That(result, Is.EqualTo("3"));
    }

    [Test]
    public void StandardBoard_GetNotation_ReturnsCorrectResult()
    {
        var position = new Position(1,2);
        var board = new StandardBoard();
        var result = board.GetNotation(position);
        
        Assert.That(result, Is.EqualTo("b3"));
    }

}