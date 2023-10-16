using Project11.Chess.Boards;
using Project11.Chess.Extension;
using Project11.Chess.Moves;
using Project11.Chess.Moves.Action;
using Project11.Chess.Pieces;

namespace Project11.Chess;

public class ChessGame
{
    private readonly Dictionary<PieceColor, List<Piece>> _pieces;
    private readonly Dictionary<PieceColor, List<ChessAction>> _actions;
    private readonly Dictionary<string, int> _snapshots;
    private readonly Board _board;
    private int _fiftyRuleCount;

    /// <summary>
    /// Get the current status of the game.
    /// </summary>
    public GameStatus GameStatus { get; private set; }

    /// <summary>
    /// Get the current <see cref="PieceColor"/> turn.
    /// </summary>
    public PieceColor CurrentTurn { get; private set; }

    /// <summary>
    /// Get or set the last successfully executed <see cref="ChessAction"/>. This instance doesn't
    /// necessarily be equal to the last <see cref="ChessAction"/> in the game's history. For example,
    /// when the game is rolled back to a certain point in the game's history, this instance will be
    /// different with the last <see cref="ChessAction"/> of the game's history.
    /// However, the <see cref="ChessAction.Id"/> of this instance will always be equal
    /// to <see cref="ActionPointerId"/>.
    /// </summary>
    public ChessAction? LastAction { get; private set; }

    /// <summary>
    /// Get or set the <see cref="ChessAction.Id"/> of the <see cref="ChessAction"/> the game is
    /// currently pointing to. Normally the value of this instance should be equal to <see cref="ActionCount"/> - 1,
    /// except when the user call <see cref="GoToAction"/> and the state of the game rolled back to some point
    /// in the game's history.
    /// </summary>
    public int ActionPointerId { get; private set; }

    /// <summary>
    /// Get the number of <see cref="ChessAction"/> stored in the game's history.
    /// </summary>
    public int ActionCount => _actions[PieceColor.White].Count + _actions[PieceColor.Black].Count;

    /// <summary>
    /// Get the <see cref="Board.Width"/> of the <see cref="Board"/> owned by this instance.
    /// </summary>
    public int BoardWidth => _board.Width;

    /// <summary>
    /// Get the <see cref="Board.Height"/> of the <see cref="Board"/> owned by this instance.
    /// </summary>
    public int BoardHeight => _board.Height;

    /// <summary>
    /// Occurs when a <see cref="Piece"/> has moved from one <see cref="Position"/> to another.
    /// </summary>
    public event Action<Piece, Move, bool>? PieceMoved;

    /// <summary>
    /// Occurs when a <see cref="King"/> has checked by another <see cref="Piece"/>.
    /// </summary>
    public event Action<King, List<Piece>>? Checked;

    /// <summary>
    /// Occurs when the game ends, either it was caused by a checkmate or a draw.
    /// </summary>
    public event Action<GameResult>? GameOver;

    /// <summary>
    /// Occurs when a <see cref="Pawn"/> reaches the far most opposite side of the board.
    /// For <see cref="StandardBoard"/>, the <see cref="Position.Y"/> value will be equal to 7
    /// for <see cref="PieceColor.White"/> and 0 for <see cref="PieceColor.Black"/>.
    /// The <see cref="Pawn"/> doesn't automatically promoted by the game, the user should
    /// provide a way for the player to select a <see cref="PieceType"/> and then
    /// call <see cref="PromotePawn"/> to promote the <see cref="Pawn"/> manually.
    /// </summary>
    public event Action<Pawn>? PawnPromotion;

    /// <summary>
    /// Occurs when a <see cref="Piece"/> has been updated. This even is called after a <see cref="Pawn"/>
    /// got promoted.
    /// </summary>
    public event Action<Piece>? PieceUpdated;

    /// <summary>
    /// Occurs when the <see cref="CurrentTurn"/> value has been changed.
    /// </summary>
    public event Action<PieceColor>? TurnChanged;

    /// <summary>
    /// Occurs when a <see cref="ChessAction"/> has been added to the game's history.
    /// This event is called during a successful execution of <see cref="MovePiece"/>.
    /// </summary>
    public event Action<PieceColor, ChessAction>? ActionAdded;

    /// <summary>
    /// Occurs when a <see cref="ChessAction"/> in the game's history has been updated.
    /// This event is called when there is a situation which changed the state of a <see cref="ChessAction"/>.
    /// For example, when a <see cref="Pawn"/> got promoted.
    /// </summary>
    public event Action<PieceColor, ChessAction>? ActionUpdated;

    /// <summary>
    /// Occurs when a <see cref="ChessAction"/> has been removed from the game's history.
    /// This event is called when the game state was rolled back to a certain point in the game's history
    /// and the user called <see cref="MovePiece"/>.
    /// </summary>
    public event Action<PieceColor, ChessAction>? ActionRemoved;

    /// <summary>
    /// Occurs when the <see cref="ActionPointerId"/> value has been changed.
    /// </summary>
    public event Action<int>? ActionPointerChanged;

    public ChessGame(Board board)
    {
        _board = board;
        GameStatus = GameStatus.Ongoing;
        CurrentTurn = PieceColor.White;
        ActionPointerId = -1;

        _pieces = new Dictionary<PieceColor, List<Piece>>
        {
            { PieceColor.White, GeneratePieceSet(PieceColor.White) },
            { PieceColor.Black, GeneratePieceSet(PieceColor.Black) }
        };

        _actions = new Dictionary<PieceColor, List<ChessAction>>
        {
            { PieceColor.White, new List<ChessAction>() },
            { PieceColor.Black, new List<ChessAction>() }
        };

        _snapshots = new Dictionary<string, int>();
        _fiftyRuleCount = 0;
    }

    #region Public Methods

