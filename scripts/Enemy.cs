using System.Collections.Generic;
using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] public int maxHealth = 40;
    [Export] public int health = 40;
    [Export] private int score = 10;

	[Export] public Sprite2D iceBlock;
    [Export] public CollisionShape2D collisionShape2D;
    [Export] public Sprite2D sprite;
    [Export] public AudioStreamPlayer2D freezeAudioPlayer;
    [Export] public PackedScene magnetComponentScene;
    
	protected double frozenTime = 0;

    public bool isMagnetized = false;
    public bool isFrozen = false;
    private MagnetComponent magnetComponent;
    
	[Export] public float magneticForce = 50000;
    [Export] public float forceMaxDistance = 5;
    protected HashSet<Enemy> magnetizationPulls = new();

    [Export] public float championSpeedMultiplier = 1;
    [Export] public float championToughnessMultiplier = 1;
    [Export] public BubbleModifier championDamageImmunity;
    
    public override void _Ready() {
        health = maxHealth;
    }
    
    public override void _Process(double delta) {
        frozenTime -= delta;
        isFrozen = frozenTime > 0;
        iceBlock.Visible = isFrozen;
    }

    public void Scale(float scale) {
        iceBlock.Scale *= scale;
        collisionShape2D.Scale *= scale;
        sprite.Scale *= scale;
    }

    public void ColorAsSpeedy() {
        sprite.SelfModulate = new Color(1, 2, 3);
    }

    public void SetDamageImmunityShader(BubbleModifier immunity) {
        var shader = (ShaderMaterial)sprite.Material;
        shader.SetShaderParameter("enabled", true);
        if (immunity.HasFlag(BubbleModifier.Ice)) {
            shader.SetShaderParameter("modulateColor", new Color(0, 1, 1));
        }
        if (immunity.HasFlag(BubbleModifier.Explode)) {
            shader.SetShaderParameter("modulateColor", new Color(1, 0, 0));
        }
        if (immunity.HasFlag(BubbleModifier.Magnet)) {
            shader.SetShaderParameter("modulateColor", new Color(1, 0, 1));
        }
    }
    
    public override void OnBodyEntered(Node2D body) {
        base.OnBodyEntered(body);
        if (body is Bubble b)
            b.EnemyHit(this);
    }

    public void Freeze(Bubble b) {
        frozenTime = b.freezeTime;
        freezeAudioPlayer?.Play();
    }

    public void Magnetize() {
        isMagnetized = true;
        if (magnetComponent == null) {
            magnetComponent = magnetComponentScene.Instantiate<MagnetComponent>();
            magnetComponent.AreaEntered += PullArea;
            magnetComponent.BodyEntered += PullBody;
            magnetComponent.AreaExited += ReleaseArea;
            magnetComponent.BodyExited += ReleaseBody;
            AddChild(magnetComponent);
        }
    }

    public void Damage(int d, Bubble damagingBubble = null) {
        if (Consts.world.player.cushion || health <= 0) return;

        if ((damagingBubble?.bubbleModifier & championDamageImmunity) > 0) {
            return;
        }
        
        if (isFrozen)
            d *= 2;
        
        health -= d;
        this.BlinkWithTween();
        if (health <= 0) {
            Die();
            Consts.world.player.Score += score;
        }
    }

    private void PullArea(Area2D a) {
        if (a is Enemy e) {
            e.magnetizationPulls.Add(this);
        }
    }
    
    private void ReleaseArea(Area2D a) {
        if (a is Enemy e) {
            e.magnetizationPulls.Remove(this);
        }
    }
    
    private void PullBody(Node2D n) {
        if (n is Bubble b) {
            b.magnetizationPulls.Add(this);
        }
    }
    
    private void ReleaseBody(Node2D n) {
        if (n is Bubble b) {
            b.magnetizationPulls.Remove(this);
        }
    }
}