using Godot;

// Player character with jump animation and directional facing
public partial class Player : CharacterBody2D
{
    private AnimatedSprite2D _sprite = null!;
    private AnimationPlayer _animationPlayer = null!;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    // Called when game over - tints player red
    public void OnGameover()
    {
        _sprite.Modulate = new Color(1.0f, 0.2f, 0.2f);
    }

    // Trigger jump animation
    public void Jump()
    {
        _animationPlayer.Stop(true);
        _animationPlayer.Play("PlayerJump");
        _animationPlayer.Queue("PlayerIdle");
    }

    // Face the player toward a direction
    public void PlayerLookAt(Vector2 direction)
    {
        var delta = Position.X - direction.X;
        if (Mathf.Abs(delta) > 0)
        {
            _sprite.FlipH = delta > 0;
        }
    }
}
