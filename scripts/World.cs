using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using Globalgamejam25;
using Globalgamejam25.scripts;

public partial class World : Node2D
{
	[Export] PackedScene worldScene;
	[Export] PackedScene soapParticleScene;
	[Export] public Player player;
	[Export] private EnemySpawner[] enemySpawners;
	[Export] public int spawnTileSize = 100;
	[Export] public int spawnTileLimit = 3;
	
	[Export(hintString: "Maps from C# puddle class name (e.g. IcePuddle) to its PackedScene")]
	public PackedScene[] puddleScenes;
	
	private Dictionary<(int, int), PuddleSpawnTracker> spawnTiles = new();
	private record PuddleSpawnTracker(int maxPuddlesInTile, List<PackedScene> puddlesToSpawn);
	[Export] private int minPuddlesInTile = 1;
	[Export] private int maxPuddlesInTile = 5;
	[Export] private float spawnMisfiringChance = 0.95f;
	
	[Export] private double soapParticleSpawnTimer = 1;
	[Export] private double soapParticleSpawnTimerMax = 1;
	[Export] private int waveCount = 10;
		
	[Export] private Label waveLabel;
	[Export] private Label waveTimeLabel;
	[Export] private Timer waveTimer;

	public int CurrentWaveIndex {
		get => currentWaveIndex;
		set {
			currentWaveIndex = value;
			waveLabel.Text = $"WAVE: {(currentWaveIndex + 1).ToString(),2}";
		}
	}
	private int currentWaveIndex;
	
	public override void _Ready()
	{
		QueueRedraw();
		Consts.world = this;
	}
	
	public override void _Process(double delta)
	{
		QueueRedraw();
		SpawnSoapParticles(delta);
		SpawnPuddles();
		UpdateWaveTimeLabel();
	}
	
	private void SpawnSoapParticles(double delta) {
		soapParticleSpawnTimer -= delta;
		if (soapParticleSpawnTimer > 0) return;

		soapParticleSpawnTimer = soapParticleSpawnTimerMax;
		
		var viewport = player.GetViewBorderRect();
		var spawnRect = viewport.Grow(viewport.Size.X / 2);
		var soapSpawnPos = Util.getRandomPosition(spawnRect);
		var soap = soapParticleScene.Instantiate<SoapParticle>();
		soap.Position = soapSpawnPos;
		AddChild(soap);
	}

	private void UpdateWaveTimeLabel()
	{
		waveTimeLabel.Text = $"Next: {Math.Round(waveTimer.TimeLeft).ToString().PadLeft(2, '0')}s";
	}

	public void Restart() {
		GetTree().ChangeSceneToPacked(worldScene);
	}

	public void OnWaveTimerTimeout() {
		if (CurrentWaveIndex < waveCount - 1)
			CurrentWaveIndex++;
	}

	public override void _Draw() {
		// var spawnBorder = player.GetViewBorderRect().Grow(100);
		// var borderTiles = WalkBorder(spawnBorder);
		// foreach (var borderTile in borderTiles) {
		// 	var pos = new Vector2(borderTile.Item1, borderTile.Item2) * spawnTileSize;
		// 	var rect = new Rect2(pos, spawnTileSize, spawnTileSize);
		// 	DrawRect(rect, Colors.Red);
		// }
	}

	public void SpawnPuddles() {
		var r = new Random();
		var spawnBorder = player.GetViewBorderRect().Grow(100);
		var borderTiles = WalkBorder(spawnBorder);
		foreach (var borderTile in borderTiles) {
			/* We do some unnecessary work here: It would be enough to just continue if the key already exists, and to spawn the puddles all at once if not.
			 * Then the duplicate checking logic would get simpler.
			 * Doing it this way, though, gives us the flexibility of changing the PuddleSpawnTracker's max puddle count while the game is running.
			 * Good for balancing, if more puddles should spawn toward the end.
			 */
			if (!spawnTiles.TryGetValue(borderTile, out var alreadySpawnedInfo)) {
				int puddlesInTile = r.Next(minPuddlesInTile, maxPuddlesInTile);
				alreadySpawnedInfo = new(puddlesInTile, puddleScenes.ToList());
				spawnTiles[borderTile] = alreadySpawnedInfo;
			}
			if (alreadySpawnedInfo.puddlesToSpawn.Count >= alreadySpawnedInfo.maxPuddlesInTile || alreadySpawnedInfo.puddlesToSpawn.Count == 0) {
				continue;
			}

			var i = r.Next(alreadySpawnedInfo.puddlesToSpawn.Count);
			var randomPuddle = alreadySpawnedInfo.puddlesToSpawn[i];
			alreadySpawnedInfo.puddlesToSpawn.RemoveAt(i);

			if (r.NextSingle() < spawnMisfiringChance) {
				continue; // if spawn misfires, don't create a puddle (note that we've already removed it from the spawnable puddles!)
			}
				
			var puddle = randomPuddle.Instantiate<Puddle>();
			var borderTilePos = new Vector2(borderTile.Item1, borderTile.Item2) * spawnTileSize;
			var spawnRect = new Rect2(borderTilePos, spawnTileSize, spawnTileSize).Grow(-20);
			var spawnPos = Util.getRandomPosition(spawnRect);
			puddle.Position = spawnPos;
			AddChild(puddle);
		}
	}

	private List<(int, int)> WalkBorder(Rect2 rect) {
		var borderTiles = new List<(int, int)>();
		var upperLeft = rect.Position / spawnTileSize;
		Vector2I upperLeftWholeCoords = new((int)Math.Floor(upperLeft.X), (int)Math.Floor(upperLeft.Y));
		var bottomRight = (rect.Position + rect.Size) / spawnTileSize;
		Vector2I bottomRightWholeCoords = new((int)Math.Floor(bottomRight.X), (int)Math.Floor(bottomRight.Y));
		borderTiles.AddRange(Util.OneToN(upperLeftWholeCoords.X, bottomRightWholeCoords.X).Select(x => (x, upperLeftWholeCoords.Y)).ToList());
		borderTiles.AddRange(Util.OneToN(upperLeftWholeCoords.Y, bottomRightWholeCoords.Y).Select(y => (bottomRightWholeCoords.X, y)).ToList());
		borderTiles.AddRange(Util.NToOne(bottomRightWholeCoords.X, upperLeftWholeCoords.X).Select(x => (x, bottomRightWholeCoords.Y)).ToList());
		borderTiles.AddRange(Util.NToOne(bottomRightWholeCoords.Y, upperLeftWholeCoords.Y).Select(y => (upperLeftWholeCoords.X, y)).ToList());
		return borderTiles;
	}
}
