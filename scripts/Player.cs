using System.Collections.Generic;
using Globalgamejam25.scripts;
using Godot;

namespace Globalgamejam25;

public partial class Player : Area2D {
    public Vector2 velocity;
    private Vector2 lastMovement;
    private float rotationSpeed;
    [Export]
    private float maxSpeed = 300;
    [Export]
    private float maxDashSpeed = 1000;
    [Export]
    private float dashCooldown = 3f;
    [Export]
    private float dashDuration = 0.5f;
    [Export]
    private float dashAcceleration = 3000;
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
    [Export]
	private GameOver gameOver;
    [Export] private Sprite2D sprite;
    [Export] private AudioStreamPlayer2D damageAudioPlayer;
    [Export] private AudioStreamPlayer2D dashAudioPlayer;
    [Export] private AudioStreamPlayer2D eatSoapPieceAudioPlayer;

    private List<Sprite2D> eyes = new();
    private bool godmode = false;

	[Export] public Label scoreLabel;

	private int score = 0;
	public int Score {
		get => score;
		set {
			score = value;
			scoreLabel.Text = score.ToString();
		}
	}

    public bool cushion = false;
    
    private List<float> superChargeSpawnTimerModifiers = new();
    [Export] private float superChargeFactor = 0.5f;
    [Export] private float superChargeTime = 2f;

    [Export]
    private Sprite2D stripeSprite;
    private float dashTimer = int.MaxValue;
    private float dashDurationTimer = int.MaxValue;

    [Signal] public delegate void EnemyCollidedEventHandler(Enemy enemy);

    [Export] public ProgressBar healthBar;
    [Export] public int maxHealth = 100;
    [Export] public int health = 100;
    [Export] public Camera2d camera;

    public override void _Ready() {
        healthBar.MaxValue = maxHealth;
        healthBar.Value = health;
        eyes.Add(GetNode<Sprite2D>("eye1"));
        eyes.Add(GetNode<Sprite2D>("eye2"));
    }

    public override void _Process(double delta) {
        if (dashTimer < dashCooldown)
            dashTimer += (float)delta;
        
        if (dashDurationTimer < dashDuration)
            dashDurationTimer += (float)delta;
        
        if (Input.IsActionPressed("dash") && dashTimer >= dashCooldown)
        {
            dashTimer = 0;
            dashDurationTimer = 0;
            dashAudioPlayer.Play();
        }
        
        var dash = dashDurationTimer < dashDuration;
        stripeSprite.Visible = dash;
        stripeSprite.GlobalPosition = Position - velocity.Normalized() * 20;
        stripeSprite.GlobalRotation = velocity.Normalized().Angle();
        
        var move = new Vector2();

        if (Input.IsActionPressed("move_up"))
            move.Y -= 1;
        if (Input.IsActionPressed("move_down"))
            move.Y += 1;
        if (Input.IsActionPressed("move_left"))
            move.X -= 1;
        if (Input.IsActionPressed("move_right"))
            move.X += 1;
        if (Input.IsActionJustPressed("godmode"))
            godmode = !godmode;
        if (Input.IsActionJustPressed("nodamage"))
            cushion = !cushion;

        if (lastMovement != Vector2.Zero) {
            var difference = move.Angle() - lastMovement.Angle();
            if (difference > Mathf.Pi)
                difference -= Mathf.Tau;
            else if (difference < -Mathf.Pi)
                difference += Mathf.Tau;

            rotationSpeed += difference * rotationAcceleration;
        }

        lastMovement = move;

        move = move.Normalized() * (dash ? dashAcceleration : acceleration) * (float)delta;

        if ((move.X >= 0 && velocity.X <= 0) || (move.X <= 0 && velocity.X >= 0))
            move.X += -velocity.X / deceleration * (float)delta;
        if ((move.Y >= 0 && velocity.Y <= 0) || (move.Y <= 0 && velocity.Y >= 0))
            move.Y += -velocity.Y / deceleration * (float)delta;

        velocity += move;
        velocity = velocity.LimitLength(dash ? maxDashSpeed : maxSpeed);
        var supercharge = UpdateSupercharge(delta);
        var speedPercent = velocity.Length() / maxSpeed;
        if (IsInstanceValid(bubbleSpawner))
            bubbleSpawner.spawnTimerMultiplier = speedPercent;
            bubbleSpawner.spawnTimerMultiplier *= supercharge;
            
        velocity *= 1 + (supercharge-1) / 2.0f;
        
        Position += velocity * (float)delta;

        RotationDegrees += rotationSpeed * (float)delta;
        rotationSpeed -= rotationSpeed * rotationDeceleration * (float)delta;

    }
    
    private float UpdateSupercharge(double delta) {
        float supercharge = 1;
        for (int i = 0; i < superChargeSpawnTimerModifiers.Count; i++) {
            superChargeSpawnTimerModifiers[i] -= (float)delta;
            double remainingSuperChargeRatio = Mathf.Min(superChargeSpawnTimerModifiers[i] / delta, 1);
            supercharge += superChargeFactor * (float)remainingSuperChargeRatio;
            
            if (superChargeSpawnTimerModifiers[i] <= 0) {
                superChargeSpawnTimerModifiers.RemoveAt(i);
                i--;
            }
        }
        eyes.ForEach(e => e.Modulate = new Color(1, 1, 1, (float)(supercharge-1)));
        double playerModulate = Mathf.Max(supercharge, 1);
        sprite.Modulate = new Color((float)playerModulate, (float)playerModulate, (float)playerModulate);
        return supercharge;
    }

    public void OnEnemyCollided(DamageEntity e) {
        damage(e.damage);
        e.Die();
    }

    public void damage(int d) {
        if (godmode) return;
        health -= d;
        this.BlinkWithTween();
        damageAudioPlayer.Play();
        healthBar.Value = health;
        if (health < 0) {
            gameOver.Initiate();
        }
        camera.AddShakeDamage(d);
    }

    public void OnArea2DEntered(Area2D area) {
        if (area is SoapParticle s) {
            superChargeSpawnTimerModifiers.Add(superChargeTime);
            s.QueueFree();
            eatSoapPieceAudioPlayer.Play();
            if (health < maxHealth) 
            {
                health += 2;
            }
            Score += 5;
        }
    }
}