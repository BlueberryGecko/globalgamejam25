using Godot;

namespace Globalgamejam25.scripts;

[GlobalClass]
public partial class WaveSettings : Resource
{
    [Export]
    public bool isActive;
    
    [Export]
    public float spawnInterval;
}
