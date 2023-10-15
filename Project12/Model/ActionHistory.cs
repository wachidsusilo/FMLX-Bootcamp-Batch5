using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project12.Model;

public class ActionHistory : INotifyPropertyChanged
{
    private string _number;
    private string _notation;
    
    public int Id { get; }
    public string Tag => Id.ToString();

    public ActionHistory(int id, int number, string notation)
    {
        Id = id;
        _number = $"{number}.";
        _notation = notation;
    }

    public string Number
    {
        get => _number;
        set => SetField(ref _number, value);
    }

    public string Notation
    {
        get => _notation; 
        set => SetField(ref _notation, value);
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
    
    #region HashCode and Equals Implementation

    public override int GetHashCode()
    {
        return Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is ActionHistory history && history.Id == Id;
    }
    #endregion
    
}