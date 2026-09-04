using Godot;

public partial class logMessage : PanelContainer
{
    private RichTextLabel _label;

    public override void _Ready()
    {
        _label = GetNode<RichTextLabel>("RichTextLabel");
    }

    public void Display(string text, double lifetime = 3.0, double fadeTime = 1.0)
    {
        if (_label == null)
        {
            _label = GetNode<RichTextLabel>("RichTextLabel");
        }

        _label.Text = text;

        Tween tween = CreateTween();
        tween.TweenInterval(lifetime);
        tween.TweenProperty(this, "modulate:a", 0.0f, fadeTime);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}