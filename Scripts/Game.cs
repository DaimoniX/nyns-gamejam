using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace GJNYS.Scripts;

public partial class Game : Node2D
{
	private Timer _timer;

	[Export]
	private Array<Caller> _leftSlots = new();
	[Export]
	private Array<Caller> _rightSlots = new();
	[Export]
	private Array<Color> _colors = new();
	[Export]
	private Array<Socket> _sockets = new();

	private static readonly Color Color = new(255, 255, 255, 0f);
	private int _freePairs;

	struct ActiveCall
	{
		public int Caller1;
		public int Caller2;
		public float TimeLeft;
	}

	private List<ActiveCall> _calls = new();

	public override void _Ready()
	{
		_timer = new Timer();
		_timer.OneShot = true;
		_timer.Timeout += Call;
		AddChild(_timer);
		Call();
		_freePairs = _leftSlots.Count / 2;
		for(var i = 0; i < _colors.Count; i++)
			_sockets[i].SetColor(_colors[i]);
	}

	public override void _Process(double delta)
	{
		foreach (var lc in _leftSlots)
		{
			var pos = lc.GlobalPosition;
			pos.X = Mathf.Lerp(pos.X,  lc.Active ? -650 : -1000, (float)delta * 2);
			lc.GlobalPosition = pos;
			lc.Modulate = lc.Modulate.Lerp(lc.Active ? Colors.White : Color, (float)delta * 2);
		}
		
		foreach (var rc in _rightSlots)
		{
			var pos = rc.GlobalPosition;
			pos.X = Mathf.Lerp(pos.X,  rc.Active ? 650 : 1000, (float)delta * 2);
			rc.GlobalPosition = pos;
			rc.Modulate = rc.Modulate.Lerp(rc.Active ? Colors.White : Color, (float)delta * 2);
		}

		UpdateCalls((float)delta);
	}

	private void UpdateCalls(float delta, int i = 0)
	{
		for (; i < _calls.Count; i++)
		{
			var call = _calls[i];
			call.TimeLeft -= delta;
			if (call.TimeLeft < 0)
			{
				_calls.RemoveAt(i);
				_leftSlots[call.Caller1].Active = false;
				_rightSlots[call.Caller2].Active = false;
				UpdateCalls(delta, i);
				return;
			}
			_calls[i] = call;
		}
	}

	private void Call()
	{
		var lc = (int)(GD.Randi() % _leftSlots.Count);
		var rc = (int)(GD.Randi() % _rightSlots.Count);
		var lac = _leftSlots[lc];
		var rac = _rightSlots[rc];
		lac.Active = true;
		rac.Active = true;
		lac.SetColor(_colors[4 + lc]);
		rac.SetColor(_colors[4 + rc]);
		_timer.Start(GD.RandRange(10, 20));
		_calls.Add(new ActiveCall { Caller1 = lc, Caller2 = rc, TimeLeft = GD.RandRange(5, 15) });
	}
}