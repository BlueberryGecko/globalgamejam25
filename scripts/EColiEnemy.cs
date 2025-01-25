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
		frozenTime -= delta;
		if (frozenTime > 0) {
			return;
		}
		else {
			iceBlock.Visible = false;
		}
		var dir = (Consts.world.player.Position - Position).Normalized();
		Position += speed * (float)delta * dir;
	}

	public void OnAreaEntered(Area2D a) {
		if (a is Player) OnBodyEntered(a);
	}

	public override void Die() {
		QueueFree();
	}
}
