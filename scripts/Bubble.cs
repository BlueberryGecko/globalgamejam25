using Godot;
using System;

public partial class Bubble : CharacterBody2D {
	[Export] private CollisionShape2D collisionShape2D;
	public Vector2 acceleration  = Vector2.Zero;
	public Vector2 randomForce = new(Random.Shared.NextSingle() * 2 - 1, Random.Shared.NextSingle() * 2 - 1);
	[Export] public float mass = 1;
	[Export] public float frictionCoeff = 0.1f;
	
	private bool manualCollision = false;
	private MouseCollectionCircle collectionCircleDuringManualCollision;

	/// <summary>
	/// Apply a force lasting a certain time delta. Will directly modify linear acceleration.
	/// </summary>
	/// <param name="force"></param>
	public void ApplyForceThisFrame(Vector2 force) {
		acceleration += force / mass;
	}
	
	public void SetAcceleration(Vector2 acceleration)
	{
		this.acceleration = acceleration;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var viewportRect = GetViewportRect().Grow(((CircleShape2D)collisionShape2D.Shape).Radius);
		if (!viewportRect.HasPoint(GlobalPosition)) {
			QueueFree();
		}
		Velocity += acceleration * (float)delta;
		// Velocity *= (1 - frictionCoeff * (float)delta);
		acceleration = Vector2.Zero;
		MoveAndSlide();
	}
	public override void _Draw() {
		base._Draw();
	}
	
	public void CaptureInCircle(MouseCollectionCircle mouseCollectionCircle) {
		manualCollision = true;
		collectionCircleDuringManualCollision = mouseCollectionCircle;
		// TODO: set collision mask, probably
	}

	public void ReleaseFromCircle() {
		manualCollision = false;
	}

	public float GetRadius() => ((CircleShape2D)collisionShape2D.Shape).Radius;
}
