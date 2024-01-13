using Godot;

namespace GJNYS.Scripts;

public partial class Socket : Node2D, IInteractable
{
	[Signal]
	public delegate void ConnectedEventHandler(Socket a, Socket b);
	private Line2D _line;
	private Player _player;
	private Socket _connectedSocket;
	[Export] private Texture2D _ropeTexture;
	[Export] private PointLight2D _pointLight;
	public bool Active { get; set; }
	public Socket AcceptedSocket { get; set; }

	public void SetColor(Color color)
	{
		_pointLight.Color = color;
	}
	
	public override void _Ready()
	{
		_line = new Line2D();
		_line.Width = 48;
		_line.TopLevel = true;
		_line.AddPoint(GlobalPosition);
		_line.AddPoint(GlobalPosition);
		_line.Texture = _ropeTexture;
		_line.ZIndex++;
		_line.TextureMode = Line2D.LineTextureMode.Tile;
		AddChild(_line);
	}

	public void Interact(Player player)
	{
		if(_connectedSocket != null || !Active) return;
		
		if (player.ConnectedSocket == this)
		{
			player.ConnectedSocket = null;
			_player = null;
			_line.SetPointPosition(1, GlobalPosition);
		}
		else if (player.ConnectedSocket != null)
		{
			if(player.ConnectedSocket == AcceptedSocket)
				ConnectSocket(player.ConnectedSocket);
		}
		else
		{
			_player = player;
			player.ConnectedSocket = this;
		}
	}

	private void ConnectSocket(Socket socket)
	{
		if(socket == this || _connectedSocket != null || !Active) return;
		if (_player != null)
		{
			_player.ConnectedSocket = null;
			_line.SetPointPosition(1, socket.GlobalPosition);
		}
		_player = null;
		_connectedSocket = socket;
		_connectedSocket.ConnectSocket(this);
		EmitSignal(SignalName.Connected, this, _connectedSocket);
	}

	public void Deactivate()
	{
		Active = false;
		if (_connectedSocket != null)
		{
			var cs = _connectedSocket;
			_connectedSocket = null;
			cs.Deactivate();
		}
		if (_player != null)
		{
			if(_player.ConnectedSocket == this)
				_player.ConnectedSocket = null;
			_player = null;
		}
		_line.SetPointPosition(1, GlobalPosition);
	}

	public override void _Process(double delta)
	{
		if(_player == null) return;
		_line.SetPointPosition(1, _player.GlobalPosition);
	}
}
