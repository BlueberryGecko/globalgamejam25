using Godot;
using System;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class EColiEnemy : Enemy
{
	[Export] private int maxHealth = 40;
	[Export] private int health = 40;
	[Export] public float speed = 100;
	
    private Vector2 magneticVelocity = Vector2.Zero; // track this separately so it can accumulate over multiple frames
    [Export] public float magneticFriction = 0.9f; // small friction term so magnetic influence doesn't last forever.
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		health = maxHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (isFrozen)
			return;
		
        magneticVelocity *= (float)(magneticFriction * delta);
        magneticVelocity += MagnetPuddle.GetMagneticPull(magnetizationPulls, delta, Position, this);
		  
		var dir = (Consts.world.player.Position - Position).Normalized();
		Position += speed * (float)delta * dir + magneticVelocity * (float)delta;
	}

	public void OnAreaEntered(Area2D a) {
		if (a is Player) OnBodyEntered(a);
	}

	public override void Die() {
		QueueFree();
	}
}
