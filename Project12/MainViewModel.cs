using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Project11.Chess;
using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;
using Project11.Chess.Pieces;
using Project12.Model;

namespace Project12;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly SoundManager _soundManager;
    
    private Player _playerWhite;
    private Player _playerBlack;

    private ChessGame _game;
    private readonly Dictionary<string, ChessAction> _hintNotations;
    private readonly List<string> _highlightNotations;
    private readonly List<string> _lastMoveNotations;
    private readonly List<string> _checkNotations;

    private int _pawnPromotionPieceId;

    private double _boardSize;
    private double _tileSize;
    private double _boardFontSize;
    private double _arrowThickness;
    private string _gameStatus;
    private PieceColor _currentTurn;
    private string _actionPointerId;
    private double _numberMinWidth;
    private double _promotionCardWidth;
    private double _promotionCardHeight;
    private Thickness _promotionCardContentMargin;
    private Thickness _promotionCardContentMarginEnd;
    private Visibility _promotionCardVisibility;
    private Visibility _movableTileVisibility;
    private Visibility _arrowPreviewVisibility;
    private Arrow _arrowPreview;

    public bool IsGameOver => _game.GameStatus == Project11.Chess.GameStatus.GameOver;
    public ObservableDictionary<string, Tile> Tiles { get; }
    public Tile MovableTile { get; }
    public ObservableCollection<ActionHistory> WhiteHistories { get; }
    public ObservableCollection<ActionHistory> BlackHistories { get; }
    public ObservableDictionary<string, Arrow> Arrows { get; }

    public MainViewModel()
    {
        _soundManager = new SoundManager();
        
        _playerWhite = new Player(0, "Koala");
        _playerBlack = new Player(1, "Tiger");

        _game = new ChessGame(new StandardBoard());
        _hintNotations = new Dictionary<string, ChessAction>();
        _highlightNotations = new List<string>();
        _lastMoveNotations = new List<string>();
        _checkNotations = new List<string>();

        _pawnPromotionPieceId = -1;

        _boardSize = 0;
        _tileSize = 0;
        _boardFontSize = SystemFonts.MessageFontSize;
        _arrowThickness = 8;
        _gameStatus = "White Move";
        _currentTurn = PieceColor.White;
        _actionPointerId = "-1";
        _numberMinWidth = 24;
        _promotionCardWidth = 0;
        _promotionCardHeight = 0;
        _promotionCardContentMargin = new Thickness();
        _promotionCardContentMarginEnd = new Thickness();
        _promotionCardVisibility = Visibility.Collapsed;
        _movableTileVisibility = Visibility.Collapsed;
        _arrowPreviewVisibility = Visibility.Collapsed;
        _arrowPreview = new Arrow(Position.None, Position.None);

        Tiles = new ObservableDictionary<string, Tile>();
        MovableTile = new Tile("mt", new Position(0, 0));
        WhiteHistories = new ObservableCollection<ActionHistory>();
        BlackHistories = new ObservableCollection<ActionHistory>();
        Arrows = new ObservableDictionary<string, Arrow>();

        Reset();
    }

    #region Event Listeners

    private void OnTurnChanged(PieceColor color)
    {
        CurrentTurn = color;
        GameStatus = $"{color} Move";
    }

    private void OnActionAdded(PieceColor color, ChessAction action)
    {
        if (color.IsWhite())
        {
            WhiteHistories.Add(new ActionHistory(action.Id, WhiteHistories.Count + 1, _game.GetActionNotation(action)));
        }
        else
        {
            BlackHistories.Add(new ActionHistory(action.Id, BlackHistories.Count + 1, _game.GetActionNotation(action)));
        }

        if (action.IsCheck)
        {
            _soundManager.Play(SoundManager.ChessSound.MoveCheck);
            return;
        }
        
        if (action is Capture)
        {
            _soundManager.Play(SoundManager.ChessSound.Capture);
            return;
        }

        if (action.IsPromotion)
        {
            _soundManager.Play(SoundManager.ChessSound.Promote);
            return;
        }
        
        _soundManager.Play(SoundManager.ChessSound.MoveSelf);
    }

    private void OnActionRemoved(PieceColor color, ChessAction action)
    {
        if (color.IsWhite())
        {
            for (var i = 0; i < WhiteHistories.Count; i++)
            {
                if (WhiteHistories[i].Id != action.Id)
                {
                    continue;
                }
                
                WhiteHistories.Remove(WhiteHistories[i]);
                break;
            }
        }
        else
        {
            for (var i = 0; i < BlackHistories.Count; i++)
            {
                if (BlackHistories[i].Id != action.Id)
                {
                    continue;
                }
                
                BlackHistories.Remove(BlackHistories[i]);
                break;
            }
        }
    }

    private void OnActionUpdated(PieceColor color, ChessAction action)
    {
        if (color.IsWhite())
        {
            foreach (var history in WhiteHistories)
            {
                if (history.Id != action.Id)
                {
                    continue;
                }

                history.Notation = _game.GetActionNotation(action);
                break;
            }
        }
        else
        {
            foreach (var history in BlackHistories)
            {
                if (history.Id != action.Id)
                {
                    continue;
                }

                history.Notation = _game.GetActionNotation(action);
                break;
            }
        }
        
        if (action.IsCheck)
        {
            _soundManager.Play(SoundManager.ChessSound.MoveCheck);
            return;
        }
        
        if (action is Capture)
        {
            _soundManager.Play(SoundManager.ChessSound.Capture);
            return;
        }

        if (action.IsPromotion)
        {
            _soundManager.Play(SoundManager.ChessSound.Promote);
            return;
        }
        
        _soundManager.Play(SoundManager.ChessSound.MoveSelf);
    }

    private void OnActionPointerChanged(int pointerId)
    {
        ActionPointerId = pointerId.ToString();
    }

    private void OnPieceMoved(Piece piece, Move move, bool isBackward)
    {
        if (Tiles.TryGetValue(_game.GetTileNotation(move.From), out var from))
        {
            from.SetPiece(null);
        }
        
        if (_game.ContainsTile(move.To) && Tiles.TryGetValue(_game.GetTileNotation(move.To), out var to))
        {
            to.SetPiece(piece);
        }

        if (move.To != Position.None)
        {
            ClearTileLastMove();
            ClearTileCheck();
            ClearTileHint();
        }
        
        if (isBackward || move.From == Position.None || move.To == Position.None)
        {
            return;
        }
        
        ShowTileLastMove(piece.GetPath(move.From, move.To).Select(pos => _game.GetTileNotation(pos)));
    }

    private void OnPieceUpdated(Piece piece)
    {
        if (Tiles.TryGetValue(_game.GetTileNotation(piece.Position), out var tile))
        {
            tile.SetPiece(piece);
        }
    }

    private void OnPawnPromotion(Pawn pawn)
    {
        _pawnPromotionPieceId = pawn.Id;
        PromotionCardVisibility = Visibility.Visible;
    }

    private void OnChecked(King king, List<Piece> attackers)
    {
        var positions = attackers
            .Select(piece => piece.GetPathTo(king.Position).Where(pos => pos != piece.Position))
            .SelectMany(positions => positions);
        
        foreach (var position in positions)
        {
            ShowTileCheck(_game.GetTileNotation(position));
        }
    }

    private void OnGameOver(GameResult result)
    {
        GameStatus = result switch
        {
            GameResult.WhiteWin => "White Win",
            GameResult.BlackWin => "Black Win",
            GameResult.DrawByStalemate => "Draw by Stalemate",
            GameResult.DrawByThreefoldRepetition => "Draw by Threefold Repetition",
            GameResult.DrawByFiftyMoveRule => "Draw by Fifty Move Rule",
            _ => ""
        };
        
        _soundManager.Play(SoundManager.ChessSound.Notify);
    }

    #endregion

    public void Reset()
    {
        ClearTileHint();
        ClearTileHighlight();
        ClearTileLastMove();
        ClearTileCheck();

        _game = new ChessGame(new StandardBoard());
        _game.PieceMoved += OnPieceMoved;
        _game.PieceUpdated += OnPieceUpdated;
        _game.PawnPromotion += OnPawnPromotion;
        _game.Checked += OnChecked;
        _game.GameOver += OnGameOver;
        _game.TurnChanged += OnTurnChanged;
        _game.ActionAdded += OnActionAdded;
        _game.ActionRemoved += OnActionRemoved;
        _game.ActionUpdated += OnActionUpdated;
        _game.ActionPointerChanged += OnActionPointerChanged;

        Tiles.Clear();
        WhiteHistories.Clear();
        BlackHistories.Clear();
        GameStatus = "White Move";

        for (var x = 0; x < _game.BoardWidth; x++)
        {
            for (var y = 0; y < _game.BoardHeight; y++)
            {
                var position = new Position(x, y);
                var tile = new Tile(_game.GetTileNotation(position), position);

                if (x == 0)
                {
                    tile.TopLeftText = _game.GetTileNotationY(position);
                }

                if (y == 0)
                {
                    tile.BottomRightText = _game.GetTileNotationX(position);
                }

                tile.SetPiece(_game.FindPieceByPosition(position));

                Tiles.Add(_game.GetTileNotation(position), tile);
            }
        }
    }

    public bool MovePiece(ChessAction action)
    {
        return _game.MovePiece(action);
    }

    public void PromotePawn(PieceType type)
    {
        if (_pawnPromotionPieceId == -1)
        {
            return;
        }
        
        _game.PromotePawn(_pawnPromotionPieceId, type);
        _pawnPromotionPieceId = -1;
    }

    public void UndoAction()
    {
        if (_game.LastAction?.Id is { } id)
        {
            _game.GoToAction(id - 1);   
        }
    }

    public void RedoAction()
    {
        if (_game.LastAction?.Id is { } id)
        {
            _game.GoToAction(id + 1);   
        }
    }

    public void GoToAction(int actionId)
    {
        _game.GoToAction(actionId);
    }

    public ChessAction? GetChessActionRelatedToTile(string tileNotation)
    {
        foreach (var (_, value) in _hintNotations)
        {
            if (_game.GetTileNotation(value.GetPrimaryMove().To) == tileNotation)
            {
                return value;
            }
        }

        return null;
    }

    public bool StartDraggingTile(string tileNotation)
    {
        if (!Tiles.TryGetValue(tileNotation, out var tile))
        {
            return false;
        }

        var piece = _game.FindPieceByPosition(tile.Position);

        if (piece is null || piece.Color != _game.CurrentTurn)
        {
            return false;
        }
        
        MovableTile.SetImage(tile.Image);
        tile.SetImage(null);
        MovableTileVisibility = Visibility.Visible;
        return true;
    }

    public void StopDraggingTile(string tileNotation, bool resetOriginalTile = true)
    {
        if (MovableTile.Image is null || !Tiles.TryGetValue(tileNotation, out var tile))
        {
            return;
        }

        if (resetOriginalTile && _game.IsTileOccupied(tile.Position))
        {
            tile.SetImage(MovableTile.Image);
        }
        
        MovableTile.SetImage(null);
        MovableTileVisibility = Visibility.Collapsed;
    }

    public string GetTileNotation(Position position)
    {
        return _game.GetTileNotation(position);
    }

    public bool ContainsTile(Position position)
    {
        return _game.ContainsTile(position);
    }

    public void ToggleTileHighlight(string tileNotation)
    {
        if (!Tiles.TryGetValue(tileNotation, out var tile))
        {
            return;
        }

        var index = _highlightNotations.FindIndex(notation => notation == tileNotation);

        if (index != -1)
        {
            tile.SetOverlay(
                _lastMoveNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                    ? TileOverlay.LastMove
                    : _checkNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                        ? TileOverlay.Check
                        : TileOverlay.None
            );

            tile.TopRightText = "";

            _highlightNotations.RemoveAt(index);
        }
        else
        {
            tile.SetOverlay(TileOverlay.Highlight);
            tile.TopRightText = tileNotation;
            _highlightNotations.Add(tileNotation);
        }
    }

    public void ClearTileHighlight()
    {
        if (_highlightNotations.Count == 0)
        {
            return;
        }

        foreach (var highlightNotation in _highlightNotations)
        {
            if (!Tiles.TryGetValue(highlightNotation, out var tile))
            {
                continue;
            }

            tile.SetOverlay(
                _lastMoveNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                    ? TileOverlay.LastMove
                    : _checkNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                        ? TileOverlay.Check
                        : TileOverlay.None
            );

            tile.TopRightText = "";
        }

        _highlightNotations.Clear();
    }

    public void ShowTileLastMove(IEnumerable<string> tileNotations)
    {
        _lastMoveNotations.AddRange(tileNotations);

        foreach (var lastMoveNotation in _lastMoveNotations)
        {
            if (Tiles.TryGetValue(lastMoveNotation, out var tile) && tile.TileOverlay == TileOverlay.None)
            {
                tile.SetOverlay(TileOverlay.LastMove);
            }
        }
    }

    public void ClearTileLastMove()
    {
        foreach (var lastMoveNotation in _lastMoveNotations)
        {
            if (Tiles.TryGetValue(lastMoveNotation, out var tile) && tile.TileOverlay == TileOverlay.LastMove)
            {
                tile.SetOverlay(TileOverlay.None);
            }
        }

        _lastMoveNotations.Clear();
    }

    public void ShowTileHint(string tileNotation)
    {
        if (!Tiles.TryGetValue(tileNotation, out var tile))
        {
            return;
        }

        var actions = _game.GetPossibleActions(tile.Position);

        foreach (var action in actions)
        {
            var notation = _game.GetTileNotation(action.GetPrimaryMove().To);

            if (!Tiles.TryGetValue(notation, out var tileTo))
            {
                continue;
            }

            _hintNotations.Add(notation, action);

            if (action is EnPassant)
            {
                tileTo.SetHint(TileHint.Capture);
            }
            else
            {
                tileTo.ShowHint(true);
            }
        }
    }

    public void ClearTileHint()
    {
        if (_hintNotations.Count == 0)
        {
            return;
        }

        foreach (var hint in _hintNotations)
        {
            if (Tiles.TryGetValue(hint.Key, out var tile))
            {
                tile.ShowHint(false);
            }
        }

        _hintNotations.Clear();
    }

    private void ShowTileCheck(string tileNotation)
    {
        if (!Tiles.TryGetValue(tileNotation, out var tile))
        {
            return;
        }

        if (tile.TileOverlay == TileOverlay.None)
        {
            tile.SetOverlay(TileOverlay.Check);
        }

        _checkNotations.Add(tileNotation);
    }

    private void ClearTileCheck()
    {
        if (_checkNotations.Count == 0)
        {
            return;
        }

        foreach (var checkNotation in _checkNotations)
        {
            if (!Tiles.TryGetValue(checkNotation, out var tile))
            {
                continue;
            }

            tile.SetOverlay(
                _highlightNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                    ? TileOverlay.Highlight
                    : _lastMoveNotations.Exists(notation => notation == _game.GetTileNotation(tile.Position))
                        ? TileOverlay.LastMove
                        : TileOverlay.None
            );
        }

        _checkNotations.Clear();
    }

    public void AddArrow(Position from, Position to)
    {
        var key = new StringBuilder()
            .Append(_game.GetTileNotation(from))
            .Append(_game.GetTileNotation(to))
            .ToString();
        
        if (from == to || Arrows.ContainsKey(key))
        {
            return;
        }

        Arrows.Add(key, new Arrow(from, to));
    }

    public void ClearArrows()
    {
        if (Arrows.Count > 0)
        {
            Arrows.Clear();   
        }
    }

    public Player PlayerWhite
    {
        get => _playerWhite;
        set => SetField(ref _playerWhite, value);
    }

    public Player PlayerBlack
    {
        get => _playerBlack;
        set => SetField(ref _playerBlack, value);
    }

    public double BoardSize
    {
        get => _boardSize;
        set => SetField(ref _boardSize, value);
    }

    public double TileSize
    {
        get => _tileSize;
        set => SetField(ref _tileSize, value);
    }

    public double BoardFontSize
    {
        get => _boardFontSize;
        set => SetField(ref _boardFontSize, value);
    }

    public double ArrowThickness
    {
        get => _arrowThickness;
        set => SetField(ref _arrowThickness, value);
    }

    public string GameStatus
    {
        get => _gameStatus;
        set => SetField(ref _gameStatus, value);
    }

    public PieceColor CurrentTurn
    {
        get => _currentTurn;
        set => SetField(ref _currentTurn, value);
    }

    public string ActionPointerId
    {
        get => _actionPointerId;
        set => SetField(ref _actionPointerId,  value);
    }

    public double NumberMinWidth
    {
        get => _numberMinWidth;
        set => SetField(ref _numberMinWidth, value);
    }

    public double PromotionCardWidth
    {
        get => _promotionCardWidth;
        set => SetField(ref _promotionCardWidth, value);
    }

    public double PromotionCardHeight
    {
        get => _promotionCardHeight;
        set => SetField(ref _promotionCardHeight, value);
    }

    public Thickness PromotionCardContentMargin
    {
        get => _promotionCardContentMargin;
        set => SetField(ref _promotionCardContentMargin, value);
    }

    public Thickness PromotionCardContentMarginEnd
    {
        get => _promotionCardContentMarginEnd;
        set => SetField(ref _promotionCardContentMarginEnd, value);
    }

    public Visibility PromotionCardVisibility
    {
        get => _promotionCardVisibility;
        set => SetField(ref _promotionCardVisibility, value);
    }

    public Visibility MovableTileVisibility
    {
        get => _movableTileVisibility;
        set => SetField(ref _movableTileVisibility, value);
    }

    public Visibility ArrowPreviewVisibility
    {
        get => _arrowPreviewVisibility;
        set => SetField(ref _arrowPreviewVisibility, value);
    }
    
    public Arrow ArrowPreview
    {
        get => _arrowPreview;
        set => SetField(ref _arrowPreview, value);
    }

    #region IPropertyChanged Implementation

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