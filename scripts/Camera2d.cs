using Godot;
using System;

public partial class Camera2d : Camera2D {
	[Export] public float maxRotation = 0.1f;
	[Export] public Vector2 maxShakeOffset = new Vector2(100, 75);
	[Export] public float shakeDecay = 0.8f;
	[Export] public float currentShake = 0f;
	[Export] public float shakePower = 2f;
	
	[Export] public float maxDamageShake = 40f;
	private Random rand = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentShake -= shakeDecay * (float)delta;
		currentShake = Mathf.Clamp(currentShake, 0f, 1);
		var shake = Mathf.Pow(currentShake, shakePower);
		Rotation = maxRotation * (2 * rand.NextSingle() - 1) * shake;
		Offset = new Vector2(maxShakeOffset.X * (2 * rand.NextSingle() - 1), maxShakeOffset.Y * (2 * rand.NextSingle() - 1)) * shake;
	}

	public void AddShakeDamage(int damage) {
		currentShake += damage / maxDamageShake;
	}
}
