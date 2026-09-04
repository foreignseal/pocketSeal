using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class debugLog : Node
{
    public static debugLog Instance { get; private set; }
    public static bool IsDebugEnabled { get; set; } = true;

    private static readonly PackedScene ConsoleScene = GD.Load<PackedScene>("res://scenes/windows/debugConsoleScene.tscn");

    public static event Action<string, double, double> OnLogMessage;
    private static readonly List<(string msg, double life, double fade)> _backlog = new();
    private static bool _hasSubscribers = false;

    private Window _debugWindow;

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOOLWINDOW   = 0x00000080L; // Hides from taskbar
    private const long WS_EX_APPWINDOW    = 0x00040000L; // App window style (remove)
    private const long WS_EX_TRANSPARENT  = 0x00000020L; // Mouse click-through
    private const long WS_EX_LAYERED      = 0x00080000L; // REQUIRED for WS_EX_TRANSPARENT to work

    private const uint SWP_NOMOVE       = 0x0002;
    private const uint SWP_NOSIZE       = 0x0001;
    private const uint SWP_NOZORDER     = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020; // Forces Windows to apply style changes immediately

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        if (IsDebugEnabled)
        {
            Callable.From(OpenDebugWindow).CallDeferred();
        }
    }

    public static void Subscribe(Action<string, double, double> handler)
    {
        OnLogMessage += handler;
        _hasSubscribers = true;

        foreach (var (msg, life, fade) in _backlog)
        {
            handler(msg, life, fade);
        }
        _backlog.Clear();
    }

    public static void Unsubscribe(Action<string, double, double> handler)
    {
        OnLogMessage -= handler;
        if (OnLogMessage == null)
            _hasSubscribers = false;
    }

    public static void Log(string message, double lifetime = 3.0, double fadeTime = 0.8)
    {
        if (!IsDebugEnabled) return;
        if (!_hasSubscribers)
        {
            _backlog.Add((message, lifetime, fadeTime));
            return;
        }

        OnLogMessage?.Invoke(message, lifetime, fadeTime);
    }

    private void OpenDebugWindow()
    {
        if (ConsoleScene == null)
        {
            GD.PrintErr("debugLog: Could not load debugConsole.tscn! Check the res:// path.");
            return;
        }

        _debugWindow = new Window
        {
            Title = "Debug Console",
            Size = new Vector2I(350, 400),
            Borderless = true,
            AlwaysOnTop = true,
            Unresizable = true,
            Transparent = true,
            TransparentBg = true,
            Transient = false,
            Exclusive = false,
            Visible = false,
            MousePassthrough = true 
        };

        Control consoleUi = ConsoleScene.Instantiate<Control>();
        consoleUi.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        consoleUi.MouseFilter = Control.MouseFilterEnum.Ignore;

        _debugWindow.AddChild(consoleUi);
        GetTree().Root.AddChild(_debugWindow);

        // Screen positioning: Bottom-Left corner
        int currentScreen = DisplayServer.WindowGetCurrentScreen();
        Rect2I screenRect = DisplayServer.ScreenGetUsableRect(currentScreen);

        int marginX = 5;
        int marginY = 5;

        int x = screenRect.Position.X + marginX;
        int y = screenRect.Position.Y + screenRect.Size.Y - _debugWindow.Size.Y - marginY;

        Callable.From(() =>
        {
            _debugWindow.Position = new Vector2I(x, y);
            _debugWindow.Show();
            ApplyWindowStyles(_debugWindow);
        }).CallDeferred();
    }

    private void ApplyWindowStyles(Window window)
    {
        if (OS.GetName() != "Windows") return;

        IntPtr hWnd = (IntPtr)DisplayServer.WindowGetNativeHandle(
            DisplayServer.HandleType.WindowHandle, 
            window.GetWindowId()
        );

        if (hWnd != IntPtr.Zero)
        {
            long style = GetWindowLongPtr(hWnd, GWL_EXSTYLE).ToInt64();

            // 1. Remove taskbar style
            style &= ~WS_EX_APPWINDOW;

            // 2. Add tool window, layered, and transparent (click-through) flags
            style |= WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_TRANSPARENT;

            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(style));

            // 3. Force Windows to redraw frame and re-evaluate hit tests
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, 
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
}