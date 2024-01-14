using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GJNYS.Scripts;

public partial class Game : Node2D
{
	[Export] private Control _gg;
	[Export] private Array<Caller> _callers = new();
	[Export] private Array<Color> _colors = new();
	[Export] private Array<Socket> _sockets = new();
	[Export] private Label _scoreLabel;
	private static readonly Color Color = new(255, 255, 255, 0f);
	[Export] private PackedScene _healthItem;
	[Export] private HBoxContainer _healthContainer;
	[Export] private int _health = 3;
	[Export] private AudioStreamPlayer _audioPlayer;
	[Export] private AudioStream _scoreAudio;
	
	private int Score
	{
		get => _score;
		set
		{
			_score = value;
			_audioPlayer.Stream = _scoreAudio;
			_audioPlayer.Play();
			_scoreLabel.Text = $"Score: {_score}";
		}
	}
	private int _score;

	private int Health
	{
		get => _health;
		set
		{
			_gg.Visible = value == 0;
			if(_health > value)
				_hps[_health - 1].Call("damage");
			else if(_health < value)
				_hps[_health].Call("heal");
			_health = value;
		}
	}
	
	private Array<Control> _hps = new();
	private Timer _timer;

	private struct ActiveCall
	{
		public int Id1;
		public int Id2;
		public bool Active;
		public Color CallColor;
		public static float Patience = 20f;
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
		
		for (var i = 0; i < _callers.Count; i++)
		{
			_callers[i].SetColor(Color);
			_sockets[i].SetColor(Color);
			_sockets[i].Connected += OnSocketConnected;
		}

		for (var i = 0; i < _health; i++)
		{
			var hp = _healthItem.Instantiate<Control>();
			_hps.Add(hp);
			_healthContainer.AddChild(hp);
		}
		
		Score = 0;
		Call();
		_gg.Visible = false;
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
		if(_health < 1) return;
		
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
				
				_sockets[call.Id1].SetColor(Color);
				_sockets[call.Id2].SetColor(Color);
				
				_sockets[call.Id1].Deactivate();
				_sockets[call.Id2].Deactivate();
				if (call.PatienceLeft >= 0)
				{
					ActiveCall.Patience = Mathf.Clamp(ActiveCall.Patience - 0.4f, 6.5f, 20f);
					Score++;
				}
				else
					Health--;
				_calls.RemoveAt(i);
				UpdateCalls(delta, i);
				return;
			}
			_calls[i] = call;
		}
	}

	private void Call()
	{
		if(_health < 1) return;
		_timer.Stop();
		_timer.Start(GD.RandRange(8, 16) - _score * 0.1f);
		var fId = _callers.Select((_, i) => i).Where(x => !_callers[x].Active).ToArray();
		if (fId.Length < 2)
			return;

		var fColors = _colors.Where(c => _calls.All(call => call.CallColor != c)).ToArray();

		var color = fColors[(int)(GD.Randi() % fColors.Length)];
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

		c1.SetColor(color);
		c2.SetColor(color);
		
		_sockets[cId1].SetColor(color);
		_sockets[cId2].SetColor(color);
		
		_sockets[cId1].AcceptedSocket = _sockets[cId2];
		_sockets[cId2].AcceptedSocket = _sockets[cId1];
		_sockets[cId1].Active = _sockets[cId2].Active = true;
		
		_calls.Add(new ActiveCall { Id1 = cId1, Id2 = cId2, TimeLeft = GD.RandRange(8, 16) - _score * 0.1f, Active = false, PatienceLeft = ActiveCall.Patience, CallColor = color });
	}
}