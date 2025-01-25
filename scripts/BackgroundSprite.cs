using Godot;
using System;
using Globalgamejam25;

public partial class BackgroundSprite : Sprite2D
{
	[Export]
	private Camera2D camera;

	private float tileSize;
	
	public override void _Ready()
	{
		tileSize = Texture.GetSize().X * Scale.X;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RegionRect = GetViewportRect().Grow(1 / Scale.X).plus(Vector2.One * tileSize);

		if (!IsInstanceValid(camera))
			return;
		
		var cameraCenterPos = camera.GlobalPosition - GetViewportRect().Size / 2;
		var backgroundOffsetPos = (cameraCenterPos / tileSize).Floor();
		Position = backgroundOffsetPos * tileSize;
		if (Material is ShaderMaterial shaderMaterial)
		{
			shaderMaterial.SetShaderParameter("NoiseOffset", backgroundOffsetPos);
		}
	}
}
