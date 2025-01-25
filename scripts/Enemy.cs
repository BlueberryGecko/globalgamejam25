using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] private int maxHealth = 40;
    [Export] private int health = 40;
    
    public override void _Ready() {
        health = maxHealth;
    }
    
    public override void OnBodyEntered(Node2D body) {
        base.OnBodyEntered(body);
        if (body is Bubble b && !b.isPoppping) {
            health -= 20;
            this.BlinkWithTween();
            if (health <= 0) {
                Die();
            }
            b.Burst();
        }
    }
}