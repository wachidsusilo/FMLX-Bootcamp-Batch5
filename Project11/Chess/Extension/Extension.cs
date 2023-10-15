using Project11.Chess.Pieces;

namespace Project11.Chess.Extension;

public static class Extension
{
    /// <summary>
    /// Get the opposite color of a <see cref="PieceColor"/> instance.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> instance.</param>
    /// <returns>
    /// A <see cref="PieceColor"/> instance where the value is the opposite of the given <see cref="color"/>.
    /// </returns>
    public static PieceColor Inverse(this PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    /// <summary>
    /// Check whether this instance is equal to <see cref="PieceColor.White"/>.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> instance to check.</param>
    /// <returns>True if this instance is equal to <see cref="PieceColor.White"/>, False otherwise.</returns>
    public static bool IsWhite(this PieceColor color)
    {
        return color == PieceColor.White;
    }

    /// <summary>
    /// Get the sign of an <see cref="int"/> instance.
    /// </summary>
    /// <param name="value">An <see cref="int"/> instance to get the sign from.</param>
    /// <returns>
    /// -1 if the <see cref="value"/> is negative, 1 if the <see cref="value"/> is positive, and 0 if the <see cref="value"/> is zero.
    /// </returns>
    public static int Sign(this int value)
    {
        return Math.Sign(value);
    }

    /// <summary>
    /// Get the absolute value of an <see cref="int"/> instance.
    /// </summary>
    /// <param name="value">An <see cref="int"/> value.</param>
    /// <returns>An absolute value of <see cref="int"/>.</returns>
    public static int Abs(this int value)
    {
        return Math.Abs(value);
    }

    /// <summary>
    /// Check whether this instance is in the range of the given <see cref="from"/> and <see cref="to"/> value.
    /// The <see cref="from"/> value doesn't necessarily has to be smaller than the <see cref="to"/> value.
    /// </summary>
    /// <param name="value">An <see cref="int"/> instance to check.</param>
    /// <param name="from">An <see cref="int"/> instance for the lower boundary (inclusive).</param>
    /// <param name="to">An <see cref="int"/> instance for the higher boundary (inclusive).</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether the <see cref="int"/> instance is
    /// inside the range.</returns>
    public static bool In(this int value, int from, int to)
    {
        if (from > to)
        {
            return value >= to && value <= from;
        }

        return value >= from && value <= to;
    }

}