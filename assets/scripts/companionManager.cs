using Godot;

public partial class companionManager : Control
{
    public override void _Ready()
    {
        debugLog.IsDebugEnabled = true;

        Callable.From(() => {
            debugLog.Log("Companion Manager Ready! I'm still testing this debugLog system.", 4.0, 0.8);
            debugLog.Log("Companion Manager Ready! I'm still testing this debugLog system.", 4.0, 0.8);
            debugLog.Log("Companion Manager Ready! I'm still testing this debugLog system.", 4.0, 0.8);
            debugLog.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 4.0, 0.8);
        }).CallDeferred();
    }
}