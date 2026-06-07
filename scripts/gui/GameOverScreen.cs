using Godot;
using System;

public partial class GameOverScreen : CanvasLayer
{
	private Label _titleLabel;
	private Label _timeLabel;
	private Label _levelLabel;
	private Button _restartButton;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;

		_titleLabel    = GetNode<Label>("Panel/VBoxContainer/TitleLabel");
		_timeLabel     = GetNode<Label>("Panel/VBoxContainer/TimeLabel");
		_levelLabel    = GetNode<Label>("Panel/VBoxContainer/LevelLabel");
		_restartButton = GetNode<Button>("Panel/VBoxContainer/RestartButton");

		_restartButton.Text = "Menu Principal";
		_restartButton.Pressed += OnGoToMenu;
		
		EventManager.Instance.PlayerDied -= ShowGameOver;
		EventManager.Instance.GameWon    -= ShowWin;
		
		EventManager.Instance.PlayerDied += ShowGameOver;
		EventManager.Instance.GameWon    += ShowWin;
	}
	
	public override void _ExitTree()
	{
		EventManager.Instance.PlayerDied -= ShowGameOver;
		EventManager.Instance.GameWon    -= ShowWin;
	}
	
	private void ShowGameOver() => Show("GAME OVER");
	private void ShowWin()      => Show("SOBREVIVISTE!");

	private void Show(string title)
	{
		Visible = true;
		GetTree().Paused = true;
		
		_titleLabel.Text = title;
		
		int seconds = (int)GameManager.Instance.ElapsedTime;
		_timeLabel.Text  = $"Tiempo: {seconds / 60:D2}:{seconds % 60:D2}";
		_levelLabel.Text = $"Nivel alcanzado: {GameManager.Instance.CurrentLevel}";
	}

	private void OnGoToMenu()
	{
		GameManager.Instance.ResetRun();
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
	}
}
