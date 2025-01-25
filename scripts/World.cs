using Godot;
using System;
using System.Collections.Generic;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class World : Node2D
{
	[Export] PackedScene worldScene;
	[Export] public Player player;
	[Export] private PackedScene[] enemies;
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
		var viewportRect = GetViewportRect();
		var spawnRect = viewportRect.plus(player.Position - viewportRect.Size / 2).Grow(50); // spawn enemy slightly offscreen
		
		var enemy = enemies[Random.Shared.Next(0, enemies.Length)].Instantiate<Enemy>();
		
		AddChild(enemy);
		enemy.GlobalPosition = Util.samplePointOnRect(spawnRect);
	}
	
	private enum EnemyType {
		EColi,
		DirtThrower,
	}
}
