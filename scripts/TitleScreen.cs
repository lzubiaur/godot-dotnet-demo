using Godot;

// Title screen with background sprite
public partial class TitleScreen : Node2D
{
    private Sprite2D _background = null!;

    public override void _Ready()
    {
        _background = GetNode<Sprite2D>("BackgroundSprite");
    }

    public override void _Process(double delta)
    {
        // Placeholder for future title screen logic
    }
}
