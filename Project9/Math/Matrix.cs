using System.Collections;
using System.Numerics;
using System.Text;

namespace Project9.Math;

public class Matrix<T> : IEnumerable where T : INumber<T>
{
    private readonly T[,] _data;
    private readonly int _row;
    private readonly int _column;

    public int Row => _row;
    public int Column => _column;

    public Matrix(int row, int column)
    {
        _data = new T[row, column];
        _row = row;
        _column = column;
    }

    #region Get and Set

    public void Set(int row, int column, T value)
    {
        if (row < 0 || row >= _row || column < 0 || column >= _column)
        {
            throw new IndexOutOfRangeException(
                $"Attempt to access element at location [{row},{column}] from a [{_row},{_column}] Matrix"
            );
        }

        _data[row, column] = value;
    }

    public bool TrySet(int row, int column, T value)
    {
        if (row < 0 || row >= _row || column < 0 || column >= _column)
        {
            return false;
        }

        _data[row, column] = value;
        return true;
    }

    public T Get(int row, int column)
    {
        if (row < 0 || row >= _row || column < 0 || column >= _column)
        {
            throw new IndexOutOfRangeException(
                $"Attempt to access element at location [{row},{column}] from a [{_row},{_column}] Matrix"
            );
        }

        return _data[row, column];
    }

    public bool TryGet(int row, int column, out T value)
    {
        if (row < 0 || row >= _row || column < 0 || column >= _column)
        {
            value = T.Zero;
            return false;
        }

        value = _data[row, column];
        return true;
    }

    #endregion

    #region Matrix addition

    public static Matrix<T> operator +(Matrix<T> matrix, T value)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] + value;
            }
        }

        return result;
    }

    public static Matrix<T> operator +(T value, Matrix<T> matrix)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] + value;
            }
        }

        return result;
    }

    public static Matrix<T> operator +(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1._row != m2._row || m1._column != m2._column)
        {
            throw new ArgumentException(
                $"Attempt to add 2 matrices with different dimension, {m1._row}x{m1._column} and {m2._row}x{m2._column}."
            );
        }

        var result = new Matrix<T>(m1._row, m1._column);

        for (var i = 0; i < m1._row; i++)
        {
            for (var j = 0; j < m1._column; j++)
            {
                result[i, j] = m1[i, j] + m2[i, j];
            }
        }

        return result;
    }

    #endregion

    #region Matrix substraction

    public static Matrix<T> operator -(Matrix<T> matrix, T value)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] - value;
            }
        }

        return result;
    }

    public static Matrix<T> operator -(T value, Matrix<T> matrix)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] - value;
            }
        }

        return result;
    }

    public static Matrix<T> operator -(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1._row != m2._row || m1._column != m2._column)
        {
            throw new ArgumentException(
                $"Attempt to add 2 matrices with different dimension, {m1._row}x{m1._column} and {m2._row}x{m2._column}."
            );
        }

        var result = new Matrix<T>(m1._row, m1._column);

        for (var i = 0; i < m1._row; i++)
        {
            for (var j = 0; j < m1._column; j++)
            {
                result[i, j] = m1[i, j] - m2[i, j];
            }
        }

        return result;
    }

    #endregion

    #region Matrix multiplication

    public static Matrix<T> operator *(Matrix<T> matrix, T value)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] * value;
            }
        }

        return result;
    }

    public static Matrix<T> operator *(T value, Matrix<T> matrix)
    {
        var result = new Matrix<T>(matrix._row, matrix._column);

        for (var i = 0; i < matrix._row; i++)
        {
            for (var j = 0; j < matrix._column; j++)
            {
                result[i, j] = matrix[i, j] * value;
            }
        }

        return result;
    }

    public static Matrix<T> operator *(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1._column != m2._row)
        {
            throw new ArgumentException(
                $"Attempt to dot product 2 matrices with different column and row, {m1._row}x{m1._column} and {m2._row}x{m2._column}."
            );
        }

        var result = new Matrix<T>(m1._row, m2._column);

        for (var i = 0; i < m1._row; i++)
        {
            for (var j = 0; j < m2._column; j++)
            {
                for (var k = 0; k < m1._column; k++)
                {
                    result[i, j] += m1[i, k] * m2[k, j];
                }
            }
        }

        return result;
    }

    #endregion

    public T Min()
    {
        var result = T.CreateTruncating(double.MaxValue);

        for (var i = 0; i < _row; i++)
        {
            for (var j = 0; j < _column; j++)
            {
                if (_data[i, j] < result)
                {
                    result = _data[i, j];
                }
            }
        }

        return result;
    }

    public T Max()
    {
        var result = T.CreateTruncating(double.MinValue);

        for (var i = 0; i < _row; i++)
        {
            for (var j = 0; j < _column; j++)
            {
                if (_data[i, j] > result)
                {
                    result = _data[i, j];
                }
            }
        }

        return result;
    }

    public IEnumerator GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        var valueLen = Max().ToString()?.Length ?? 1;

        for (var i = 0; i < _row; i++)
        {
            sb.Append("  ");
            var isFirst = true;

            for (var j = 0; j < _column; j++)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(' ');
                }

                sb.Append(_data[i, j].ToString()?.PadRight(valueLen, ' '));
            }

            sb.Append('\n');
        }

        return sb.Remove(sb.Length - 1, 1).ToString();
    }

    public T this[int i, int j]
    {
        get => _data[i, j];
        set => _data[i, j] = value;
    }
}