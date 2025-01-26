using Godot;

namespace Globalgamejam25.scripts;

public abstract partial class Enemy : DamageEntity {
    [Export] private int maxHealth = 40;
    [Export] private int health = 40;
    [Export] private int score = 10;

	[Export] public Sprite2D iceBlock;
    [Export] public AudioStreamPlayer2D freezeAudioPlayer;
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
                freezeAudioPlayer.Play();
			}
            health -= b.damage;
            this.BlinkWithTween();
            if (health <= 0) {
                Die();
                Consts.world.player.score += score;
            }
            b.Burst();
        }
    }
}