    /// <summary>
    /// Searches a <see cref="Piece"/> with the given <see cref="Piece.Id"/>.
    /// </summary>
    /// <param name="pieceId">The <see cref="Piece.Id"/> of the <see cref="Piece"/>.</param>
    /// <returns>
    /// An instance of <see cref="Piece"/> or null if a <see cref="Piece"/> with the given <see cref="Piece.Id"/>
    /// was not found.
    /// </returns>
    public Piece? FindPieceById(int pieceId)
    {
        return pieceId < 16
            ? _pieces[PieceColor.White].Find(piece => piece.Id == pieceId)
            : _pieces[PieceColor.Black].Find(piece => piece.Id == pieceId);
    }

    /// <summary>
    /// Searches a <see cref="Piece"/> with the given <see cref="Position"/>.
    /// </summary>
    /// <param name="position">The <see cref="Position"/> of the <see cref="Piece"/>.</param>
    /// <returns>
    /// An instance of <see cref="Piece"/> or null if a <see cref="Piece"/> with the given <see cref="Position"/>
    /// was not found.
    /// </returns>
    public Piece? FindPieceByPosition(Position position)
    {
        if (!ContainsTile(position))
        {
            return null;
        }

        if (CurrentTurn.IsWhite())
        {
            return _pieces[PieceColor.White].Find(piece => piece.Position == position) ??
                   _pieces[PieceColor.Black].Find(piece => piece.Position == position);
        }

        return _pieces[PieceColor.Black].Find(piece => piece.Position == position) ??
               _pieces[PieceColor.White].Find(piece => piece.Position == position);
    }

    /// <summary>
    /// Searches a list of <see cref="Piece"/> who can attack tile occupied by another <see cref="Piece"/>
    /// with the given <see cref="PieceColor"/> at the given <see cref="Position"/>.
    /// </summary>
    /// <param name="position">The target <see cref="Position"/>.</param>
    /// <param name="attackerColor">The target <see cref="PieceColor"/></param>
    /// <returns>
    /// A <see cref="List{Piece}"/> of <see cref="Piece"/> which can attack another <see cref="Piece"/>
    /// with the given <see cref="PieceColor"/> at the given <see cref="Position"/>.
    /// </returns>
    public List<Piece> FindPiecesWhoCanAttack(Position position, PieceColor attackerColor)
    {
        if (!ContainsTile(position))
        {
            return new List<Piece>();
        }

        return _pieces[attackerColor]
            .Where(piece => piece.Color == attackerColor && piece.CanAttackTile(this, position))
            .ToList();
    }

    /// <summary>
    /// Searches a <see cref="ChessAction"/> with the given <see cref="ChessAction.Id"/>.
    /// </summary>
    /// <param name="actionId">The <see cref="ChessAction.Id"/> of the <see cref="ChessAction"/>.</param>
    /// <returns>
    /// An instance of a <see cref="ChessAction"/> or null if a <see cref="ChessAction"/>
    /// with the given <see cref="ChessAction.Id"/> was not found.
    /// </returns>
    public ChessAction? FindActionById(int actionId)
    {
        return _actions[actionId % 2 == 0 ? PieceColor.White : PieceColor.Black]
                   .Find(action => action.Id == actionId)
               ?? _actions[actionId % 2 == 0 ? PieceColor.Black : PieceColor.White]
                   .Find(action => action.Id == actionId);
    }

    /// <summary>
    /// Move a <see cref="Piece"/> based on a <see cref="ChessAction"/>.
    /// if the game state was rolled back to a certain point in the game's history,
    /// the game state after that point will be deleted and a new state generated by this method
    /// will be inserted right after that point.
    /// </summary>
    /// <param name="chessAction">An instance of <see cref="ChessAction"/></param>
    /// <returns>
    /// A <see cref="bool"/> instance that indicates whether the given <see cref="ChessAction"/>
    /// was executed successfully.
    /// </returns>
    public bool MovePiece(ChessAction chessAction)
    {
        var pieces = chessAction.GetMoves()
            .Select(move => new { Index = move.PieceId, Value = FindPieceById(move.PieceId) })
            .ToDictionary(item => item.Index, item => item.Value);

        if (pieces.Any(piece => piece.Value is null))
        {
            return false;
        }

        if (ActionPointerId < ActionCount - 1)
        {
            var actionCount = ActionCount;

            for (var i = ActionPointerId + 1; i < actionCount; i++)
            {
                var color = i % 2 == 0 ? PieceColor.White : PieceColor.Black;
                var action = FindActionById(i);

                if (action == null)
                {
                    continue;
                }

                _actions[color].Remove(action);
                ActionRemoved?.Invoke(color, action);
            }
        }

        foreach (var move in chessAction.GetMoves())
        {
            var piece = pieces[move.PieceId]!;

            piece.Position = move.To;
            piece.IncrementMoveCount();
            PieceMoved?.Invoke(piece, move, false);

            if (!ContainsTile(move.To) || piece is not Pawn pawn || !chessAction.IsPromotion)
            {
                continue;
            }

            PawnPromotion?.Invoke(pawn);
        }

        var currentTurn = CurrentTurn;
        var nextTurn = CurrentTurn.Inverse();

        LastAction = chessAction;
        _actions[CurrentTurn].Add(chessAction);
        CurrentTurn = nextTurn;
        ActionPointerId = chessAction.Id;
        ActionPointerChanged?.Invoke(ActionPointerId);

        var snapshot = GetSnapshot();

        if (!_snapshots.TryAdd(snapshot, 1))
        {
            _snapshots[snapshot] += 1;
        }

        if (_snapshots[snapshot] >= 3)
        {
            ActionAdded?.Invoke(currentTurn, chessAction);
            GameStatus = GameStatus.GameOver;
            GameOver?.Invoke(GameResult.DrawByThreefoldRepetition);
            return true;
        }

        if (currentTurn == PieceColor.Black)
        {
            if (chessAction is Capture || FindPieceById(chessAction.GetPrimaryMove().PieceId) is Pawn)
            {
                _fiftyRuleCount = 0;
            }
            else
            {
                _fiftyRuleCount++;
            }

            if (_fiftyRuleCount >= 50)
            {
                ActionAdded?.Invoke(currentTurn, chessAction);
                GameStatus = GameStatus.GameOver;
                GameOver?.Invoke(GameResult.DrawByFiftyMoveRule);
                return true;
            }
        }

        var nextTurnKing = (_pieces[nextTurn].Find(piece => piece is King) as King)!;
        var attackers = FindPiecesWhoCanAttack(nextTurnKing.Position, currentTurn);
        var piecesLeft = _pieces[nextTurn].Where(piece => ContainsTile(piece.Position)).ToList();

        if (attackers.Count == 0)
        {
            if (piecesLeft.Any(piece => GetPossibleActions(piece.Position).Count != 0))
            {
                ActionAdded?.Invoke(currentTurn, chessAction);
                TurnChanged?.Invoke(CurrentTurn);
                return true;
            }

            ActionAdded?.Invoke(currentTurn, chessAction);
            GameStatus = GameStatus.GameOver;
            GameOver?.Invoke(GameResult.DrawByStalemate);
            return true;
        }

        chessAction.IsCheck = true;
        Checked?.Invoke(nextTurnKing, attackers);

        if (piecesLeft.Any(piece => GetPossibleActions(piece.Position).Count != 0))
        {
            ActionAdded?.Invoke(currentTurn, chessAction);
            TurnChanged?.Invoke(CurrentTurn);
            return true;
        }

        chessAction.IsCheckmate = true;
        ActionAdded?.Invoke(currentTurn, chessAction);
        GameStatus = GameStatus.GameOver;
        GameOver?.Invoke(currentTurn.IsWhite() ? GameResult.WhiteWin : GameResult.BlackWin);

        return true;
    }

