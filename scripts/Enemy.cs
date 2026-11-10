using Godot;

// Enemy that moves between two points using tweens
public partial class Enemy : Area2D
{
    [Signal]
    public delegate void FinishedEventHandler();

    [Export]
    public Vector2 Start { get; set; } = Vector2.Zero;

    [Export]
    public Vector2 End { get; set; } = Vector2.Zero;

    [Export]
    public int Velocity { get; set; } = 10;

    [Export]
    public int Delay { get; set; } = 0;

    public override void _Ready()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        var pStart = Math.ToIso(Start);
        var pEnd = Math.ToIso(End);
        var length = pStart.DistanceTo(pEnd);
        var duration = length / Velocity;

        // Set initial position immediately (don't defer)
        Position = pStart;

        var tween = CreateTween();
        tween.TweenProperty(this, "position", pEnd, duration)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Linear);
        tween.TweenCallback(Callable.From(OnMoveAlongPathCallback))
            .SetDelay(duration);
        tween.Play();
    }

    private void OnMoveAlongPathCallback()
    {
        EmitSignal(SignalName.Finished);
    }
}
