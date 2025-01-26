using System.Collections.Generic;
using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] private int maxHealth = 40;
    [Export] private int health = 40;

	[Export] public Sprite2D iceBlock;
    [Export] public PackedScene magnetComponentScene;
	protected double frozenTime = 0;

    public bool isMagnetized = false;
    private MagnetComponent magnetComponent;
    
	[Export] public float magneticForce = 500;
    [Export] public float forceMaxDistance = 10;
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
			} else if ((b.bubbleModifier & BubbleModifier.Magnet) != 0) {
                isMagnetized = true;
                if (magnetComponent == null) {
                    var mc = magnetComponentScene.Instantiate<MagnetComponent>();
                    mc.AreaEntered += PullArea;
                    mc.BodyEntered += PullBody;
                    mc.AreaExited += ReleaseArea;
                    mc.BodyExited += ReleaseBody;
                    AddChild(mc);
                }
            }
            health -= b.damage;
            this.BlinkWithTween();
            if (health <= 0) {
                Die();
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