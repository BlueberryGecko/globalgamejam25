using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Globalgamejam25.scripts;
using Microsoft.VisualBasic;

public partial class MouseCollectionCircle : Area2D {
	[Export] private Sprite2D sprite;
	[Export] private CollisionShape2D collisionShape2D;
	[Export] private float bubbleReadjustmentImpulse = 50;
	[Export] private Color deactivatedModulate = new(1, 1, 1, 0.5f);
	
	private const int bubbleMask = 1 << (int)Consts.CollisionLayers.Bubbles;
	private List<Bubble> heldBubbles = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Modulate = deactivatedModulate;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		((CircleShape2D)collisionShape2D.Shape).Radius = sprite.Texture.GetSize().X * sprite.Scale.X; // TODO: can move this inside _Ready once we're sure of size.

		if (CollisionMask == bubbleMask) {
			RecaptureBubbles();
		}
		Position = GetGlobalMousePosition();
	}

	private void RecaptureBubbles() {
		foreach (var heldBubble in heldBubbles.Where(IsInstanceValid)) {
			var bubbleRadius = heldBubble.GetRadius();
			var posDelta = heldBubble.Position - Position;
			var bubbleDir = posDelta.Normalized();
			var radiusDiff = GetRadius() - heldBubble.GetRadius();
			if (posDelta.LengthSquared() > radiusDiff * radiusDiff) {
				heldBubble.Position = Position + bubbleDir * radiusDiff;
				heldBubble.ApplyImpulse(-posDelta * bubbleReadjustmentImpulse);
			}

		}
	}
	
	public override void _Input(InputEvent ev) {
		if (!(ev is InputEventMouseButton button)) {
			return;
		}
		if (button.IsActionPressed("click")) {
			CollisionMask = bubbleMask;
			Modulate = Colors.White;
		}
		else {
			CollisionMask = 0;
			heldBubbles.Where(IsInstanceValid).ToList().ForEach(b => b.ReleaseFromCircle());
			heldBubbles.Clear();
			Modulate = deactivatedModulate;
		}
	}

	public void OnBodyEntered(Node2D body) {
		if (body is Bubble b && IsInstanceValid(b)) {
			b.CaptureInCircle(this);
			heldBubbles.Add(b);
		}
	}
	
	public float GetRadius() => ((CircleShape2D)collisionShape2D.Shape).Radius;
}
