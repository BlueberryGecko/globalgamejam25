using Godot;

public abstract partial class Puddle : Area2D {
    public void OnBodyEntered(Node2D other) {
        if (other is Bubble b) {
            OnBubbleEntered(b);
        }
        // maybe some more interactions with player? idk.
    }
    
    public abstract void OnBubbleEntered(Bubble b);
}