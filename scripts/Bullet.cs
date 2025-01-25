using Godot;

namespace Globalgamejam25.scripts;

public partial class Bullet : DamageEntity
{
    public Vector2 direction;
    [Export]
    public float speed = 100;
    
    public override void _Process(double delta)
    {
        Rotation = direction.Angle();
        
        var viewportRect = GetViewportRect();
        var boundingBox = viewportRect
            .plus(Consts.world.player.Position - viewportRect.Size / 2)
            .Grow(100);
        if (!boundingBox.HasPoint(Position)) {
            QueueFree();
        }
        
        Position += speed * (float)delta * direction;
    }
}
