using Godot;
using System.Collections.Generic;

// Main game controller - handles player movement, tile generation, and game state
public partial class Game : Node2D
{
    [Signal]
    public delegate void GameoverEventHandler();

    [Signal]
    public delegate void PlayerStepEventHandler(int stepCount);

    [Signal]
    public delegate void LevelUpEventHandler();

    private static readonly PackedScene EnemyScene =
        GD.Load<PackedScene>("res://scenes/Enemy.tscn");

    private TilesGenerator _tilesGen = null!;
    private Player _player = null!;
    private Path2D _path = null!;
    private PathFollow2D _pathFollow = null!;
    private TileMapLayer _tileMap = null!;
    private Global _global = null!;

    private bool _isGameover = false;
    private bool _isInvincible = false;
    private bool _moving = false;

    private bool _cachedJump = false;
    private bool _cachedDash = false;

    public override void _EnterTree()
    {
        GD.Randomize();
        InitializeLevelParameters();
    }

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/global");
        _player = GetNode<Player>("Player");
        _path = GetNode<Path2D>("Path2D");
        _pathFollow = GetNode<PathFollow2D>("Path2D/PathFollow2D");
        _tileMap = GetNode<TileMapLayer>("GroundTileMap");

        InitializeTilesGenerator();
        StartGame();
    }

    private void InitializeTilesGenerator()
    {
        _tilesGen = new TilesGenerator();
        AddChild(_tilesGen);
        _tilesGen.NewTile += OnNewTile;
        _tilesGen.SpawnEnemy += OnSpawnEnemy;
    }

    private void InitializeLevelParameters()
    {
        var global = GetNode<Global>("/root/global");
        global.Idle = Global.MaxIdleTimeout * 0.6f;
        global.Score = 0;
        global.Level = 0;
    }

    private void StartGame()
    {
        _isGameover = false;
        _tilesGen.StartGeneratingTiles();
        _tilesGen.Pop(); // Skip the start position cell
        GetTree().Paused = false;
        SetPlayerLookAt(_tilesGen.Front());
    }

    private void OnNewTile(Vector2 position, int tileType)
    {
        _tileMap.CallDeferred("set_cell", position, 0, TileTypeToAtlasCoords(tileType));
    }

    private Vector2I TileTypeToAtlasCoords(int tileType)
    {
        return tileType switch
        {
            (int)TilesGenerator.TileType.Empty => new Vector2I(-1, -1),
            (int)TilesGenerator.TileType.Ground => new Vector2I(0, 0),
            _ => new Vector2I(-1, -1)
        };
    }

    private int AtlasCoordsToTileType(Vector2I coords)
    {
        if (coords == new Vector2I(-1, -1))
            return (int)TilesGenerator.TileType.Empty;
        if (coords == new Vector2I(0, 0))
            return (int)TilesGenerator.TileType.Ground;
        return (int)TilesGenerator.TileType.Empty;
    }

    private void SetPlayerLookAt(Vector2 point)
    {
        _player.PlayerLookAt(Math.ToIso(point));
    }

    private void OnSpawnEnemy(Vector2 position)
    {
        var enemy = EnemyScene.Instantiate<Enemy>();
        enemy.Velocity = 50;
        enemy.ZIndex = 10;
        enemy.Start = position - new Vector2(3, 0);
        enemy.End = position + new Vector2(3, 0);
        enemy.BodyEntered += OnCollidedWithEnemy;
        _tileMap.CallDeferred(MethodName.AddChild, enemy);
    }

    private void OnCollidedWithEnemy(Node other)
    {
        if (!_moving)
        {
            GameOver();
        }
    }

    public override void _Process(double delta)
    {
        if (_isGameover) return;

        var steps = ProcessInputs();
        if (steps.Count > 0)
        {
            StartMovingPlayerAlongPath(steps);
        }
    }

    private List<Vector2> ProcessInputs()
    {
        var steps = new List<Vector2>();

        // Cache inputs while moving, process when stopped
        if (Input.IsActionJustPressed("Jump") || _cachedJump)
        {
            if (_moving)
            {
                _cachedJump = true;
                return steps;
            }
            AcknowledgeStep(1);
            steps.Add(_tilesGen.Pop());
        }
        else if (Input.IsActionJustPressed("Dash") || _cachedDash)
        {
            if (_moving)
            {
                _cachedDash = true;
                return steps;
            }
            AcknowledgeStep(2);
            steps.Add(_tilesGen.Pop());
            steps.Add(_tilesGen.Pop());
        }

        _cachedDash = false;
        _cachedJump = false;

        return steps;
    }

    private void StartMovingPlayerAlongPath(List<Vector2> steps)
    {
        var curve = _path.Curve;
        curve.ClearPoints();
        curve.AddPoint(_player.Position);
        foreach (var point in steps)
        {
            curve.AddPoint(Math.ToIso(point));
        }

        var tween = GetTree().CreateTween();
        tween.TweenMethod(Callable.From<float>(OnMovingAlongPath), 0.0f, 1.0f, Global.StepDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);

        var lastStep = steps[steps.Count - 1];
        tween.TweenCallback(Callable.From(() => OnMovingAlongPathFinished(lastStep)))
            .SetDelay(Global.StepDuration);

        SetPlayerLookAt(lastStep);
        _player.Jump();
    }

    private Vector2 GetNextPositionOnPath(float t)
    {
        var pointCount = _path.Curve.PointCount;
        var pointIndex = Mathf.Min((int)(pointCount * t) + 1, pointCount - 1);
        return _path.Curve.GetPointPosition(pointIndex);
    }

    private void OnMovingAlongPath(float value)
    {
        _moving = true;
        _player.PlayerLookAt(GetNextPositionOnPath(value));
        _pathFollow.ProgressRatio = value;
    }

    private void CheckPlayerPositionOnMap(Vector2 position)
    {
        var coords = _tileMap.GetCellAtlasCoords((Vector2I)position);
        var tile = AtlasCoordsToTileType(coords);
        if (tile == (int)TilesGenerator.TileType.Empty)
        {
            GameOver();
        }
    }

    private void OnMovingAlongPathFinished(Vector2 destination)
    {
        _moving = false;
        SetPlayerLookAt(_tilesGen.Front());
        CheckPlayerPositionOnMap(destination);
    }

    private void AcknowledgeStep(int stepCount)
    {
        _global.Score += stepCount;
        EmitSignal(SignalName.PlayerStep, stepCount);

        if (_global.Score % 20 == 0)
        {
            _global.Level++;
            EmitSignal(SignalName.LevelUp);
        }

        if (_global.High < _global.Score)
        {
            _global.High = _global.Score;
        }
    }

    private void Respawn()
    {
        // Clear path before respawn
        _path.Curve.ClearPoints();
        GetTree().ReloadCurrentScene();
    }

    private async void GameOver()
    {
        if (_isGameover || _isInvincible) return;

        _isGameover = true;
        EmitSignal(SignalName.Gameover);

        // Delay respawn for 1 second
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        Respawn();
    }

    // Connected via signal from HUD
    public void OnHUDGameplayTimeout()
    {
        GameOver();
    }
}
