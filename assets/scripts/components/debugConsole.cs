using Godot;

public partial class debugConsole : VBoxContainer
{
    [Export]
    public PackedScene LogMessageScene { get; set; }

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