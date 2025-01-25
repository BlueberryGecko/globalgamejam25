using Godot;
using System;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class EColiEnemy : Enemy
{
	[Export] public float speed = 100;
	
	public override void _Process(double delta)
	{
		var dir = (Consts.world.player.Position - Position).Normalized();
		Position += speed * (float)delta * dir;
	}
}
