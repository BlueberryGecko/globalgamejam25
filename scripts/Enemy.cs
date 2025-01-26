using System.Collections.Generic;
using Godot;
using Vector2 = System.Numerics.Vector2;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] private int maxHealth = 40;
    [Export] private int health = 40;
    [Export] private int score = 10;

	[Export] public Sprite2D iceBlock;
    [Export] public AudioStreamPlayer2D freezeAudioPlayer;
    [Export] public PackedScene magnetComponentScene;
	protected double frozenTime = 0;

    public bool isMagnetized = false;
    private MagnetComponent magnetComponent;
    
	[Export] public float magneticForce = 50000;
    [Export] public float forceMaxDistance = 5;
    protected HashSet<Enemy> magnetizationPulls = new();
    
    public override void _Ready() {
        health = maxHealth;
    }
    
    public override void OnBodyEntered(Node2D body) {
        base.OnBodyEntered(body);
        if (body is Bubble b && !b.isPoppping) {
			if ((b.bubbleModifier & BubbleModifier.Ice) != 0) {
				frozenTime = b.freezeTime;
				iceBlock.Visible = true;
                freezeAudioPlayer?.Play();
			}
			if ((b.bubbleModifier & BubbleModifier.Magnet) != 0) {
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
            if (!Consts.world.player.cushion) {
                health -= b.damage;
            }
            this.BlinkWithTween();
            if (health <= 0) {
                Die();
                Consts.world.player.score += score;
            }
            b.Burst();
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