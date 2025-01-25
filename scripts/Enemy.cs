using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] private int maxHealth = 40;
    [Export] private int health = 40;

	[Export] public Sprite2D iceBlock;
	protected double frozenTime = 0;
    
    public override void _Ready() {
        health = maxHealth;
    }
    
    public override void OnBodyEntered(Node2D body) {
        base.OnBodyEntered(body);
        if (body is Bubble b && !b.isPoppping) {
			if ((b.bubbleModifier & BubbleModifier.Ice) != 0) {
				frozenTime = b.freezeTime;
				iceBlock.Visible = true;
			}
            health -= b.damage;
            this.BlinkWithTween();
            if (health <= 0) {
                Die();
            }
            b.Burst();
        }
    }
}