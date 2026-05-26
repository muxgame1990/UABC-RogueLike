using Godot;
using System;

public partial class PauseScreen : CanvasLayer
{
	private BaseButton resumeButton;
	private BaseButton configButton;
	private BaseButton exitButton;

	private bool isPaused = false;

	public override void _Ready()
	{
		Visible = false;
		resumeButton = GetNode<BaseButton>("MainPanel/ReanudarButton");
		configButton = GetNode<BaseButton>("MainPanel/ConfigButton");
		exitButton = GetNode<BaseButton>("MainPanel/ExitButton");

		resumeButton.Pressed += Resume;
		configButton.Pressed += Config;
		exitButton.Pressed += Exit;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			togglePaused();
		}
	}

	private void togglePaused()
	{
		if(isPaused)
			Resume();
		else
			PauseGame();
	}

	private void PauseGame()
	{
		isPaused = true;
		Visible = true;
		GetTree().Paused = true;
		EventManager.Instance.EmitGamePaused();
		GD.Print("Juego pausado");
	}

	private void Resume()
	{
		isPaused = false;
		Visible = false;
		GetTree().Paused = false;
		GD.Print("Juego reanudado");
	}

	private void Config()
	{
		GD.Print("Boton de configuracion");
	}

	private void Exit()
	{
		GetTree().Paused = false;
		GameManager.Instance.ResetRun();
		GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
	}
}
