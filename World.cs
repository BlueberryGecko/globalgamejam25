using Godot;
using System;

public partial class World : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		QueueRedraw();
	}
	public override void _Draw() {
		base._Draw();
		DrawRect(GetViewportRect(), Colors.DarkRed, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
