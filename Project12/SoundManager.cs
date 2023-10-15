using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Project12;

public class SoundManager
{
    private readonly MediaPlayer _capture = new();
    private readonly MediaPlayer _moveCheck = new();
    private readonly MediaPlayer _moveSelf = new();
    private readonly MediaPlayer _notify = new();
    private readonly MediaPlayer _promote = new();

    public SoundManager()
    {
        _capture.Open(new Uri("Sounds/Capture.mp3", UriKind.Relative));
        _moveCheck.Open(new Uri("Sounds/MoveCheck.mp3", UriKind.Relative));
        _moveSelf.Open(new Uri("Sounds/MoveSelf.mp3", UriKind.Relative));
        _notify.Open(new Uri("Sounds/Notify.mp3", UriKind.Relative));
        _promote.Open(new Uri("Sounds/Promote.mp3", UriKind.Relative));

        _capture.MediaFailed += OnMediaFailed;
        _moveCheck.MediaFailed += OnMediaFailed;
        _moveSelf.MediaFailed += OnMediaFailed;
        _notify.MediaFailed += OnMediaFailed;
        _promote.MediaFailed += OnMediaFailed;
    }

    private static void OnMediaFailed(object? sender, ExceptionEventArgs args)
    {
        Console.WriteLine(args.ErrorException.Message);
    }

    public void Play(ChessSound chessSound)
    {
        switch (chessSound)
        {
            case ChessSound.Capture:
                _capture.Stop();
                _capture.Play();
                break;
            case ChessSound.MoveCheck:
                _moveCheck.Stop();
                _moveCheck.Play();
                break;
            case ChessSound.MoveSelf:
                _moveSelf.Stop();
                _moveSelf.Play();
                break;
            case ChessSound.Notify:
                _notify.Stop();
                _notify.Play();
                break;
            case ChessSound.Promote:
                _promote.Stop();
                _promote.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(chessSound), chessSound, $"{(int)chessSound} is not a valid Type");
        }
    }

    public void PlayDelayed(ChessSound chessSound, double delayMs)
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(delayMs)
        };

        timer.Tick += (_, _) =>
        {
            Play(chessSound);
            timer.Stop();
        };

        timer.Start();
    }

    public enum ChessSound
    {
        Capture,
        MoveCheck,
        MoveSelf,
        Notify,
        Promote
    }
}