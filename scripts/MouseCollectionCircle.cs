using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Globalgamejam25.scripts;
using Microsoft.VisualBasic;

public partial class MouseCollectionCircle : Area2D {
	[Export] private Sprite2D sprite;
	[Export] private CollisionShape2D collisionShape2D;
	[Export] private float bubbleReadjustmentForce = 10000;
	[Export] private float borderRepellantForce = 1000;
	[Export] private float borderRepellantMidwayDistanceCutoff = 0.5f;
	[Export] private Color deactivatedModulate = new(1, 1, 1, 0.5f);
	[Export] private AudioStreamPlayer2D audioPlayer;

	private Vector2[] lastMousePositions = new Vector2[5];
	
	private const int bubbleMask = 1 << (int)Consts.CollisionLayers.Bubbles;
	
	/// <summary>
	/// list of potentially disposed bubbles. If you use this, you'll hate your life.
	/// </summary>
	private List<Bubble> potentiallyDisposedHeldBubbles = new();
	private IEnumerable<Bubble> heldBubbles => potentiallyDisposedHeldBubbles.Where(IsInstanceValid);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Modulate = deactivatedModulate;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		((CircleShape2D)collisionShape2D.Shape).Radius = sprite.Texture.GetSize().X * sprite.GlobalScale.X / 2; // TODO: can move this inside _Ready once we're sure of size.

		if (CollisionMask == bubbleMask) {
			RecaptureBubbles(delta);
		}
		for (var i = lastMousePositions.Length - 1; i >= 1; i--)
		{
			lastMousePositions[i] = lastMousePositions[i - 1];
		}
		lastMousePositions[0] = Position;
		Position = GetGlobalMousePosition();
		
		if (Input.IsActionJustPressed("click")) {
			CollisionMask = bubbleMask;
			Modulate = Colors.White;
		}
		if (Input.IsActionJustReleased("click")) {
			CollisionMask = 0;
			var positionsDeltas = new List<Vector2>{Position - lastMousePositions[0]};
			for (var i = 0; i < lastMousePositions.Length - 1; i++)
			{
				positionsDeltas.Add(lastMousePositions[i] - lastMousePositions[i + 1]);
			}
			var positionDelta = new Vector2(positionsDeltas.Average(d => d.X), positionsDeltas.Average(d => d.Y));
			var exitSpeed = positionDelta / (float)delta;
			var exitSpeedStrength = exitSpeed.Length();
			if (exitSpeedStrength > 10 && heldBubbles.Any())
				audioPlayer.Play();
			heldBubbles.ToList().ForEach(b =>
			{
				if (exitSpeedStrength > 10f)
				{
					b.SetAcceleration(Vector2.Zero);
					b.Velocity = exitSpeed + b.randomForce * exitSpeedStrength * 0.1f;
				}
				b.ReleaseFromCircle();
			});
			potentiallyDisposedHeldBubbles.Clear();
			Modulate = deactivatedModulate;
		}
	}

	private void RecaptureBubbles(double delta) {
		ClampBubbles(delta);
		ApplyInwardForce(delta);
	}

	/// <summary>
	/// Apply small inward force; stronger if the bubble is right on the border. Assumes that all <see cref="heldBubbles"/> are already inside the collection circle.
	/// </summary>
	/// <param name="delta"></param>
	private void ApplyInwardForce(double delta) {
		foreach (var heldBubble in heldBubbles) {
			var maxPossibleDist = GetRadius() - heldBubble.GetRadius();
			var actualDist = (heldBubble.GlobalPosition - Position).Length();
			var ratio = actualDist / maxPossibleDist;
			if (ratio < borderRepellantMidwayDistanceCutoff) {
				ratio = 0; // don't apply a force to bubbles that are past halfway distance towards the center.
	 		}
			var f = ratio * borderRepellantForce;
			var randomForce = heldBubble.randomForce * Mathf.Max(f, 10) * 0.75f;
			heldBubble.ApplyForceThisFrame( f * (Position - heldBubble.GlobalPosition).Normalized() + randomForce);
		}
	}
	
	private void ClampBubbles(double delta) {
		foreach (var heldBubble in heldBubbles) {
			var bubbleRadius = heldBubble.GetRadius();
			var posDelta = heldBubble.GlobalPosition - Position;
			var bubbleDir = posDelta.Normalized();
			var radiusDiff = GetRadius() - heldBubble.GetRadius();
			if (posDelta.LengthSquared() > radiusDiff * radiusDiff) {
				var oldHeldBubblePos = heldBubble.GlobalPosition;
				heldBubble.GlobalPosition = Position + bubbleDir * (radiusDiff - 1);
				heldBubble.ApplyForceThisFrame((heldBubble.GlobalPosition - oldHeldBubblePos) * bubbleReadjustmentForce);
			}
		}
	}

	public void OnBodyEntered(Node2D body) {
		if (body is Bubble b && IsInstanceValid(b)) {
			b.CaptureInCircle(this);
			potentiallyDisposedHeldBubbles.Add(b);
		}
	}
	
	public float GetRadius() => ((CircleShape2D)collisionShape2D.Shape).Radius;
}
