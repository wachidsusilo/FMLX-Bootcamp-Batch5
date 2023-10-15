using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Project12.Model;

public class ObservableDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>, INotifyPropertyChanged, INotifyCollectionChanged
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    #region IDictionary Implementation

    public int Count => _dictionary.Count;
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;
    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;
    
    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        NotifyItemAdded(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void AddRange(Dictionary<TKey, TValue> items)
    {
        if (items.Count <= 0)
        {
            return;
        }

        var newItems = items.Where(i => !_dictionary.ContainsKey(i.Key)).ToList();

        if (!newItems.Any())
        {
            return;
        }

        foreach (var item in newItems)
        {
            _dictionary.Add(item.Key, item.Value);
        }
        
        NotifyItemsAdded(newItems);
    }

    public bool Remove(TKey key)
    {
        if (!_dictionary.TryGetValue(key, out var value))
        {
            return false;
        }

        if (!_dictionary.Remove(key))
        {
            return false;
        }

        NotifyItemRemoved(key, value);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item))
        {
            return false;
        }

        NotifyItemRemoved(item.Key, item.Value);
        return true;
    }

    public void Clear()
    {
        _dictionary.Clear();
        NotifyItemsCleared();
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (_dictionary.TryGetValue(key, out var oldValue))
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                {
                    return;
                }

                _dictionary[key] = value;

                NotifyItemChanged(
                    new KeyValuePair<TKey, TValue>(key, value),
                    new KeyValuePair<TKey, TValue>(key, oldValue)
                );
            }
            else
            {
                Add(key, value);
            }
        }
    }

    #endregion

    #region INotifyCollectionChanged Implementation

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Keys));
        OnPropertyChanged(nameof(Values));
    }

    private void NotifyItemAdded(TKey key, TValue value)
    {
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TValue>(key, value)
            )
        );
    }

    private void NotifyItemsAdded(IList items)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
    }

    private void NotifyItemRemoved(TKey key, TValue value)
    {
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                new KeyValuePair<TKey, TValue>(key, value)
            )
        );
    }

    private void NotifyItemsCleared()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void NotifyItemChanged(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
    {
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem)
        );
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
