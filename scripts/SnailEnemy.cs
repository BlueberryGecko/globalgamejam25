using Godot;
using System;
using Globalgamejam25.scripts;

public partial class SnailEnemy : Enemy
{
	[Export]
	private float maxSpeed = 500;
	[Export]
	private float acceleration = 1500;
	[Export]
	private float slowDownRadius = 300;
	private Vector2 velocity;
	
	public override void _Process(double delta)
	{
		frozenTime -= delta;
		if (frozenTime > 0) {
			return;
		}
		else {
			iceBlock.Visible = false;
		}
		
		var targetPosition = Consts.world.player.Position;
		
		var difference = targetPosition - Position;
		var distance = difference.Length();
		var direction = difference.Normalized();
		
		var accelerationMultiplier = Mathf.Min(distance / slowDownRadius, 1);

		velocity += direction * acceleration * accelerationMultiplier * (float)delta;
		velocity = velocity.LimitLength(maxSpeed);
		
		var magneticAcceleration = MagnetPuddle.GetMagneticPull(magnetizationPulls, delta, Position, this);
		velocity += magneticAcceleration; // do this after since magnetic accel can be faster than maxSpeed.
		
		Position += velocity * (float)delta;
		Rotation = direction.Angle();
	}
}
