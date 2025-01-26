using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class Bubble : CharacterBody2D {
	[Export] private CollisionShape2D collisionShape2D;
	[Export] private AudioStreamPlayer2D audioPlayer;
	[Export] public PackedScene explosionScene;
	public Vector2 acceleration  = Vector2.Zero;
	public Vector2 randomForce = new(Random.Shared.NextSingle() * 2 - 1, Random.Shared.NextSingle() * 2 - 1);
	[Export] public float mass = 1;
	[Export] public float frictionCoeff = 0.1f;
	[Export] public AnimatedSprite2D sprite;
	[Export] private BubbleModifierSprite[] bubbleModifierSprites;
	public int damage {
		get {
			if (bubbleModifier.HasFlag(BubbleModifier.Ice) || bubbleModifier.HasFlag(BubbleModifier.Magnet)) {
				return 0;
			} else if (bubbleModifier.HasFlag(BubbleModifier.Explode)) {
				return 40;
			}
			return 10;
		}
	}
	
	[Export] public int health = 1;
	[Export] public int frozenBubbleHealth = 4;
	public BubbleModifier bubbleModifier = 0;
	
	[Export] public double freezeTime = 3;
	
	private bool manualCollision = false;
	private MouseCollectionCircle collectionCircleDuringManualCollision;
	public bool isPoppping = false;

	public HashSet<Enemy> magnetizationPulls = new();

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
		sprite.Stop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		foreach (var bubbleModifierSprite in bubbleModifierSprites)
		{
			
			GetNode<Sprite2D>(bubbleModifierSprite.sprite).Visible = bubbleModifier.HasFlag(bubbleModifierSprite.modifier);
		}
		
		var viewportRect = GetViewportRect();
		var boundingBox = viewportRect
			.Grow(((CircleShape2D)collisionShape2D.Shape).Radius)
			.plus(Consts.world.player.Position - viewportRect.Size / 2)
			.Grow(100);
		if (!boundingBox.HasPoint(Position)) {
			QueueFree();
		}
		Velocity += acceleration * (float)delta + MagnetPuddle.GetMagneticPull(magnetizationPulls, delta, Position) / mass;
		// Velocity *= (1 - frictionCoeff * (float)delta);
		acceleration = Vector2.Zero;
		MoveAndSlide();
	}
	
	public void CaptureInCircle(MouseCollectionCircle mouseCollectionCircle) {
		manualCollision = true;
		collectionCircleDuringManualCollision = mouseCollectionCircle;
		CollisionMask = 0;
		CollisionLayer = 0;
	}

	public void ReleaseFromCircle() {
		manualCollision = false;
		CollisionMask = 1 << (int)Consts.CollisionLayers.Enemies;
		CollisionLayer = 1 << (int)Consts.CollisionLayers.Bubbles;
	}

	public float GetRadius() => ((CircleShape2D)collisionShape2D.Shape).Radius * GlobalScale.X;
	
	public void Burst() {
		health -= 1;
		if (health <= 0) {
			isPoppping = true;
			sprite.Play();
			audioPlayer.Play();
		}
	}

	public void OnAnimationFinished() {
		if (isPoppping) {
			QueueFree();
		}
	}
	
	public void EnemyHit(Enemy enemy)
	{
		if (isPoppping)
			return;

		if (bubbleModifier.HasFlag(BubbleModifier.Ice) && enemy.isFrozen)
			return;
		
		ApplyModifier(enemy);
		if (bubbleModifier.HasFlag(BubbleModifier.Explode)) 
			ExplodeBubble();
		enemy.Damage(damage);
		Burst();
	}

	public void ExplodeBubble()
	{
		var explosion = explosionScene.Instantiate<Explosion>();
		explosion.Position = Position;
		explosion.bubble = this;
		Consts.world.AddChild(explosion);
	}
	
	public void ApplyModifier(Enemy enemy) {
		if ((bubbleModifier & BubbleModifier.Ice) != 0) {
			enemy.Freeze(this);
		}
		if ((bubbleModifier & BubbleModifier.Magnet) != 0) {
			enemy.Magnetize();
		}
	}
}

[Flags]
public enum BubbleModifier {
	Ice = 1,
	Magnet = 2,
	Explode = 4,
	Bounce = 8,
	Pierce = 16,
	Electric = 32
}