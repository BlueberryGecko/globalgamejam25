using Godot;
using System;
using System.Collections.Generic;
using Globalgamejam25.scripts;

public partial class MagnetPuddle : Puddle {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void OnBubbleEntered(Bubble b) {
		b.bubbleModifier |= BubbleModifier.Magnet;
		b.damage = 0;
		b.sprite.SetAnimation("magnetic");
		b.sprite.Stop();
	}

	public static Vector2 GetMagneticPull(HashSet<Enemy> pulls, double delta, Vector2 atPosition) {
		var totalAccel = Vector2.Zero;
		foreach (var e in pulls) {
			totalAccel += e.magneticForce / (0.5f*Mathf.Max((atPosition - e.Position).LengthSquared(), e.forceMaxDistance * e.forceMaxDistance)) *
			              (e.Position - atPosition).Normalized();
		}
		return totalAccel * (float)delta;
	}
}
