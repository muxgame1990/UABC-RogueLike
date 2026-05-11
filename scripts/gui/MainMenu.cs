using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _exitButton;
	private TextureRect _background;

	public override void _Ready()
	{
		_playButton  = GetNode<Button>("CenterContainer/VBox/PlayButton");
		_exitButton  = GetNode<Button>("CenterContainer/VBox/ExitButton");
		_background  = GetNode<TextureRect>("Background");
		
		_playButton.Pressed += OnPlay;
		_exitButton.Pressed += OnExit;
	}

	private void OnPlay()
	{
		GetTree().ChangeSceneToFile("res://scenes/ui/character_select.tscn");
	}
	
	private void OnExit()
	{
		GetTree().Quit();
	}
}
