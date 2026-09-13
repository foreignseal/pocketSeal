using Godot;

public partial class logMessage : PanelContainer
{
    private RichTextLabel _label;

    public override void _Ready()
    {
        Setup();
    }

    private void Setup()
    {
        if (_label != null) return;

        _label = GetNode<RichTextLabel>("RichTextLabel");

        // Force wrapping behavior
        _label.AutowrapMode = TextServer.AutowrapMode.Arbitrary;
        _label.FitContent = true;
        _label.ScrollActive = false;
        _label.MouseFilter = MouseFilterEnum.Ignore;

        // Force PanelContainer to 340px wide so text wraps within the 350px window
        CustomMinimumSize = new Vector2(340, 0);
        Size = new Vector2(340, 0);

        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
        SizeFlagsVertical = SizeFlags.ShrinkEnd;
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void Display(string text, double lifetime = 3.0, double fadeTime = 1.0)
    {
        Setup();

        _label.Text = text;

        Tween tween = CreateTween();
        tween.TweenInterval(lifetime);
        tween.TweenProperty(this, "modulate:a", 0.0f, fadeTime);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}