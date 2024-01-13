using Godot;

namespace GJNYS.Scripts;

public partial class Player : CharacterBody2D
{
	[Export] private Area2D _interactionZone;
	[Export] private Sprite2D _character;
	
	[Export] private float _speed = 300.0f;
	[Export] private float _jumpVelocity = 600.0f;
	[Export] private RayCast2D _putChecker;
	
	private bool _grounded;
	public Box HoldingBox { get; set; }
	private readonly float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private const float CoyoteTime = 0.15f;
	private float _coyote = 0.1f;
	
	public Socket ConnectedSocket { get; set; }

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("interact"))
			Interact();
	}

	private readonly Vector2 _downForce = Vector2.Down * 5000f;

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		
		var input = Input.GetAxis("left", "right");
		
		velocity.X = Mathf.MoveToward(velocity.X, input * _speed, _speed * (float)delta * 5f);

		if (!IsOnFloor())
		{
			_coyote -= (float)delta;
			velocity.Y += _gravity * 1.5f * (float)delta;
		}
		else
			_coyote = CoyoteTime;
		
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || _coyote > 0))
			velocity.Y = -_jumpVelocity;
		
		_character.FlipH = input switch
		{
			> 0 when !_character.FlipH => true,
			< 0 when _character.FlipH => false,
			_ => _character.FlipH
		};

		Velocity = velocity;
		
		var c = MoveAndCollide(Velocity * (float)delta);
		if (c?.GetCollider() is RigidBody2D r && _interactionZone.GetOverlappingBodies().Contains(r))
			r.ApplyCentralImpulse(new Vector2(Velocity.X / r.Mass, 0));
		MoveAndSlide();

		if (HoldingBox != null)
			HoldingBox.GlobalPosition = GlobalPosition + Vector2.Up + Vector2.Right * (_character.FlipH ? 25 : -25);
	}

	private void Interact()
	{
		if (HoldingBox != null)
		{
			_putChecker.TargetPosition = Vector2.Right * (_character.FlipH ? 60 : -60);
			_putChecker.ForceRaycastUpdate();
			if(_putChecker.IsColliding()) return;
			HoldingBox.Put(GlobalPosition + Vector2.Up + Vector2.Right * (_character.FlipH ? 50 : -50));
			HoldingBox = null;
			return;
		}
		
		var iBod = _interactionZone.GetOverlappingBodies();
		foreach (var b in iBod)
			if (b is IInteractable i)
			{
				i.Interact(this);
				return;
			}
		
		if(HoldingBox != null) return;
		var iObj = _interactionZone.GetOverlappingAreas();
		foreach (var io in iObj)
			if (io.GetParent() is IInteractable i)
			{
				i.Interact(this);
				return;
			}
	}
}