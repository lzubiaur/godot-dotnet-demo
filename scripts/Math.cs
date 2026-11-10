using Godot;

// Math utilities for isometric coordinate conversion
public static class Math
{
    // Isometric cell size in pixels
    public const int CellSize = 32;

    // Convert from Cartesian to Isometric space
    public static Vector2 ToIso(Vector2 point)
    {
        return new Vector2(point.X - point.Y, (point.X + point.Y) / 2) * CellSize;
    }

    // Convert from Isometric to Cartesian space
    public static Vector2 ToCart(Vector2 point)
    {
        return new Vector2(point.X / 2 + point.Y, point.Y - point.X / 2);
    }

    // Return the Isometric cell center from Cartesian space
    public static Vector2 ToIsoCenter(Vector2 point)
    {
        return ToIso(point) + new Vector2(0, CellSize / 2.0f);
    }
}