    /// <summary>
    /// Promote a <see cref="Pawn"/> to another type of <see cref="Piece"/>. A <see cref="Pawn"/> cannot be
    /// promoted to <see cref="Pawn"/> or <see cref="King"/>.
    /// </summary>
    /// <param name="pieceId">The <see cref="Piece.Id"/> of the <see cref="Pawn"/>.</param>
    /// <param name="type">
    /// An instance of <see cref="PieceType"/> that determines the type of <see cref="Piece"/> after promotion.
    /// </param>
    /// <returns>True if the <see cref="Pawn"/> was successfully promoted, False otherwise.</returns>
    public bool PromotePawn(int pieceId, PieceType type)
    {
        if (type is PieceType.King or PieceType.Pawn)
        {
            return false;
        }

        var piece = FindPieceById(pieceId);

        if (piece is not Pawn)
        {
            return false;
        }

        var index = _pieces[piece.Color].FindIndex(p => p.Id == pieceId);

        if (index == -1)
        {
            return false;
        }

        var newPiece = CreatePiece(type, piece.Id, piece.Color, piece.Position);
        _pieces[piece.Color][index] = newPiece;
        PieceUpdated?.Invoke(newPiece);

        var snapshot = GetSnapshot();

        if (!_snapshots.TryAdd(snapshot, 1))
        {
            _snapshots[snapshot] += 1;
        }

        if (LastAction is null)
        {
            return true;
        }

        LastAction.PromotionPieceType = type;

        if (LastAction.IsPromotion is false)
        {
            return true;
        }

        var kingCandidate = _pieces[newPiece.Color.Inverse()].Find(p => p is King);

        if (kingCandidate is not King king)
        {
            ActionUpdated?.Invoke(newPiece.Color, LastAction);
            return true;
        }

        var positions = newPiece.GetPossibleActions(this);

        if (!positions.Exists(pos => pos.GetPrimaryMove().To == kingCandidate.Position))
        {
            ActionUpdated?.Invoke(newPiece.Color, LastAction);
            return true;
        }

        var piecesLeft = _pieces[newPiece.Color.Inverse()].Where(p => ContainsTile(p.Position)).ToList();

        LastAction.IsCheck = true;
        Checked?.Invoke(king, new List<Piece> { newPiece });

        if (piecesLeft.Any(p => GetPossibleActions(p.Position).Count != 0))
        {
            ActionUpdated?.Invoke(newPiece.Color, LastAction);
            return true;
        }

        LastAction.IsCheckmate = true;
        ActionUpdated?.Invoke(newPiece.Color, LastAction);
        GameStatus = GameStatus.GameOver;
        GameOver?.Invoke(newPiece.Color.IsWhite() ? GameResult.WhiteWin : GameResult.BlackWin);

        return true;
    }

    /// <summary>
    /// Get the possible <see cref="ChessAction"/> a <see cref="Piece"/> at the given <see cref="Position"/>
    /// can do. If the tile at the given <see cref="Position"/> is unoccupied, this method will return an
    /// empty <see cref="List{ChessAction}"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> to check.</param>
    /// <returns>
    /// A <see cref="List{ChessAction}"/> of possible <see cref="ChessAction"/>
    /// a <see cref="Piece"/> at the given <see cref="Position"/> can do.
    /// </returns>
    public List<ChessAction> GetPossibleActions(Position position)
    {
        if (GameStatus == GameStatus.GameOver)
        {
            return new List<ChessAction>();
        }

        var piece = FindPieceByPosition(position);

        if (piece is null || piece.Color != CurrentTurn)
        {
            return new List<ChessAction>();
        }

        var actions = piece.GetPossibleActions(this);

        if (piece is not King && IsPieceDefendingKing(piece, out _, out var attackerPath))
        {
            actions = actions.Where(action => attackerPath?.Exists(pos => action.GetPrimaryMove().To == pos) == true)
                .ToList();
        }

        if (!IsInCheck(CurrentTurn))
        {
            return actions;
        }

        var king = _pieces[CurrentTurn].Find(p => p is King)!;
        var attackers = FindPiecesWhoCanAttack(king.Position, CurrentTurn.Inverse());

        if (piece is King)
        {
            return actions.Where(action =>
                !attackers.Any(attacker =>
                    attacker
                        .GetPathTo(action.GetPrimaryMove().To)
                        .Any(pos => pos == action.GetPrimaryMove().To))
            ).ToList();
        }

        return attackers
            .Select(attacker =>
                attacker.GetPathTo(king.Position).Where(pos => pos != king.Position).ToList())
            .Aggregate(actions, (current, path) =>
                current.Where(action => path.Exists(pos => pos == action.GetPrimaryMove().To)).ToList());
    }

