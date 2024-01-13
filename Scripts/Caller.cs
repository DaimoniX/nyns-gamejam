using Godot;

namespace GJNYS.Scripts;

public partial class Caller : Node2D
{
	[Export] private ColorRect _color;
	[Export] private TextureProgressBar _patience;
	public bool Active { get; set; }
	public bool LeftSide { get; private set; }

	public override void _Ready()
	{
		Modulate = new Color(255, 255, 255, 0f);
		LeftSide = GlobalPosition.X < 0;
	}

	public void SetColor(Color color)
	{
		_color.Color = color;
	}

	public void SetPatience(float val)
	{
		_patience.Visible = val > 0;
		_patience.Value = val;
	}
}