using Godot;
using System;

public partial class Bubble : RigidBody2D {
	[Export] private CollisionShape2D collisionShape2D;
	
	private bool manualCollision = false;
	private MouseCollectionCircle collectionCircleDuringManualCollision;
	 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var viewportRect = GetViewportRect().Grow(((CircleShape2D)collisionShape2D.Shape).Radius);
		if (!viewportRect.HasPoint(Position)) {
			QueueFree();
		}
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