    /// <summary>
    /// Go to certain <see cref="ChessAction"/> with the given <see cref="ChessAction.Id"/>. This method
    /// allows for rolling back or rolling forward to a specific point in the game's history.
    /// </summary>
    /// <param name="actionId">The <see cref="ChessAction.Id"/> of the <see cref="ChessAction"/></param>
    /// <returns>A <see cref="bool"/> value that indicates whether the operation was successful.</returns>
    public bool GoToAction(int actionId)
    {
        if (actionId < -1 || actionId >= ActionCount || LastAction?.Id is not { } lastActionId ||
            lastActionId == actionId)
        {
            return false;
        }

        var chessAction = FindActionById(Math.Max(0, actionId));
        Piece? piece;
        string snapshot;

        if (chessAction is null)
        {
            return false;
        }

        var isMovingBack = actionId < lastActionId;
        var incrementor = isMovingBack ? -1 : 1;

        for (var i = lastActionId;
             isMovingBack ? i >= Math.Max(0, actionId) : i < Math.Max(0, actionId);
             i += incrementor)
        {
            var action = FindActionById(i)!;
            piece = FindPieceById(action.GetPrimaryMove().PieceId)!;

            if (isMovingBack)
            {
                snapshot = GetSnapshot();
                _snapshots[snapshot] -= 1; 
            }

            if (piece.Color == PieceColor.Black)
            {
                if (action is Capture || FindPieceById(action.GetPrimaryMove().PieceId) is Pawn)
                {
                    _fiftyRuleCount = 0;
                }
                else
                {
                    _fiftyRuleCount += isMovingBack ? -1 : 1;
                }
            }

            var moves = action.GetMoves().ToList();

            if (isMovingBack)
            {
                moves.Reverse();
            }

            foreach (var move in moves)
            {
                piece = FindPieceById(move.PieceId)!;
                piece.Position = isMovingBack ? move.From : move.To;

                if (isMovingBack)
                {
                    piece.DecrementMoveCount();
                }
                else
                {
                    piece.IncrementMoveCount();
                }

                PieceMoved?.Invoke(
                    piece,
                    new Move(piece.Id, isMovingBack ? move.To : move.From, isMovingBack ? move.From : move.To),
                    true
                );
            }

            if (!isMovingBack)
            {
                snapshot = GetSnapshot();
                _snapshots[snapshot] += 1;
            }

            if (action is not { IsPromotion: true, PromotionPieceType: { } type })
            {
                continue;
            }

            var index = _pieces[piece.Color].FindIndex(p => p.Id == piece.Id);

            _pieces[piece.Color][index] = isMovingBack
                ? CreatePiece(PieceType.Pawn, piece.Id, piece.Color, piece.Position, piece.MoveCount)
                : CreatePiece(type, piece.Id, piece.Color, piece.Position, piece.MoveCount);

            PieceUpdated?.Invoke(_pieces[piece.Color][index]);
        }

        if (GameStatus == GameStatus.GameOver && isMovingBack)
        {
            GameStatus = GameStatus.Ongoing;
        }

        if (actionId == -1)
        {
            CurrentTurn = PieceColor.White;
            TurnChanged?.Invoke(CurrentTurn);
            ActionPointerId = -1;
            ActionPointerChanged?.Invoke(ActionPointerId);
            return true;
        }

        var pieces = chessAction.GetMoves()
            .Select(move => new { Index = move.PieceId, Value = FindPieceById(move.PieceId) })
            .ToDictionary(item => item.Index, item => item.Value);

        foreach (var move in chessAction.GetMoves())
        {
            piece = pieces[move.PieceId]!;
            piece.Position = move.To;
            piece.IncrementMoveCount();
            PieceMoved?.Invoke(piece, move, false);
        }

        piece = FindPieceById(chessAction.GetPrimaryMove().PieceId)!;

        if (chessAction is { IsPromotion: true, PromotionPieceType: { } pieceType })
        {
            var index = _pieces[piece.Color].FindIndex(p => p.Id == piece.Id);

            _pieces[piece.Color][index] =
                CreatePiece(pieceType, piece.Id, piece.Color, piece.Position, piece.MoveCount);

            PieceUpdated?.Invoke(_pieces[piece.Color][index]);
        }

        var currentTurn = piece.Color;
        var nextTurn = piece.Color.Inverse();

        LastAction = chessAction;
        CurrentTurn = nextTurn;
        ActionPointerId = chessAction.Id;
        ActionPointerChanged?.Invoke(ActionPointerId);

        snapshot = GetSnapshot();
        _snapshots[snapshot] += 1;

        if (_snapshots[snapshot] >= 3)
        {
            GameStatus = GameStatus.GameOver;
            GameOver?.Invoke(GameResult.DrawByThreefoldRepetition);
            return true;
        }

        if (currentTurn == PieceColor.Black)
        {
            if (chessAction is Capture || FindPieceById(chessAction.GetPrimaryMove().PieceId) is Pawn)
            {
                _fiftyRuleCount = 0;
            }
            else
            {
                _fiftyRuleCount++;
            }

            if (_fiftyRuleCount >= 50)
            {
                GameStatus = GameStatus.GameOver;
                GameOver?.Invoke(GameResult.DrawByFiftyMoveRule);
                return true;
            }
        }

        var nextTurnKing = (_pieces[nextTurn].Find(p => p is King) as King)!;
        var attackers = FindPiecesWhoCanAttack(nextTurnKing.Position, currentTurn);
        var piecesLeft = _pieces[nextTurn].Where(p => ContainsTile(p.Position)).ToList();

        if (attackers.Count == 0)
        {
            if (piecesLeft.Any(p => GetPossibleActions(p.Position).Count != 0))
            {
                TurnChanged?.Invoke(CurrentTurn);
                return true;
            }

            GameStatus = GameStatus.GameOver;
            GameOver?.Invoke(GameResult.DrawByStalemate);
            return true;
        }

        if (chessAction.IsCheck)
        {
            Checked?.Invoke(nextTurnKing, attackers);
        }

        if (!chessAction.IsCheckmate)
        {
            TurnChanged?.Invoke(CurrentTurn);
            return true;
        }

        GameStatus = GameStatus.GameOver;
        GameOver?.Invoke(currentTurn.IsWhite() ? GameResult.WhiteWin : GameResult.BlackWin);

        return true;
    }

