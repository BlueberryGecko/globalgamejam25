using Godot;

namespace Globalgamejam25.scripts;

public static class Extensions
{
    public static Vector2 Swap(this Vector2 v) => new(v.Y, v.X);
}
