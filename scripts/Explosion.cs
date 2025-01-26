using Globalgamejam25;
using Godot;
using Globalgamejam25.scripts;

public partial class Explosion : Area2D {
	[Export] public int damage = 40;
	public Bubble bubble;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(Area2D a) {
		if (a is Enemy e) {
			e.Damage(damage);
			bubble.ApplyModifier(e);
		}
		if (a is Player p) {
			p.damage(damage);
		}
	}
	
	public void OnBodyEntered(Node2D a) {
		if (a is Bubble e) {
			e.QueueFree();
		}
	}

	public void OnAnimationFinished() {
		QueueFree();
	}
}
