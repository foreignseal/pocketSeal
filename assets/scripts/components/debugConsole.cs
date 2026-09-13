using Godot;

public partial class debugConsole : VBoxContainer
{
    [Export]
    public PackedScene LogMessageScene { get; set; }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.TopLeft);

        // 2. Lock the container to the 350x400 window
        CustomMinimumSize = new Vector2(350, 400);
        Size = new Vector2(350, 400);

        Alignment = BoxContainer.AlignmentMode.End;
        AddThemeConstantOverride("separation", 4);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public override void _EnterTree()
    {
        debugLog.Subscribe(HandleLogMessage);
    }

    public override void _ExitTree()
    {
        debugLog.Unsubscribe(HandleLogMessage);
    }

    private void HandleLogMessage(string message, double lifetime, double fadeTime)
    {
        if (LogMessageScene == null)
        {
            GD.PrintErr("debugConsole: LogMessageScene is not assigned!");
            return;
        }

        var msgInstance = LogMessageScene.Instantiate<logMessage>();
        AddChild(msgInstance);
        msgInstance.Display(message, lifetime, fadeTime);
    }
}