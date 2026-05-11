using Godot;
using System;

public partial class Hud : CanvasLayer
{
	private TextureProgressBar _hpBar;
	private ProgressBar _xpBar;
	private Label _levelLabel;
	private Label _timerLabel;
	private Label _coinsLabel;
	private Player _player;

	public override void _Ready()
	{
		_hpBar = GetNode<TextureProgressBar>("HpBar");
		_xpBar = GetNode<ProgressBar>("XpBar");
		_levelLabel = GetNode<Label>("LevelLabel");
		_timerLabel = GetNode<Label>("TimerLabel");
		_coinsLabel = GetNode<Label>("CoinsLabel");
		
		_player = GetTree().GetFirstNodeInGroup("player") as Player;
	}

	public override void _Process(double delta)
	{
		//buscar al jugador aun si no esta
		if (_player == null)
		{
			_player = GetTree().GetFirstNodeInGroup("player") as Player;
			return;
		}

		_hpBar.MaxValue = _player.MaxHp;
		_hpBar.Value    = _player.CurrentHp;

		_xpBar.MaxValue  = GameManager.Instance.XpToNextLevel;
		_xpBar.Value     = GameManager.Instance.CurrentXp;

		_levelLabel.Text = $"Lvl {GameManager.Instance.CurrentLevel}";
		_coinsLabel.Text = $"{GameManager.Instance.Coins}"; //

		int seconds      = (int)GameManager.Instance.ElapsedTime;
		_timerLabel.Text = $"{seconds / 60:D2}:{seconds % 60:D2}";
	}
}
