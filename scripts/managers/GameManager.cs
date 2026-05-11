using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void GameWonEventHandler();
	[Signal] public delegate void LevelUpEventHandler();

	public int CurrentLevel { get; set; } = 1;
	public float CurrentXp { get; set; } = 0f;
	public float XpToNextLevel { get; set; } = 100f;
	public float ElapsedTime { get; set; } = 0f;
	public float WinTime { get; } = 900f;
	public int Coins {get; set; } = 0;

	public string SelectedCharacterScene { get; set; } = "res://scenes/player/cimarron_char.tscn";
	public string DefaultWeaponScene {get; set; } = "res://scenes/weapons/pencil_weapon.tscn";
	public int SelectedCharacterIndex { get; set; } = 0;

	public override void _EnterTree() => Instance = this;

	public override void _Process(double delta)
	{
		ElapsedTime += (float)delta;
		if (ElapsedTime >= WinTime)
			EmitSignal(SignalName.GameWon);
	}

	public void AddXp(float amount)
	{
		CurrentXp += amount;
		if (CurrentXp >= XpToNextLevel)
		{
			CurrentXp -= XpToNextLevel;
			CurrentLevel++;
			XpToNextLevel *= 1.2f;
			EmitSignal(SignalName.LevelUp);
		}
	}
	
	public void AddCoins(int amount)
	{
		Coins += amount;
	}

	public void ResetRun()
	{
		CurrentLevel = 1;
		CurrentXp = 0f;
		XpToNextLevel = 100f;
		ElapsedTime = 0f;
	}
}
