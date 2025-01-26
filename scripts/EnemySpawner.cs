using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class EnemySpawner : Node2D
{
	[Export]
	private PackedScene enemy;

	[Export]
	private WaveSettings[] waves;
	
	private float spawnTimer;
	
	public override void _Process(double delta)
	{
		var waveIndex = Mathf.Min(Consts.world.CurrentWaveIndex, waves.Length - 1);
		var waveSettings = waves[waveIndex];
		
		if (!waveSettings.isActive)
			return;
		
		spawnTimer += (float)delta;
		var spawnCount = Mathf.Floor(spawnTimer / waveSettings.spawnInterval);
		for (var i = 0; i < spawnCount; i++)
			SpawnEnemy(waveSettings);
		spawnTimer %= waveSettings.spawnInterval;
	}

	private void SpawnEnemy(WaveSettings waveSettings)
	{
		var validSides = new List<Vector2>{Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right};
		validSides = validSides.Where(s =>
		{
			var diff = Consts.world.player.velocity * s;
			return diff.X > 1 || diff.Y > 1;
		}).ToList();

		if (!validSides.Any())
		{
			validSides = new List<Vector2>{Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right};
		}
		
		var randomSide = validSides[Random.Shared.Next(0, validSides.Count)];
		
		var spawnRect = Util.GetViewBorderRect();

		var randomSideStart = (randomSide.Swap() + randomSide) * spawnRect.Size + spawnRect.Position;
		var randomSideEnd = (-randomSide.Swap() + randomSide) * spawnRect.Size + spawnRect.Position;
		
		var randomPos = randomSideStart.Lerp(randomSideEnd, Random.Shared.NextSingle());
		
		var spawnedEnemy = enemy.Instantiate<Enemy>();
		Consts.world.AddChild(spawnedEnemy);
		spawnedEnemy.GlobalPosition = randomPos;

		if (Random.Shared.NextSingle() < waveSettings.spawnTypeImmuneChampionChance) {
			switch (Random.Shared.Next(2)) {
				case 0:
					spawnedEnemy.championDamageImmunity = BubbleModifier.Ice;
					spawnedEnemy.SetDamageImmunityShader(BubbleModifier.Ice);
					break;
				case 1:
					spawnedEnemy.championDamageImmunity = BubbleModifier.Magnet;
					spawnedEnemy.SetDamageImmunityShader(BubbleModifier.Magnet);
					break;
				case 2:
					spawnedEnemy.championDamageImmunity = BubbleModifier.Explode;
					spawnedEnemy.SetDamageImmunityShader(BubbleModifier.Explode);
					break;
			}
		}
		if (Random.Shared.NextSingle() < waveSettings.spawnToughChampionChance) {
			spawnedEnemy.maxHealth *= 3;
			spawnedEnemy.health *= 3;
			spawnedEnemy.Scale(1.5f);
		}
		if (Random.Shared.NextSingle() < waveSettings.spawnSpeedChampionChance) {
			spawnedEnemy.championSpeedMultiplier = 1.7f;
			spawnedEnemy.Scale(1.1f);
			spawnedEnemy.ColorAsSpeedy();
		}
	}
}
