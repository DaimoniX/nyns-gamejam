using Godot;

namespace GJNYS.Scripts;

public partial class Player : RigidBody2D
{
	public override void _Process(double delta)
	{
		var input = Input.GetAxis("ui_left", "ui_right");
		var f = ConstantForce;
		f.X = input * 200f * LinearDamp * Mass;
		ConstantForce = f;
		if(Input.IsActionJustPressed("ui_accept"))
			ApplyImpulse(Vector2.Up * Mass * LinearDamp * 250f);
	}
}