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
    [Export]
    private AudioStreamPlayer2D audioPlayer;
    private Vector2 randomTargetOffset = new(Random.Shared.NextSingle() * 2 - 1, Random.Shared.NextSingle() * 2 - 1);
    
    private float timer;
    private Vector2 magneticVelocity = Vector2.Zero; // track this separately so it can accumulate over multiple frames
    [Export] public float magneticFriction = 0.9f; // small friction term so magnetic influence doesn't last forever.
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isFrozen)
            return;
        
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
        var velocity = Mathf.Min(speed * (float)delta, distance) * dir;
        magneticVelocity *= (float)(magneticFriction * delta);
        magneticVelocity += MagnetPuddle.GetMagneticPull(magnetizationPulls, delta, Position, this);
        Position += velocity + magneticVelocity * (float)delta * championSpeedMultiplier;
    }

    private void Shoot()
    {
        var bullet = bulletScene.Instantiate<Bullet>();
        Consts.world.AddChild(bullet);
        bullet.GlobalPosition = Position;
        bullet.direction = (Consts.world.player.Position - Position).Normalized();
        if (IsInstanceValid(audioPlayer))
            audioPlayer.Play();
    }
}
