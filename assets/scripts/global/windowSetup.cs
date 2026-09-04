using Godot;

public partial class windowSetup : Node
{
    private static readonly Vector2I WindowSize = new Vector2I(100, 100);
    private Vector2I TargetSize = WindowSize;

    [Export] public int Scale = 3;
    [Export] public int MarginX = 0;
    [Export] public int MarginY = 0;

    public override void _Ready()
    {
        Callable.From(PositionWindow).CallDeferred();
    }

    private void PositionWindow()
    {
        var window = GetWindow();

        int screenIndex = window.CurrentScreen;
        Rect2I screenRect = DisplayServer.ScreenGetUsableRect(screenIndex);

        int targetWidth = TargetSize.X * Scale;
        int targetHeight = TargetSize.Y * Scale;

        window.Size = new Vector2I(targetWidth, targetHeight);
        window.Borderless = true;
        window.AlwaysOnTop = true;
        window.Unresizable = true;

        int x = screenRect.Position.X + screenRect.Size.X - targetWidth - MarginX;
        int y = screenRect.Position.Y + screenRect.Size.Y - targetHeight - MarginY;

        window.Position = new Vector2I(x, y);
    }
}