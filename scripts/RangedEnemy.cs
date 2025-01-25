using System;
using Godot;

namespace Globalgamejam25.scripts;

public partial class RangedEnemy : Enemy
{
    [Export]
    public float speed = 150;
    [Export]
    private float shootInterval = 3f;
    [Export]
    private float distanceToPlayer = 50f;
    [Export]
    private PackedScene bulletScene;
    [Export]
    private float randomTargetOffsetMax = 10;
    private Vector2 randomTargetOffset = new(Random.Shared.NextSingle() * 2 - 1, Random.Shared.NextSingle() * 2 - 1);
    
    private float timer;
    
    public override void _Process(double delta)
    {
        timer += (float)delta;
        var spawnCount = Mathf.Floor(timer / shootInterval);
        for (var i = 0; i < spawnCount; i++)
            Shoot();
        timer %= shootInterval;
        
        var targetPosition = Consts.world.player.Position + (Position - Consts.world.player.Position).Normalized() * distanceToPlayer;
        targetPosition += randomTargetOffset * randomTargetOffsetMax;
        
        var difference = targetPosition - Position;
        var distance = difference.Length();
        if (distance < 5f)
            return;
        
        var dir = difference.Normalized();
        Position += Mathf.Min(speed * (float)delta, distance) * dir;
    }

    private void Shoot()
    {
        var bullet = bulletScene.Instantiate<Bullet>();
        Consts.world.AddChild(bullet);
        bullet.GlobalPosition = Position;
        bullet.direction = (Consts.world.player.Position - Position).Normalized();
    }
}
