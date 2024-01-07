using Godot;

namespace GJNYS.Scripts;

public partial class Socket : Node2D, IInteractable
{
	private Line2D _line;
	private Node2D _player;
	private Socket _connectedSocket;
	
	public override void _Ready()
	{
		_line = new Line2D();
		_line.Width = 4;
		_line.TopLevel = true;
		_line.AddPoint(GlobalPosition);
		_line.AddPoint(GlobalPosition);
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
		if(_connectedSocket != null) return;
		_player = null;
		_connectedSocket = socket;
		_connectedSocket.ConnectSocket(this);
		_line.SetPointPosition(1, socket.GlobalPosition);
	}

	public override void _Process(double delta)
	{
		if(_player == null) return;
		_line.SetPointPosition(1, _player.GlobalPosition);
	}
}
