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

    [Signal] public delegate void EnemyCollidedEventHandler(Enemy enemy);

    [Export] public ProgressBar healthBar;
    [Export] public int maxHealth = 100;
    [Export] public int health = 100;

    public override void _Ready() {
        healthBar.MaxValue = maxHealth;
        healthBar.Value = health;
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
}