    /// <summary>
    /// Get a symbol that represents a <see cref="ChessAction"/> instance. The symbol contains
    /// the type and <see cref="Position"/> of the <see cref="Piece"/> followed by a '+' if it
    /// <see cref="ChessAction.IsCheck"/> or a '#' if it <see cref="ChessAction.IsCheckmate"/>.
    /// The symbol can also contains 'x' if it is a <see cref="Capture"/>,
    /// '=' if it is a <see cref="Pawn"/> promotion, and an 'e.p.' if it is an <see cref="EnPassant"/>.
    /// The <see cref="CastlingShort"/> and <see cref="CastlingLong"/> symbol is 'O-O' and 'O-O-O'
    /// respectively.
    /// </summary>
    /// <param name="action">An instance of <see cref="ChessAction"/>.</param>
    /// <returns>
    /// A <see cref="string"/> value that represent the symbol of the given <see cref="ChessAction"/>.
    /// </returns>
    public string GetActionNotation(ChessAction action)
    {
        var primaryMove = action.GetPrimaryMove();
        var piece = FindPieceById(primaryMove.PieceId);
        var checkNotation = action.IsCheck ? action.IsCheckmate ? "#" : "+" : "";

        if (piece is null)
        {
            return "";
        }

        return action switch
        {
            BasicMove or DoubleStep => action.IsPromotion
                ? $"{GetTileNotation(primaryMove.To)}={piece.Notation}{checkNotation}"
                : $"{piece.Notation}{GetTileNotation(primaryMove.To)}{checkNotation}",
            Capture => action.IsPromotion
                ? $"{GetTileNotationX(primaryMove.From)}x{GetTileNotation(primaryMove.To)}={piece.Notation}{checkNotation}"
                : piece is Pawn
                    ? $"{GetTileNotationX(primaryMove.From)}x{GetTileNotation(primaryMove.To)}{checkNotation}"
                    : $"{piece.Notation}x{GetTileNotation(primaryMove.To)}{checkNotation}",
            EnPassant => $"{GetTileNotationX(primaryMove.From)}x{GetTileNotation(primaryMove.To)} e.p.{checkNotation}",
            CastlingShort => "O-O",
            CastlingLong => "O-O-O",
            _ => ""
        };
    }

    /// <summary>
    /// Get a snapshot that represent a list of <see cref="Piece"/> and its current <see cref="Position"/> on the <see cref="Board"/>.
    /// The snapshot is constructed using the <see cref="Piece"/> and it's <see cref="Position"/> symbol and is
    /// sorted by the <see cref="Position.Y"/> and <see cref="Position.X"/> value of it's <see cref="Position"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> value that represent the type and location of <see cref="Piece"/> on the <see cref="Board"/>.
    /// </returns>
    public string GetSnapshot()
    {
        var notations = _pieces[PieceColor.White]
            .Concat(_pieces[PieceColor.Black])
            .Where(piece => ContainsTile(piece.Position))
            .OrderBy(piece => piece.Y)
            .ThenBy(piece => piece.X)
            .Select(piece => $"{piece.Notation}{GetTileNotation(piece.Position)}");

        return string.Join("", notations);
    }

    /// <summary>
    /// Check whether the <see cref="King"/> of the given <see cref="PieceColor"/> is currently under attack.
    /// </summary>
    /// <param name="color">An instance of <see cref="PieceColor"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a <see cref="King"/> of the given <see cref="PieceColor"/>
    /// is currently under attack.
    /// </returns>
    public bool IsInCheck(PieceColor color)
    {
        return IsTileUnderAttack(_pieces[color].Find(piece => piece is King)!.Position, color.Inverse());
    }

