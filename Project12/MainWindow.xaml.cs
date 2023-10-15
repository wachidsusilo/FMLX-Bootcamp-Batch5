using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Project11.Chess.Boards;
using Project11.Chess.Pieces;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Project12;

public partial class MainWindow
{
    private bool _isChangingWindowState;
    private double _cursorRelativeX;
    private double _cursorAbsoluteY;
    private bool _isDraggingTile;
    private string _draggedTileNotation;
    
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        _isDraggingTile = false;
        _draggedTileNotation = "";
    }

    private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Border border)
        {
            return;
        }
        
        var availableWidth = border.ActualWidth - 400;
        var availableHeight = border.ActualHeight - 128;

        if (availableWidth > availableHeight)
        {
            _viewModel.BoardSize = (int)availableHeight;
        }
        else
        {
            _viewModel.BoardSize = (int)availableWidth;
        }

        _viewModel.BoardFontSize = SystemFonts.MessageFontSize * _viewModel.BoardSize / 460;
        _viewModel.NumberMinWidth = 24 * _viewModel.BoardSize / 460;
        _viewModel.PromotionCardWidth = 400 * _viewModel.BoardSize / 460;
        _viewModel.PromotionCardHeight = 150 * _viewModel.BoardSize / 460;
        _viewModel.TileSize = 53 * _viewModel.BoardSize / 460;

        var margin = 10.0 * _viewModel.BoardSize / 460;
        _viewModel.PromotionCardContentMargin = new Thickness(margin, margin, margin / 2, margin);
        _viewModel.PromotionCardContentMarginEnd = new Thickness(margin / 2, margin, margin, margin);
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Canvas canvas)
        {
            return;
        }
        
        _viewModel.TileSize = (int)(canvas.ActualWidth / 8);
    }

    private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((sender as Border)?.Tag is not string tag)
        {
            return;
        }

        _viewModel.ClearTileHighlight();
        
        if (_viewModel.IsGameOver)
        {
            return;
        }
        
        var action = _viewModel.GetChessActionRelatedToTile(tag);
        
        _viewModel.ClearTileHint();

        if (action is null)
        {
            _viewModel.ShowTileHint(tag);   
        }
        else
        {
            _viewModel.MovePiece(action);
        }

        if (!_viewModel.StartDraggingTile(tag))
        {
            return;
        }
        
        _draggedTileNotation = tag;
        _isDraggingTile = true;
    }
    
    private void Tile_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDraggingTile)
        {
            return;
        }
        
        var point = e.GetPosition(CanvasBoard);
        var halfTileSize = _viewModel.TileSize / 2;
        var x = (int)point.X  - halfTileSize;
        var y = (int)point.Y - halfTileSize;

        _viewModel.MovableTile.Position = new Position(x, y);
    }
    
    private void Tile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDraggingTile || _viewModel.IsGameOver)
        {
            return;
        }
        
        var point =  e.GetPosition(CanvasBoard);
        var x = (int)Math.Floor(point.X / _viewModel.TileSize);
        var y = (int)Math.Floor((CanvasBoard.ActualHeight -  point.Y) / _viewModel.TileSize);
        var position = new Position(x, y);
        var action = _viewModel.GetChessActionRelatedToTile(_viewModel.GetTileNotation(position));
        var draggedTileNotation = _draggedTileNotation;

        _isDraggingTile = false;
        _draggedTileNotation = "";
        
        if (!_viewModel.ContainsTile(position) || action is null)
        {
            _viewModel.StopDraggingTile(draggedTileNotation);
            return;
        }

        _viewModel.ClearTileHint();
        _viewModel.StopDraggingTile(draggedTileNotation, !_viewModel.MovePiece(action));
    }

    private void Tile_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((sender as Border)?.Tag is not string tag)
        {
            return;
        }
        
        _viewModel.ToggleTileHighlight(tag);
    }

    private void ActionHistory_Click(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not Button { Tag: string tag } || !int.TryParse(tag, out var actionId))
        {
            return;
        }
        
        _viewModel.GoToAction(actionId);
    }

    private void Undo_Click(object sender, RoutedEventArgs routedEventArgs)
    {
        _viewModel.UndoAction();
    }

    private void Redo_Click(object sender, RoutedEventArgs routedEventArgs)
    {
        _viewModel.RedoAction();
    }

    private void Reset_Click(object sender, RoutedEventArgs routedEventArgs)
    {
        _viewModel.Reset();
    }

    private void Promotion_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string tag })
        {
            return;
        }

        switch (tag)
        {
            case "Q":
                _viewModel.PromotePawn(PieceType.Queen);
                break;
            case "R":
                _viewModel.PromotePawn(PieceType.Rook);
                break;
            case "B":
                _viewModel.PromotePawn(PieceType.Bishop);
                break;
            case "N":
                _viewModel.PromotePawn(PieceType.Knight);
                break;
        }

        _viewModel.PromotionCardVisibility = Visibility.Collapsed;
    }
    
    #region Window Control Event Listeners
    
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        BorderThickness = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Keyboard.ClearFocus();
        FocusManager.SetFocusedElement(this, null);
    }

    private void HeaderGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            var cursorPosition = e.GetPosition(this);
            _cursorRelativeX = cursorPosition.X / ActualWidth;
            _cursorAbsoluteY = cursorPosition.Y;
        }

        if (e.ClickCount > 1)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            _isChangingWindowState = true;
        }
        else
        {
            try
            {
                DragMove();
            }
            catch
            {
                // ignored
            }
        }
    }

    private void HeaderGrip_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized &&
            !_isChangingWindowState)
        {
            WindowState = WindowState.Normal;

            var helper = new WindowInteropHelper(this);
            var currentScreen = Screen.FromHandle(helper.Handle);

            var cursorPosition = e.GetPosition(this);
            Left = currentScreen.Bounds.Left + cursorPosition.X - ActualWidth * _cursorRelativeX;
            Top = cursorPosition.Y - _cursorAbsoluteY;

            try
            {
                DragMove();
            }
            catch
            {
                // ignored
            }
        }

        if (_isChangingWindowState)
        {
            _isChangingWindowState = false;
        }
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    #endregion

    #region Window Configuration 
    
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        try
        {
            var text = App.LoadConfig("WindowPlacement");
            if (text is null) return;

            var windowPlacement = JsonSerializer.Deserialize<WindowPlacement>(text);
            windowPlacement.Length = Marshal.SizeOf(typeof(WindowPlacement));
            windowPlacement.Flags = 0;
            windowPlacement.ShowCmd =
                windowPlacement.ShowCmd == SwShowMinimized ? SwShowNormal : windowPlacement.ShowCmd;

            var handle = new WindowInteropHelper(this).Handle;
            SetWindowPlacement(handle, ref windowPlacement);
        }
        catch
        {
            // ignored
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        try
        {
            var handle = new WindowInteropHelper(this).Handle;
            GetWindowPlacement(handle, out var windowPlacement);
            App.SaveConfig("WindowPlacement", JsonSerializer.Serialize(windowPlacement));
        }
        catch
        {
            // ignored
        }
    }


    #endregion

    #region Win32 API declarations to set and get window placement

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

    private const int SwShowNormal = 1;
    private const int SwShowMinimized = 2;

    #endregion

}