using Godot;

namespace GJNYS.Scripts;

public partial class Socket : Node2D, IInteractable
{
	private Line2D _line;
	private Player _player;
	private Socket _connectedSocket;
	[Export] private Texture2D _ropeTexture;

	public void SetColor(Color color)
	{
		Modulate = color;
	}
	
	public override void _Ready()
	{
		_line = new Line2D();
		_line.Width = 48;
		_line.TopLevel = true;
		_line.AddPoint(GlobalPosition);
		_line.AddPoint(GlobalPosition);
		_line.Texture = _ropeTexture;
		_line.TextureMode = Line2D.LineTextureMode.Tile;
		AddChild(_line);
	}

	public void Interact(Player player)
	{
		if(_connectedSocket != null) return;
		if(player.ConnectedSocket != null)
			ConnectSocket(player.ConnectedSocket);
		else
		{
			_player = player;
			player.ConnectedSocket = this;
		}
	}

	private void ConnectSocket(Socket socket)
	{
		if(socket == this || _connectedSocket != null) return;
		if (_player != null)
		{
			_player.ConnectedSocket = null;
			_line.SetPointPosition(1, socket.GlobalPosition);
		}
		_player = null;
		_connectedSocket = socket;
		_connectedSocket.ConnectSocket(this);
	}

	public void Deactivate()
	{
		if(_connectedSocket == null) return;
		_connectedSocket.Deactivate();
		_connectedSocket = null;
		_line.SetPointPosition(1, GlobalPosition);
	}

	public override void _Process(double delta)
	{
		if(_player == null) return;
		_line.SetPointPosition(1, _player.GlobalPosition);
	}
}
