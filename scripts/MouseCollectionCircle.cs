using Godot;
using System;
using System.Collections.Generic;

public partial class MouseCollectionCircle : Area2D {
	[Export] private Sprite2D sprite;
	[Export] private CollisionShape2D collisionShape2D;
	private const int bubbleMask = 4;
	private List<Bubble> heldBubbles;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		((CircleShape2D)collisionShape2D.Shape).Radius = sprite.Texture.GetSize().X * sprite.Scale.X; // TODO: can move this inside _Ready once we're sure of size.
	}
	
	public override void _Input(InputEvent ev) {
		if (!(ev is InputEventMouseButton button)) {
			return;
		}
		if (button.IsActionPressed("click")) {
			CollisionMask = bubbleMask;
		}
		else {
			CollisionMask = 0;
			heldBubbles.ForEach(b => b.ReleaseFromCircle());
			heldBubbles.Clear();
		}
	}

	public void OnBodyEntered(Node2D body) {
		if (body is Bubble b) {
			b.CaptureInCircle(this);
		}
	}
}
