using System;
using Godot;

public partial class IcePuddle : Puddle
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void OnBubbleEntered(Bubble b) {
		b.bubbleModifier |= BubbleModifier.Ice;
		b.sprite.SetAnimation("frozen (the movie by Disney)");
		b.sprite.Stop();
	}
}