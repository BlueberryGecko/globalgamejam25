using Godot;
using System;
using Globalgamejam25.scripts;

public partial class MagnetComponent : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(Area2D a) {
		if (a is Enemy e) {
		}
	}
}
