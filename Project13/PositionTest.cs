using Project11.Chess.Boards;

namespace Project13;

public class PositionTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Deconstruct_ReturnsCorrectResult()
    {
        var position = new Position(1, 2);
        var (x, y) = position;
        
        Assert.Multiple(() =>
        {
            Assert.That(x, Is.EqualTo(1));
            Assert.That(y, Is.EqualTo(2));
        });
    }

    [Test]
    public void TranslateX_WhenParameterIsPositive_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.TranslateX(10);
        var expected = new Position(10, 0);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void TranslateX_WhenParameterIsNegative_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.TranslateX(-10);
        var expected = new Position(-10, 0);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void TranslateY_WhenParameterIsPositive_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.TranslateY(10);
        var expected = new Position(0, 10);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void TranslateY_WhenParameterIsNegative_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.TranslateY(-10);
        var expected = new Position(0, -10);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Translate_WhenParameterIsPositive_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.Translate(10, 10);
        var expected = new Position(10, 10);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Translate_WhenParameterIsNegative_ReturnsCorrectResult()
    {
        var position = new Position(0, 0);
        var result = position.Translate(-10, -10);
        var expected = new Position(-10, -10);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsAdjacentTo_DirectionLeft_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(-1, 0);
        var result = position1.IsAdjacentTo(position2, Direction.Left);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DirectionTop_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(0, 1);
        var result = position1.IsAdjacentTo(position2, Direction.Top);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DirectionRight_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(1, 0);
        var result = position1.IsAdjacentTo(position2, Direction.Right);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DirectionBottom_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(0, -1);
        var result = position1.IsAdjacentTo(position2, Direction.Bottom);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DiagonalDirectionTopLeft_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(-1, 1);
        var result = position1.IsAdjacentTo(position2, DiagonalDirection.TopLeft);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DiagonalDirectionTopRight_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(1, 1);
        var result = position1.IsAdjacentTo(position2, DiagonalDirection.TopRight);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DiagonalDirectionBottomRight_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(1, -1);
        var result = position1.IsAdjacentTo(position2, DiagonalDirection.BottomRight);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAdjacentTo_DiagonalDirectionBottomLeft_ReturnsTrue()
    {
        var position1 = new Position(0, 0);
        var position2 = new Position(-1, -1);
        var result = position1.IsAdjacentTo(position2, DiagonalDirection.BottomLeft);
        
        Assert.That(result, Is.True);
    }

}