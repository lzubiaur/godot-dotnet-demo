using Godot;

// HUD with timeout progress bar - game ends when bar depletes
public partial class HeadupDisplay : CanvasLayer
{
    [Signal]
    public delegate void GameplayTimeoutEventHandler();

    private Tween? _tween;
    private TextureProgressBar _progressBar = null!;
    private Global _global = null!;

    private float _idle;
    private float _extraTime;
    private float _minExtraTime;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/global");
        _progressBar = GetNode<TextureProgressBar>("TextureProgressBar");

        _idle = _global.Idle;
        _extraTime = _global.ExtraTime;
        _minExtraTime = _global.MinExtraTime;

        _global.GetGame().Gameover += OnGameover;
        _global.GetGame().PlayerStep += OnPlayerStep;
        SetTimeout(_global.Idle);
    }

    private void SetTimeout(float time)
    {
        _tween?.Kill();
        _tween = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
        var fromValue = (time / Global.MaxIdleTimeout) * 100;
        _tween.TweenProperty(_progressBar, "value", 0.0, time).From(fromValue);
    }

    private float GetRemainingTime()
    {
        return Global.MaxIdleTimeout * ((float)_progressBar.Value / 100f);
    }

    private void AddExtraTime(float time)
    {
        var newTimeout = Mathf.Min(GetRemainingTime() + time, Global.MaxIdleTimeout);
        SetTimeout(newTimeout);
    }

    private void OnPlayerStep(int stepCount)
    {
        var newExtraTime = _extraTime + ((_extraTime - _minExtraTime) * (1 - _global.Difficulty));
        AddExtraTime(newExtraTime);
    }

    private void OnGameover()
    {
        // Stop the timeout when game ends
        _tween?.Kill();
    }

    // Connected via signal in scene
    public void OnTextureProgressValueChanged(double value)
    {
        if (value <= 0)
        {
            EmitSignal(SignalName.GameplayTimeout);
        }
    }
}
