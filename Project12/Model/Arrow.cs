using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project11.Chess.Boards;

namespace Project12.Model;

public class Arrow : INotifyPropertyChanged
{
    private Position _from;
    private Position _to;

    public Position From
    {
        get => _from;
        set => SetField(ref _from, value);
    }

    public Position To
    {
        get => _to;
        set => SetField(ref _to, value);
    }

    public Arrow(Position from, Position to)
    {
        _from = from;
        _to = to;
    }

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    #endregion
}