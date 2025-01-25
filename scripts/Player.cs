using System.Collections.Generic;
using Globalgamejam25.scripts;
using Godot;

namespace Globalgamejam25;

public partial class Player : Area2D {
    private Vector2 velocity;
    private Vector2 lastMovement;
    private float rotationSpeed;
    [Export]
    private float maxSpeed = 300;
    [Export]
    private float acceleration = 1500;
    [Export]
    private float deceleration = 1f / 5f;
    [Export]
    private float rotationAcceleration = 100;
    [Export]
    private float rotationDeceleration = 1f / 1.1f;
    [Export]
    private BubbleSpawner bubbleSpawner;
    [Export] private Sprite2D sprite;

    
    private List<Sprite2D> eyes = new();
    
    private List<float> superChargeSpawnTimerModifiers = new();
    [Export] private float superChargeFactor = 0.5f;
    [Export] private float superChargeTime = 2f;

    [Signal] public delegate void EnemyCollidedEventHandler(Enemy enemy);

    [Export] public ProgressBar healthBar;
    [Export] public int maxHealth = 100;
    [Export] public int health = 100;

    public override void _Ready() {
        healthBar.MaxValue = maxHealth;
        healthBar.Value = health;
        eyes.Add(GetNode<Sprite2D>("eye1"));
        eyes.Add(GetNode<Sprite2D>("eye2"));
    }

    public override void _Process(double delta) {
        var move = new Vector2();

        if (Input.IsActionPressed("move_up"))
            move.Y -= 1;
        if (Input.IsActionPressed("move_down"))
            move.Y += 1;
        if (Input.IsActionPressed("move_left"))
            move.X -= 1;
        if (Input.IsActionPressed("move_right"))
            move.X += 1;

        if (lastMovement != Vector2.Zero) {
            var difference = move.Angle() - lastMovement.Angle();
            if (difference > Mathf.Pi)
                difference -= Mathf.Tau;
            else if (difference < -Mathf.Pi)
                difference += Mathf.Tau;

            rotationSpeed += difference * rotationAcceleration;
        }

        lastMovement = move;

        move = move.Normalized() * acceleration * (float)delta;

        if ((move.X >= 0 && velocity.X <= 0) || (move.X <= 0 && velocity.X >= 0))
            move.X += -velocity.X / deceleration * (float)delta;
        if ((move.Y >= 0 && velocity.Y <= 0) || (move.Y <= 0 && velocity.Y >= 0))
            move.Y += -velocity.Y / deceleration * (float)delta;

        velocity += move;
        velocity = velocity.LimitLength(maxSpeed);
        var speedPercent = velocity.Length() / maxSpeed;
        if (IsInstanceValid(bubbleSpawner))
            bubbleSpawner.spawnTimerMultiplier = speedPercent;
        
        Position += velocity * (float)delta;

        RotationDegrees += rotationSpeed * (float)delta;
        rotationSpeed -= rotationSpeed * rotationDeceleration * (float)delta;

        UpdateSupercharge(delta);
    }
    
    private void UpdateSupercharge(double delta) {
        var baseSpawnModifier = bubbleSpawner.spawnTimerMultiplier;
        double superChargeModifier = 1;
        for (int i = 0; i < superChargeSpawnTimerModifiers.Count; i++) {
            superChargeSpawnTimerModifiers[i] -= (float)delta;
            double remainingSuperChargeRatio = Mathf.Min(superChargeSpawnTimerModifiers[i] / delta, 1);
            superChargeModifier += superChargeFactor * remainingSuperChargeRatio;
            
            if (superChargeSpawnTimerModifiers[i] <= 0) {
                superChargeSpawnTimerModifiers.RemoveAt(i);
                i--;
            }
        }
        bubbleSpawner.spawnTimerMultiplier *= (float)superChargeModifier;
        eyes.ForEach(e => e.Modulate = new Color(1, 1, 1, (float)(superChargeModifier-1)));
        double playerModulate = Mathf.Max(superChargeModifier, 1);
        sprite.Modulate = new Color((float)playerModulate, (float)playerModulate, (float)playerModulate);
    }

    public void OnEnemyCollided(DamageEntity e) {
        health -= e.damage;
        this.BlinkWithTween();
        healthBar.Value = health;
        if (health < 0) {
            Consts.world.Restart();
        }
        e.Die();
    }

    public void OnArea2DEntered(Area2D area) {
        if (area is SoapParticle s) {
            superChargeSpawnTimerModifiers.Add(superChargeTime);
            s.QueueFree();
        }
    }
}