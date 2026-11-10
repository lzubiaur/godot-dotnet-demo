# Cactus Jump - Godot C# Demo

An isometric endless runner game demonstrating Godot 4.x with C# scripting, ported from the original GDScript version for a .NET user group presentation.

## Prerequisites

- **Godot 4.4+** (.NET version) - Download from [godotengine.org](https://godotengine.org/download)
- **.NET 8.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)

## How to Open

1. Launch Godot 4.4+ (.NET version)
2. Click "Import" in the Project Manager
3. Navigate to this folder and select `project.godot`
4. Click "Import & Edit"
5. Wait for Godot to import assets and build the C# solution

## How to Run

- Press **F5** or click the Play button in the editor
- Alternatively: `dotnet build` then run from Godot

## Controls

| Action | Key |
|--------|-----|
| Jump (1 tile) | A or Up Arrow |
| Dash (2 tiles) | D or Down Arrow |

## Gameplay

- Jump or dash across procedurally generated isometric tiles
- Avoid falling into empty tiles
- Dodge the crab enemies - you're safe while moving!
- Score increases with each successful step
- Level up every 20 points
- Time bar depletes - keep moving to add extra time!

## Project Structure

```
godot-game-csharp/
├── assets/           # Sprites, textures, animations
│   ├── jump/         # Player jump animation frames
│   ├── idle/         # Player idle animation frames
│   └── fonts/        # Custom fonts
├── scenes/           # Godot scene files (.tscn)
│   ├── Game.tscn     # Main game scene
│   ├── Player.tscn   # Player character scene
│   ├── Enemy.tscn    # Crab enemy scene
│   ├── HUD.tscn      # Heads-up display
│   └── TitleScreen.tscn  # Title screen
├── scripts/          # C# source files
│   ├── Game.cs       # Main game controller (~180 lines)
│   ├── Player.cs     # Player character logic (~35 lines)
│   ├── Enemy.cs      # Enemy AI with tween movement (~55 lines)
│   ├── Global.cs     # Singleton for game state (~40 lines)
│   ├── TilesGenerator.cs  # Procedural tile generation (~90 lines)
│   ├── HeadupDisplay.cs   # Timeout bar logic (~60 lines)
│   ├── ScoreLabel.cs      # Score display with bounce (~25 lines)
│   ├── TitleScreen.cs     # Title screen (~15 lines)
│   └── Math.cs       # Isometric math utilities (~20 lines)
├── project.godot     # Godot project configuration
└── GodotCSharpDemo.csproj  # .NET project file
```

## Scripts for Live-Coding

### 1. Player.cs (Best - ~35 lines)
Simple script showing:
- Node inheritance (`CharacterBody2D`)
- Getting child nodes (`GetNode<T>()`)
- Animation playback with queue
- Sprite flipping based on direction

### 2. Math.cs (Good - ~20 lines)
Static utility class demonstrating:
- Pure C# in Godot (no node inheritance)
- Isometric coordinate math
- `Vector2` operations

### 3. Enemy.cs (Good - ~55 lines)
Shows:
- `[Export]` properties for editor tweaking
- `[Signal]` for custom events
- `Tween` for smooth animations
- Area2D collision detection

## Key Concepts Demonstrated

- `partial` classes inheriting from Godot nodes
- `[Signal]` delegate pattern for custom signals
- `[Export]` for editor-visible properties
- `GetNode<T>()` for type-safe node access
- `Input.IsActionJustPressed()` for input handling
- `EmitSignal()` for signal emission
- `async/await` with `ToSignal()` for coroutines
- `Tween` for smooth property animations
- `TileMapLayer` for isometric tile rendering
- Autoload singletons for global state
- `Path2D` + `PathFollow2D` for movement along curves
