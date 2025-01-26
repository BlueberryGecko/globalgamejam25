using Godot;

namespace Globalgamejam25.scripts;

[GlobalClass]
public partial class BubbleModifierSprite : Resource
{
    [Export]
    public NodePath sprite;
    [Export]
    public BubbleModifier modifier;
}