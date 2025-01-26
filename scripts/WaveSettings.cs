using Godot;

namespace Globalgamejam25.scripts;

[GlobalClass]
public partial class WaveSettings : Resource
{
    [Export]
    public bool isActive;
    
    [Export]
    public float spawnInterval;

    [Export] public float spawnSpeedChampionChance;
    [Export] public float spawnToughChampionChance;
    [Export] public float spawnTypeImmuneChampionChance;
}
