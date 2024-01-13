using Godot;

namespace GJNYS.Scripts;

public partial class Hp : TextureRect
{
	[Export] private Texture2D _notOk;
	[Export] private Texture2D _ok;

	public void Add()
	{
		Texture = _ok;
	}

	public void Remove()
	{
		Texture = _notOk;
	}

	public override void _Ready()
	{
		Add();
	}
}