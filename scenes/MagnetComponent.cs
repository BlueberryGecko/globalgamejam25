using Godot;
using System;
using Globalgamejam25.scripts;

public partial class MagnetComponent : Area2D
{
	[Export] private AnimationPlayer animationPlayer;
	
	public override void _Ready()
	{
		animationPlayer.Play("magnetEffect");
	}
}
