using Godot;
using System.Collections.Generic;

// Procedurally generates tiles for the endless runner path
public partial class TilesGenerator : Node
{
    [Signal]
    public delegate void NewTileEventHandler(Vector2 position, int tileType);

    [Signal]
    public delegate void SpawnEnemyEventHandler(Vector2 position);

    public enum TileType { Empty = -1, Ground = 0 }

    private const int MaxPoolSize = 20;
    private List<Vector2> _pool = new();
    private Global _global = null!;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/global");
    }

    private bool RollDice(float odds)
    {
        return GD.Randf() > odds;
    }

    private Vector2 Choose(float odds, Vector2 value1, Vector2 value2)
    {
        return RollDice(odds) ? value1 : value2;
    }

    public Vector2 Front()
    {
        return _pool.Count > 0 ? _pool[0] : Vector2.Zero;
    }

    public void Push(Vector2 direction)
    {
        _pool.Add(direction);
    }

    public Vector2 Pop()
    {
        if (_pool.Count == 0) return Vector2.Zero;
        var front = _pool[0];
        _pool.RemoveAt(0);
        return front;
    }

    private void AddNewTileToMap(Vector2 position, TileType tileType)
    {
        if (tileType != TileType.Empty)
        {
            EmitSignal(SignalName.NewTile, position, (int)tileType);
        }
    }

    private void SpawnNewEnemy(Vector2 position)
    {
        if (RollDice(0.9f))
        {
            EmitSignal(SignalName.SpawnEnemy, position);
        }
    }

    private TileType GenerateNextTileType(Vector2 pos, TileType prevType)
    {
        if (prevType != TileType.Empty && RollDice(0.6f))
        {
            return TileType.Empty;
        }
        return TileType.Ground;
    }

    // Async generator that creates tiles as player progresses
    public async void StartGeneratingTiles()
    {
        var position = Vector2.Zero;
        var left = new Vector2(1, 0);
        var right = new Vector2(0, 1);
        var tileType = TileType.Ground;

        while (true)
        {
            // Sleep until we need to create a new tile
            if (_pool.Count > MaxPoolSize)
            {
                await ToSignal(_global.GetGame(), Game.SignalName.PlayerStep);
            }

            Push(position);
            AddNewTileToMap(position, tileType);
            SpawnNewEnemy(position);

            tileType = GenerateNextTileType(position, tileType);
            position += Choose(0.5f, left, right);
        }
    }
}
