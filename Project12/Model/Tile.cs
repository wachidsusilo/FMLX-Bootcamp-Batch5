using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Pieces;

namespace Project12.Model;

public class Tile : INotifyPropertyChanged
{
    private Brush? _overlay;
    private ImageSource? _image;
    private ImageSource? _imageHint;
    private Position _position;

    private string _topLeftText;
    private string _topRightText;
    private string _bottomRightText;
    private string _bottomLeftText;

    public TileOverlay TileOverlay { get; private set; }
    public TileHint TileHint { get; private set; }

    public Tile(string tag, Position position)
    {
        Tag = tag;
        _position = position;

        _topLeftText = "";
        _topRightText = "";
        _bottomRightText = "";
        _bottomLeftText = "";

        TileOverlay = TileOverlay.None;
        TileHint = TileHint.None;

        CornerRadius = new CornerRadius(
            X == 0 && Y == 7 ? 4 : 0,
            X == 7 && Y == 7 ? 4 : 0,
            X == 7 && Y == 0 ? 4 : 0,
            X == 0 && Y == 0 ? 4 : 0
        );

        if (Y % 2 == 0)
        {
            Background = X % 2 == 0
                ? Application.Current.FindResource("TileBlack") as SolidColorBrush
                : Application.Current.FindResource("TileWhite") as SolidColorBrush;

            Foreground = X % 2 != 0
                ? Application.Current.FindResource("TileBlack") as SolidColorBrush
                : Application.Current.FindResource("TileWhite") as SolidColorBrush;
        }
        else
        {
            Background = X % 2 != 0
                ? Application.Current.FindResource("TileBlack") as SolidColorBrush
                : Application.Current.FindResource("TileWhite") as SolidColorBrush;

            Foreground = X % 2 == 0
                ? Application.Current.FindResource("TileBlack") as SolidColorBrush
                : Application.Current.FindResource("TileWhite") as SolidColorBrush;
        }
    }

    public void SetPiece(Piece? piece)
    {
        if (piece == null)
        {
            Image = null;
            return;
        }
        
        Image = piece switch
        {
            Pawn => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhitePawn.png" : "BlackPawn.png")}"
                )
            ),
            Rook => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhiteRook.png" : "BlackRook.png")}"
                )
            ),
            Knight => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhiteKnight.png" : "BlackKnight.png")}"
                )
            ),
            Bishop => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhiteBishop.png" : "BlackBishop.png")}"
                )
            ),
            Queen => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhiteQueen.png" : "BlackQueen.png")}"
                )
            ),
            King => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/{(piece.Color.IsWhite() ? "WhiteKing.png" : "BlackKing.png")}"
                )
            ),
            _ => null
        };
    }

    public void SetOverlay(TileOverlay overlay)
    {
        TileOverlay = overlay;
        Overlay = overlay switch
        {
            TileOverlay.LastMove => Application.Current.FindResource("TileLastMove") as SolidColorBrush,
            TileOverlay.Highlight => Application.Current.FindResource("TileHighlight") as SolidColorBrush,
            TileOverlay.Check => Application.Current.FindResource("TileCheck") as SolidColorBrush,
            _ => null
        };
    }

    public void SetHint(TileHint hint)
    {
        TileHint = hint;
        ImageHint = hint switch
        {
            TileHint.Move => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/MoveHint.png"
                )
            ),
            
            TileHint.Capture => new BitmapImage(
                new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly()?.GetName().Name};component/Images/CaptureHint.png"
                )
            ),
            
            _ => null
        };
    }

    public void ShowHint(bool show)
    {
        SetHint(show ? Image == null ? TileHint.Move : TileHint.Capture : TileHint.None);
    }

    public void SetImage(ImageSource? image)
    {
        Image = image;
    }

    public Brush? Background { get; }

    public Brush? Foreground { get; }

    public Brush? Overlay
    {
        get => _overlay;
        private set => SetField(ref _overlay, value);
    }

    public ImageSource? Image
    {
        get => _image;
        private set => SetField(ref _image, value);
    }

    public ImageSource? ImageHint
    {
        get => _imageHint;
        private set => SetField(ref _imageHint, value);
    }

    public string Tag { get; }

    public CornerRadius CornerRadius { get; }
    public int X => Position.X;
    public int Y => Position.Y;

    public Position Position
    {
        get => _position;
        set => SetField(ref _position, value);
    }
    
    public string TopLeftText
    {
        get => _topLeftText;
        set => SetField(ref _topLeftText, value);
    }

    public string TopRightText
    {
        get => _topRightText;
        set => SetField(ref _topRightText, value);
    }

    public string BottomRightText
    {
        get => _bottomRightText;
        set => SetField(ref _bottomRightText, value);
    }

    public string BottomLeftText
    {
        get => _bottomLeftText;
        set => SetField(ref _bottomLeftText, value);
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