using Godot;
using System;
using Globalgamejam25;

public partial class BubbleSpawner : Node2D {
	[Export] private PackedScene bubbleScene;
	[Export] private float randomBubbleSpeedMax = 10;
	[Export] private float randomBubbleSpeedMin = 5;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTimerTimeout() {
		var bubble = bubbleScene.Instantiate<Bubble>();
		GetTree().Root.GetNode("World").AddChild(bubble);
		bubble.Position = GlobalPosition;
		var randomDir = new Vector2(1, 0).Rotated(Util.getRandomFloat(Mathf.Tau));
		bubble.LinearVelocity = randomDir * Util.getRandomFloat(randomBubbleSpeedMin, randomBubbleSpeedMax);
	}
}
