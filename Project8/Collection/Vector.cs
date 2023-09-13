using System.Collections;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace Project8.Collection;

public delegate void VoidPredicate<in T>(int index, T value);

public delegate bool BoolPredicate<in T>(T value);

public delegate TR MapPredicate<in T, out TR>(T value);

public class Vector<T> : IEnumerable
{
    private T[] _items;
    private int _size;
    private int _capacity;

    public int LastIndex => _size - 1;
    public int Size => _size;

    public event VoidPredicate<T>? OnItemAdded;
    public event VoidPredicate<T>? OnItemRemoved;

    public Vector(int capacity = 2)
    {
        if (_capacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Vector capacity cannot be a negative values"
            );
        }

        _items = new T[capacity];
        _size = 0;
        _capacity = capacity;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException(
                $"Attempt to access element at index {index} from a Vector of size {_size}"
            );
        }

        return _items[index];
    }

    public void Set(int index, T value)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException(
                $"Attempt to access element at index {index} from a Vector of size {_size}"
            );
        }

        _items[index] = value;
    }

    public void Add(T value)
    {
        var newSize = _size + 1;

        if (newSize >= _capacity)
        {
            Reallocate(_capacity + _capacity / 2);
        }

        _items[_size] = value;
        _size++;

        OnItemAdded?.Invoke(_size - 1, value);
    }

    public void Add(params T[] values)
    {
        var newSize = _size + values.Length;

        if (newSize >= _capacity)
        {
            var newCapacity = _capacity + _capacity / 2;
            var newCapacityBySize = newSize + newSize / 2;

            Reallocate(newCapacity > newSize ? newCapacity : newCapacityBySize);
        }

        for (var i = _size; i < newSize; i++)
        {
            _items[i] = values[i - _size];
            OnItemAdded?.Invoke(i, _items[i]);
        }

        _size = newSize;
    }

    public void Remove(T value)
    {
        var index = IndexOf(value);

        if (index != -1)
        {
            RemoveAt(index);
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException(
                $"Attempt to remove element at index {index} from a Vector of size {_size}"
            );
        }

        var value = _items[index];
        _size--;

        for (var i = index; i < _size; i++)
        {
            _items[i] = _items[i + 1];
        }

        OnItemRemoved?.Invoke(index, value);
    }

    public void RemoveIf(BoolPredicate<T> predicate)
    {
        var indices = new List<int>();

        for (var i = 0; i < _size; i++)
        {
            if (predicate?.Invoke(_items[i]) == true)
            {
                indices.Add(i);
            }
        }

        if (indices.Count == 0)
        {
            return;
        }

        var currentIndex = 0;

        for (var i = indices[0]; i < _size; i++)
        {
            if (i == indices[currentIndex])
            {
                currentIndex++;
                OnItemRemoved?.Invoke(i, _items[i]);
            }

            if (currentIndex >= indices.Count)
            {
                break;
            }

            _items[i] = _items[i + currentIndex];
        }

        _size -= indices.Count;
    }

    public int IndexOf(T value)
    {
        for (var i = 0; i < _size; i++)
        {
            if (_items[i]?.Equals(value) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public int IndexOf(BoolPredicate<T> predicate)
    {
        for (var i = 0; i < _size; i++)
        {
            if (predicate?.Invoke(_items[i]) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public int LastIndexOf(T value)
    {
        for (var i = _size - 1; i >= 0; i--)
        {
            if (_items[i]?.Equals(value) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public int LastIndexOf(BoolPredicate<T> predicate)
    {
        for (var i = _size - 1; i >= 0; i--)
        {
            if (predicate?.Invoke(_items[i]) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public bool Contains(T value)
    {
        return IndexOf(value) != -1;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public bool IsNotEmpty()
    {
        return _size != 0;
    }

    public void Clear()
    {
        for (var i = 0; i < _size; i++)
        {
            OnItemRemoved?.Invoke(i, _items[i]);
        }
        
        _size = 0;
        Reallocate(2);
    }

    public Vector<TR> Map<TR>(MapPredicate<T, TR> predicate)
    {
        var results = new Vector<TR>();

        for (var i = 0; i < _size; i++)
        {
            results.Add(predicate(_items[i]));
        }

        return results;
    }

    #region Interface implementation

    public IEnumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    #endregion

    #region Private methods

    private void Reallocate(int capacity)
    {
        var buffer = new T[capacity];

        if (capacity < _size)
        {
            _size = capacity;
        }

        for (var i = 0; i < _size; i++)
        {
            buffer[i] = _items[i];
        }

        _capacity = capacity;
        _items = buffer;
    }

    #endregion

    #region Override from object

    public override string ToString()
    {
        return $"Vector(size: {_size}, cap: {_capacity}, values: [{string.Join(", ", _items[.._size])}])";
    }

    #endregion
}