using Godot;

namespace GJNYS.Scripts;

public partial class Caller : Node2D
{
	[Export] private ColorRect _color;
	public bool Active { get; set; }

	public override void _Ready()
	{
		Modulate = new Color(255, 255, 255, 0f);
	}

	public void SetColor(Color color)
	{
		_color.Color = color;
	}
}