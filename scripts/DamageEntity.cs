using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class DamageEntity : Area2D
{
    [Export] public int damage = 10;
    
    public virtual void Die()
    {
        QueueFree();
    }
    
    public virtual void OnAreaEntered(Area2D a) {
        if (a is Player) OnBodyEntered(a);
    }
	
    public virtual void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            player.EmitSignal(Player.SignalName.EnemyCollided, this);
        }
    }
}
