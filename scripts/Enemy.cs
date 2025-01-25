using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : Area2D {
    [Export] public int damage = 10;

    public abstract void Die();
}