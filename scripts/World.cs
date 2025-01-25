using Godot;
using System;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class World : Node2D
{
	[Export] PackedScene worldScene;
	[Export] public Player player;
	[Export] private PackedScene eColi;
	[Export] public AudioStreamPlayer2D audioPlayer;
	
	public override void _Ready()
	{
		QueueRedraw();
		Consts.world = this;
	}
	
	public override void _Process(double delta)
	{
	}
	
	public void Restart() {
		GetTree().ChangeSceneToPacked(worldScene);
	}

	public void OnEnemySpawnTimerTimeout() {
		var spawnRect = GetViewportRect().plus(player.Position).Grow(50); // spawn enemy slightly offscreen
		
		var newEColi = eColi.Instantiate<EColiEnemy>();
		newEColi.Position = Util.samplePointOnRect(spawnRect);
		AddChild(newEColi);
	}
}
