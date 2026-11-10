using Godot;

// Displays current score with bounce animation on update
public partial class ScoreLabel : Label
{
    private Global _global = null!;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/global");
        _global.GetGame().PlayerStep += OnPlayerStep;
        UpdateScore();
    }

    private void UpdateScore()
    {
        Text = _global.Score.ToString();
        Bounce();
    }

    private void Bounce()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.3f, 1.3f), 0.1f);
        tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.15f);
    }

    private void OnPlayerStep(int stepCount)
    {
        UpdateScore();
    }
}
