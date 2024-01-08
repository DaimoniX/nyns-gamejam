using Godot;

namespace GJNYS.Scripts;

public partial class Box : RigidBody2D, IInteractable
{
    private CollisionShape2D _collider;
    private Vector2? _tp;

    public override void _Ready()
    {
        _collider = GetChild<CollisionShape2D>(0);
        FreezeMode = FreezeModeEnum.Static;
    }

    public void Interact(Player player)
    {
        Rotation = 0;
        LockRotation = true;
        Freeze = true;
        Sleeping = true;
        _collider.Disabled = true;
        player.HoldingBox = this;
    }

    public void Put(Vector2 position)
    {
        Freeze = false;
        Sleeping = false;
        GlobalPosition = position;
        _collider.Disabled = false;
        _tp = position;
        LockRotation = false;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (_tp == null) return;
        GlobalPosition = _tp.Value;
        Rotation = 0;
        _tp = null;
    }
}