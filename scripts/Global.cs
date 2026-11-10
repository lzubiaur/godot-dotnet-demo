using Godot;

// Global singleton for game state and settings
// Registered as autoload in project.godot
public partial class Global : Node
{
    // Current scene reference
    public Node CurrentScene { get; set; } = null!;

    // Player state
    public int Gold { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int Level { get; set; } = 0;
    public int High { get; set; } = 0;
    public int Total { get; set; } = 0;

    // Game settings
    public bool SoundOn { get; set; } = true;
    public bool MusicOn { get; set; } = true;

    // Animation timing
    public const float StepDuration = 0.3f;
    public const float MaxIdleTimeout = 5f;

    // Gameplay parameters
    public float Idle { get; set; } = MaxIdleTimeout;
    public float ExtraTime { get; set; } = 0.4f;
    public float MinExtraTime { get; set; } = 0.3f;
    public float Difficulty { get; set; } = 0f;

    public Game GetGame()
    {
        return GetNode<Game>("/root/Game");
    }

    public override void _Ready()
    {
        var root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }
}