    /// <summary>
    /// Check whether a <see cref="Piece"/> is currently defending a <see cref="King"/> of the same
    /// <see cref="PieceColor"/> from an attack.
    /// </summary>
    /// <param name="piece">An instance of <see cref="Piece"/> to check.</param>
    /// <param name="attacker">
    /// An instance of <see cref="Piece"/>. If the given <see cref="Piece"/> to check is not currently defending
    /// a <see cref="King"/>, this value will be set to null.
    /// </param>
    /// <param name="attackerPath">
    /// A <see cref="List{Position}"/> of <see cref="Position"/>. If the given <see cref="Piece"/> to check
    /// is not currently defending a <see cref="King"/>, this value will be set to null.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a <see cref="Piece"/> is currently defending
    /// a <see cref="King"/> of the same <see cref="PieceColor"/>.
    /// </returns>
    public bool IsPieceDefendingKing(Piece piece, out Piece? attacker, out List<Position>? attackerPath)
    {
        if (!ContainsTile(piece.Position))
        {
            attacker = null;
            attackerPath = null;
            return false;
        }

        var king = _pieces[piece.Color].Find(p => p is King)!;

        var pieceX = piece.X;
        var pieceY = piece.Y;

        var kingX = king.X;
        var kingY = king.Y;

        var diffX = pieceX - kingX;
        var diffY = pieceY - kingY;

        List<Piece> attackerCandidates;

        if (diffX == 0)
        {
            if (_pieces[piece.Color].Any(p => p != piece && p != king && p.X == pieceX && p.Y.In(pieceY, kingY)))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            attackerCandidates = _pieces[piece.Color.Inverse()]
                .Where(p => p.X == pieceX && p.Y.In(pieceY, diffY > 0 ? BoardHeight - 1 : 0))
                .ToList();

            attackerCandidates.Sort((p1, p2) => diffY > 0 ? p1.Y - p2.Y : p2.Y - p1.Y);

            if (attackerCandidates.Count == 0 || attackerCandidates[0] is not (Rook or Queen))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            if (_pieces[piece.Color].Any(p => p != piece && p.X == pieceX && p.Y.In(pieceY, attackerCandidates[0].Y)))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            attacker = attackerCandidates[0];
            attackerPath = attackerCandidates[0].GetPathTo(king.Position).Where(pos => pos != piece.Position).ToList();
            return true;
        }

        if (diffY == 0)
        {
            if (_pieces[piece.Color].Any(p => p != piece && p != king && p.Y == pieceY && p.X.In(pieceX, kingX)))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            attackerCandidates = _pieces[piece.Color.Inverse()]
                .Where(p => p.Y == pieceY && p.X.In(pieceX, diffX > 0 ? BoardWidth - 1 : 0))
                .ToList();

            attackerCandidates.Sort((p1, p2) => diffX > 0 ? p1.X - p2.X : p2.X - p1.X);

            if (attackerCandidates.Count == 0 || attackerCandidates[0] is not (Rook or Queen))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            if (_pieces[piece.Color].Any(p => p != piece && p.Y == pieceY && p.X.In(pieceX, attackerCandidates[0].X)))
            {
                attacker = null;
                attackerPath = null;
                return false;
            }

            attacker = attackerCandidates[0];
            attackerPath = attackerCandidates[0].GetPathTo(king.Position).Where(pos => pos != piece.Position).ToList();
            return true;
        }

        if (diffX.Abs() != diffY.Abs())
        {
            attacker = null;
            attackerPath = null;
            return false;
        }

        if (_pieces[piece.Color]
            .Any(p =>
                p != king &&
                p != piece &&
                (p.X - pieceX).Abs() == (p.Y - pieceY).Abs() &&
                p.X.In(pieceX, kingX) &&
                p.Y.In(pieceY, kingY)
            ))
        {
            attacker = null;
            attackerPath = null;
            return false;
        }

        attackerCandidates = _pieces[piece.Color.Inverse()]
            .Where(p =>
                (p.X - pieceX).Abs() == (p.Y - pieceY).Abs() &&
                p.X.In(pieceX, diffX > 0 ? BoardWidth - 1 : 0) &&
                p.Y.In(pieceY, diffY > 0 ? BoardHeight - 1 : 0))
            .ToList();

        attackerCandidates.Sort((p1, p2) =>
            p1.X == p2.X
                ? diffY > 0
                    ? p1.Y - p2.Y
                    : p2.Y - p1.Y
                : diffX > 0
                    ? p1.X - p2.X
                    : p2.X - p1.X
        );

        if (attackerCandidates.Count == 0 || attackerCandidates[0] is not (Bishop or Queen))
        {
            attacker = null;
            attackerPath = null;
            return false;
        }

        if (_pieces[piece.Color].Any(p =>
                p != piece &&
                (p.X - pieceX).Abs() == (p.Y - pieceY).Abs() &&
                p.X.In(pieceX, attackerCandidates[0].X) &&
                p.Y.In(pieceY, attackerCandidates[0].Y)
            ))
        {
            attacker = null;
            attackerPath = null;
            return false;
        }

        attacker = attackerCandidates[0];
        attackerPath = attackerCandidates[0].GetPathTo(king.Position).Where(pos => pos != piece.Position).ToList();
        return true;
    }

    #endregion

    #region Public Tile Utility Methods

    /// <summary>
    /// Check whether a <see cref="Position"/> is located on the <see cref="Board"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a position is located on the <see cref="Board"/>.
    /// </returns>
    public bool ContainsTile(Position position)
    {
        return _board.Contains(position);
    }

    /// <summary>
    /// Get a symbol that represent the <see cref="Position.X"/> and <see cref="Position.Y"/> value
    /// of the given <see cref="Position"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/>.</param>
    /// <returns>
    /// A <see cref="string"/> value that represent the <see cref="Position.X"/> and <see cref="Position.Y"/>
    /// symbol of the given <see cref="Position"/>.
    /// </returns>
    public string GetTileNotation(Position position)
    {
        return _board.GetNotation(position);
    }

    /// <summary>
    /// Get a symbol that represent the <see cref="Position.X"/> value of the given <see cref="Position"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/>.</param>
    /// <returns>
    /// A <see cref="string"/> value that represent the <see cref="Position.X"/>
    /// symbol of the given <see cref="Position"/>.
    /// </returns>
    public string GetTileNotationX(Position position)
    {
        return _board.GetNotationX(position.X);
    }

    /// <summary>
    /// Get a symbol that represent the <see cref="Position.Y"/> value of the given <see cref="Position"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/>.</param>
    /// <returns>
    /// A <see cref="string"/> value that represent the <see cref="Position.Y"/>
    /// symbol of the given <see cref="Position"/>.
    /// </returns>
    public string GetTileNotationY(Position position)
    {
        return _board.GetNotationY(position.Y);
    }

