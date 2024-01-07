using Godot;

namespace GJNYS.Scripts;

public partial class Player : RigidBody2D
{
	[Export] private RayCast2D _ground;
	[Export] private Area2D _interactionZone;
	private bool _grounded;
	public Socket ConnectedSocket { get; set; }

	public override void _Process(double delta)
	{
		var input = Input.GetAxis("left", "right");
		var f = ConstantForce;
		f.X = input * 200f * LinearDamp * Mass;
		ConstantForce = f;
		if(Input.IsActionJustPressed("jump") && _grounded)
			ApplyImpulse(Vector2.Up * Mass * LinearDamp * 250f);
		if(Input.IsActionJustPressed("interact"))
			Interact();
	}

	public override void _PhysicsProcess(double delta)
	{
		_ground.ForceRaycastUpdate();
		_grounded = _ground.IsColliding();
		LinearDamp = _grounded ? 4f : 1f;
	}

	private void Interact()
	{
		var iObj = _interactionZone.GetOverlappingAreas();
		foreach (var io in iObj)
			if(io.GetParent() is IInteractable i)
				i.Interact(this);
	}
}