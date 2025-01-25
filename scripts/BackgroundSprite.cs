using Godot;
using System;

public partial class BackgroundSprite : Sprite2D
{
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RegionRect = GetViewportRect().Grow(1 / Scale.X);
	}
}