    /// <summary>
    /// Check whether a tile at the given <see cref="Position"/> is currently under attack
    /// by a <see cref="Piece"/> of the given <see cref="PieceColor"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> to check.</param>
    /// <param name="attackerColor">An instance of <see cref="PieceColor"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a tile at the given <see cref="Position"/>
    /// is currently under attack by a <see cref="Piece"/> of the given <see cref="PieceColor"/>.
    /// </returns>
    public bool IsTileUnderAttack(Position position, PieceColor attackerColor)
    {
        return ContainsTile(position) &&
               _pieces[attackerColor].Any(piece => piece.Position != position && piece.CanAttackTile(this, position));
    }

    /// <summary>
    /// Check whether a tile at the given <see cref="Position"/> is occupied by a <see cref="Piece"/> of any
    /// <see cref="PieceColor"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a tile at the given <see cref="Position"/>
    /// is occupied by a <see cref="Piece"/> of any <see cref="PieceColor"/>.
    /// </returns>
    public bool IsTileOccupied(Position position)
    {
        if (!ContainsTile(position))
        {
            return false;
        }

        return _pieces[PieceColor.White].Exists(piece => piece.Position == position) ||
               _pieces[PieceColor.Black].Exists(piece => piece.Position == position);
    }

    /// <summary>
    /// Check whether a tile at the given <see cref="Position"/> is occupied by a <see cref="Piece"/> of the given
    /// <see cref="PieceColor"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> to check.</param>
    /// <param name="color">An instance of <see cref="PieceColor"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value that determines whether a tile at the given <see cref="Position"/>
    /// is occupied by a <see cref="Piece"/> of the given <see cref="PieceColor"/>.
    /// </returns>
    public bool IsTileOccupiedBy(Position position, PieceColor color)
    {
        return ContainsTile(position) && _pieces[color].Exists(piece => piece.Position == position);
    }

