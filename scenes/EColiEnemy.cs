using Godot;
using System;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class EColiEnemy : Enemy
{
	[Export] private int maxHealth = 40;
	[Export] private int health = 40;
	[Export] public float speed = 100;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		health = maxHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var dir = (Consts.world.player.Position - Position).Normalized();
		Position += speed * (float)delta * dir;
	}

	public void OnAreaEntered(Area2D a) {
		if (a is Player) OnBodyEntered(a);
	}
	
	public void OnBodyEntered(Node2D body) {
		if (body is Player player) {
			player.EmitSignal(Player.SignalName.EnemyCollided, this);
		} else if (body is Bubble b && !b.isPoppping) {
			health -= 20;
			this.BlinkWithTween();
			if (health <= 0) {
				Die();
			}
			b.Burst();
		}
	}

	public override void Die() {
		QueueFree();
	}
}
