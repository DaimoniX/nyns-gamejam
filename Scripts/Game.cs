using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GJNYS.Scripts;

public partial class Game : Node2D
{
	private Timer _timer;

	[Export] private Array<Caller> _callers = new();
	[Export] private Array<Color> _colors = new();
	[Export] private Array<Socket> _sockets = new();
	[Export] private Label _scoreLabel;

	private static readonly Color Color = new(255, 255, 255, 0f);
	private int Score
	{
		get => _score;
		set
		{
			_score = value;
			_scoreLabel.Text = $"Score: {_score}";
		}
	}
	private int _score;

	private struct ActiveCall
	{
		public int Id1;
		public int Id2;
		public bool Active;
		public const float Patience = 20f;
		public float PatienceLeft;
		public float TimeLeft;
	}

	private readonly List<ActiveCall> _calls = new();

	public override void _Ready()
	{
		_timer = new Timer();
		_timer.OneShot = true;
		_timer.Timeout += Call;
		AddChild(_timer);
		for (var i = 0; i < _colors.Count; i++)
		{
			_callers[i].SetColor(_colors[i]);
			_sockets[i].SetColor(_colors[i]);
			_sockets[i].Connected += OnSocketConnected;
		}
		Score = 0;
		Call();
	}

	private void OnSocketConnected(Socket a, Socket b)
	{
		var i1 = _sockets.IndexOf(a);
		var i2 = _sockets.IndexOf(b);
		for (var i = 0; i < _calls.Count; i++)
		{
			var call = _calls[i];
			if ((call.Id1 == i1 && call.Id2 == i2) || (call.Id1 == i2 && call.Id2 == i1))
				call.Active = true;
			_calls[i] = call;
		}
	}

	public override void _Process(double delta)
	{
		foreach (var c in _callers)
		{
			var side = c.LeftSide ? -1 : 1;
			var pos = c.GlobalPosition;
			pos.X = Mathf.Lerp(pos.X, c.Active ? 650 * side : 1000 * side, (float)delta * 4);
			c.GlobalPosition = pos;
			c.Modulate = c.Modulate.Lerp(c.Active ? Colors.White : Color, (float)delta * 4);
		}

		UpdateCalls((float)delta);
	}

	private void UpdateCalls(float delta, int i = 0)
	{
		if (_calls.All(c => c.Active))
			Call();
		for (; i < _calls.Count; i++)
		{
			var call = _calls[i];
			if (call.Active)
			{
				_callers[call.Id1].SetPatience(-1);
				_callers[call.Id2].SetPatience(-1);
				call.TimeLeft -= delta;
			}
			else
			{
				call.PatienceLeft -= delta;
				_callers[call.Id1].SetPatience(call.PatienceLeft / ActiveCall.Patience);
				_callers[call.Id2].SetPatience(call.PatienceLeft / ActiveCall.Patience);
			}
			
			if (call.TimeLeft < 0 || call.PatienceLeft < 0)
			{
				_callers[call.Id1].Active = _callers[call.Id2].Active = false;
				_sockets[call.Id1].Deactivate();
				_sockets[call.Id2].Deactivate();
				if (call.PatienceLeft >= 0)
					Score++;
				_calls.RemoveAt(i);
				UpdateCalls(delta, i);
				return;
			}
			_calls[i] = call;
		}
	}

	private void Call()
	{
		_timer.Stop();
		_timer.Start(GD.RandRange(8, 16));
		var fId = _callers.Select((_, i) => i).Where(x => !_callers[x].Active).ToArray();
		if (fId.Length < 2)
			return;

		var rfId1 = (int)(GD.Randi() % fId.Length);
		var rfId2 = (int)(GD.Randi() % fId.Length);
		
		while (rfId2 == rfId1)
			rfId2 = (int)(GD.Randi() % fId.Length);
		
		var cId1 = fId[rfId1];
		var cId2 = fId[rfId2];

		var c1 = _callers[cId1];
		var c2 = _callers[cId2];
		c1.Active = true;
		c2.Active = true;

		c1.SetColor(_colors[cId2]);
		c2.SetColor(_colors[cId1]);
		_sockets[cId1].AcceptedSocket = _sockets[cId2];
		_sockets[cId2].AcceptedSocket = _sockets[cId1];
		_sockets[cId1].Active = _sockets[cId2].Active = true;
		
		_calls.Add(new ActiveCall { Id1 = cId1, Id2 = cId2, TimeLeft = GD.RandRange(8, 16), Active = false, PatienceLeft = ActiveCall.Patience });
	}
}