    /// <summary>
    /// Get a list of possible positions which is orthogonally or diagonally adjacent to the given <see cref="Position"/>.
    /// The positions returned by this method are guaranteed to be located
    /// on the <see cref="Board"/>. This method doesn't check whether the position is occupied by a <see cref="Piece"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> as the starting point.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of possible <see cref="Position"/> which is orthogonally or diagonally
    /// adjacent to the given <see cref="Position"/>.
    /// </returns>
    public List<Position> GetSurroundingTiles(Position position)
    {
        var results = new List<Position>();

        for (var x = position.X - 1; x <= position.X + 1; x++)
        {
            for (var y = position.Y - 1; y <= position.Y + 1; y++)
            {
                var pos = new Position(x, y);

                if (pos != position && ContainsTile(pos))
                {
                    results.Add(new Position(x, y));
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Get a list of possible positions in the orthogonal direction starting from
    /// the given <see cref="Position"/>. This method will check for empty tiles diagonally
    /// starting from the given location until it reaches the end of the <see cref="Board"/>
    /// or it encounter a tile which is occupied by a <see cref="Piece"/>. The positions returned by this method
    /// are guaranteed to be in order, starting from the given <see cref="Position"/> to the last possible position
    /// a <see cref="Queen"/> or <see cref="Rook"/> can go.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> as the starting point.</param>
    /// <param name="direction">
    /// An instance of <see cref="Direction"/> that determines the direction of the search.
    /// </param>
    /// <returns>
    /// A <see cref="List{Position}"/> of possible <see cref="Position"/> in the orthogonal direction.
    /// The position of an occupied tile (if any) is also included as the last element.
    /// </returns>
    public List<Position> GetOrthogonalTiles(Position position, Direction direction)
    {
        var results = new List<Position>();

        if (!ContainsTile(position))
        {
            return results;
        }

        switch (direction)
        {
            case Direction.Left:
            {
                for (var x = position.X - 1; x >= 0; x--)
                {
                    results.Add(new Position(x, position.Y));

                    if (IsTileOccupied(new Position(x, position.Y)))
                    {
                        break;
                    }
                }

                break;
            }

            case Direction.Top:
            {
                for (var y = position.Y + 1; y < _board.Width; y++)
                {
                    results.Add(new Position(position.X, y));

                    if (IsTileOccupied(new Position(position.X, y)))
                    {
                        break;
                    }
                }

                break;
            }

            case Direction.Right:
            {
                for (var x = position.X + 1; x < _board.Width; x++)
                {
                    results.Add(new Position(x, position.Y));

                    if (IsTileOccupied(new Position(x, position.Y)))
                    {
                        break;
                    }
                }

                break;
            }

            case Direction.Bottom:
            {
                for (var y = position.Y - 1; y >= 0; y--)
                {
                    results.Add(new Position(position.X, y));

                    if (IsTileOccupied(new Position(position.X, y)))
                    {
                        break;
                    }
                }

                break;
            }

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(direction),
                    direction,
                    $"{(int)direction} is not a valid {nameof(Direction)}"
                );
        }

        return results;
    }

    /// <summary>
    /// Get a list of possible positions in the diagonal direction starting from
    /// the given <see cref="Position"/>. This method will check for empty tiles diagonally
    /// starting from the given location until it reaches the end of the <see cref="Board"/>
    /// or it encounter a tile which is occupied by a <see cref="Piece"/>. The positions returned by this method
    /// are guaranteed to be in order, starting from the given <see cref="Position"/> to the last possible position
    /// a <see cref="Queen"/> or <see cref="Bishop"/> can go.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> as the starting point.</param>
    /// <param name="direction">
    /// An instance of <see cref="DiagonalDirection"/> that determines the direction of the search.
    /// </param>
    /// <returns>
    /// A <see cref="List{Position}"/> of possible <see cref="Position"/> in the diagonal direction.
    /// The position of an occupied tile (if any) is also included as the last element.
    /// </returns>
    public List<Position> GetDiagonalTiles(Position position, DiagonalDirection direction)
    {
        var results = new List<Position>();

        if (!ContainsTile(position))
        {
            return results;
        }

        switch (direction)
        {
            case DiagonalDirection.TopLeft:
            {
                for (int x = position.X - 1, y = position.Y + 1; x >= 0 && y < _board.Width; x--, y++)
                {
                    results.Add(new Position(x, y));

                    if (IsTileOccupied(new Position(x, y)))
                    {
                        break;
                    }
                }

                break;
            }

            case DiagonalDirection.TopRight:
            {
                for (int x = position.X + 1, y = position.Y + 1; x < _board.Width && y < _board.Width; x++, y++)
                {
                    results.Add(new Position(x, y));

                    if (IsTileOccupied(new Position(x, y)))
                    {
                        break;
                    }
                }

                break;
            }

            case DiagonalDirection.BottomRight:
            {
                for (int x = position.X + 1, y = position.Y - 1; x < _board.Width && y >= 0; x++, y--)
                {
                    results.Add(new Position(x, y));

                    if (IsTileOccupied(new Position(x, y)))
                    {
                        break;
                    }
                }

                break;
            }

            case DiagonalDirection.BottomLeft:
            {
                for (int x = position.X - 1, y = position.Y - 1; x >= 0 && y >= 0; x--, y--)
                {
                    results.Add(new Position(x, y));

                    if (IsTileOccupied(new Position(x, y)))
                    {
                        break;
                    }
                }

                break;
            }

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(direction),
                    direction,
                    $"{(int)direction} is not a valid {nameof(DiagonalDirection)}"
                );
        }

        return results;
    }

    /// <summary>
    /// Get a list of possible L shape position starting from
    /// the given <see cref="Position"/>. The positions returned by this method are guaranteed to be located
    /// on the <see cref="Board"/>. This method doesn't check whether the position is occupied by a <see cref="Piece"/>.
    /// </summary>
    /// <param name="position">An instance of <see cref="Position"/> as the starting point.</param>
    /// <returns>
    /// A <see cref="List{Position}"/> of possible <see cref="Position"/>.
    /// Each <see cref="Position"/> represent the destination of the L shape path.
    /// </returns>
    public List<Position> GetLShapeTiles(Position position)
    {
        if (!ContainsTile(position))
        {
            return new List<Position>();
        }

        var results = new List<Position>
        {
            new(position.X - 1, position.Y + 2),
            new(position.X + 1, position.Y + 2),
            new(position.X + 2, position.Y + 1),
            new(position.X + 2, position.Y - 1),
            new(position.X + 1, position.Y - 2),
            new(position.X - 1, position.Y - 2),
            new(position.X - 2, position.Y - 1),
            new(position.X - 2, position.Y + 1)
        };

        return results.Where(ContainsTile).ToList();
    }

    #endregion

    #region Private Piece Utility Methods

    /// <summary>
    /// Create a new <see cref="Piece"/> instance based on the given <see cref="PieceType"/>.
    /// </summary>
    /// <param name="type">A <see cref="PieceType"/> instance that determines the type of the resulting <see cref="Piece"/>.</param>
    /// <param name="id">The <see cref="Piece.Id"/> of the <see cref="Piece"/> instance.</param>
    /// <param name="color">The <see cref="Piece.Color"/> of the <see cref="Piece"/> instance.</param>
    /// <param name="position">The <see cref="Piece.Position"/> of the <see cref="Piece"/> instance.</param>
    /// <param name="moveCount">
    /// An <see cref="int"/> value that indicates how many times the <see cref="Piece"/> instance has moved.
    /// </param>
    /// <returns>A <see cref="Piece"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="type"/> is not a valid <see cref="PieceType"/>.</exception>
    private static Piece CreatePiece(PieceType type, int id, PieceColor color, Position position, int moveCount = 0)
    {
        return type switch
        {
            PieceType.Pawn => new Pawn(id, color, position, moveCount),
            PieceType.Rook => new Rook(id, color, position, moveCount),
            PieceType.Knight => new Knight(id, color, position, moveCount),
            PieceType.Bishop => new Bishop(id, color, position, moveCount),
            PieceType.Queen => new Queen(id, color, position, moveCount),
            PieceType.King => new King(id, color, position, moveCount),
            _ => throw new ArgumentOutOfRangeException($"{(int)type} is not a valid PieceType")
        };
    }

    /// <summary>
    /// Generate a <see cref="Piece"/> set of the given <see cref="PieceColor"/> instance.
    /// This method will generate a <see cref="List{Piece}"/> of <see cref="Piece"/> where the <see cref="Position"/> is
    /// determined by the <see cref="Board"/> owned by this instance.
    /// </summary>
    /// <param name="color">A <see cref="PieceColor"/> instance.</param>
    /// <returns>
    /// A <see cref="List{Piece}"/> of <see cref="Piece"/> instances of the given <see cref="PieceColor"/> instance.
    /// </returns>
    private List<Piece> GeneratePieceSet(PieceColor color)
    {
        var id = color.IsWhite() ? 0 : _board.GetPieceCount(PieceColor.Black);

        var results = _board.GetPawnPositions(color)
            .Select(pos => new Pawn(id++, color, pos))
            .Cast<Piece>()
            .ToList();

        results.AddRange(
            _board.GetRookPositions(color).Select(pos => new Rook(id++, color, pos))
        );

        results.AddRange(
            _board.GetKnightPositions(color).Select(pos => new Knight(id++, color, pos))
        );

        results.AddRange(
            _board.GetBishopPositions(color).Select(pos => new Bishop(id++, color, pos))
        );

        results.Add(new Queen(id++, color, _board.GetQueenPosition(color)));
        results.Add(new King(id, color, _board.GetKingPosition(color)));

        return results;
    }

    #endregion
}