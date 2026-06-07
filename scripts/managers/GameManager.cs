using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	
	//[Signal] public delegate void PlayerDiedEventHandler();
	//[Signal] public delegate void GameWonEventHandler();
	//[Signal] public delegate void LevelUpEventHandler();

	public int   CurrentLevel          { get; set; } = 1;
	public float CurrentXp             { get; set; } = 0f;
	public float XpToNextLevel         { get; set; } = 100f;
	public float ElapsedTime           { get; set; } = 0f;
	public float WinTime               { get; }       = 900f;
	public int   Coins                 { get; set; } = 0;
	public string      SelectedCharacterScene { get; set; } = "res://scenes/player/cimarron_char.tscn";
	public int         SelectedCharacterIndex { get; set; } = 0;
	public int         SelectedClassIndex     { get; set; } = 0;
	public PassiveType SelectedPassive        { get; set; } = PassiveType.None;
	public float diffModifier = 1f;
	public float eliteProbability = 0.3f;
	public override void _EnterTree() => Instance = this;

	public override void _Process(double delta)
	{
		ElapsedTime += (float)delta;
		if (ElapsedTime >= WinTime)
			EventManager.Instance.EmitGameWon();
	}

	public void AddXp(float amount)
	{
		CurrentXp += amount * 1f; // aquí se puede aplicar XpMultiplier del PlayerStats
		if (CurrentXp >= XpToNextLevel)
		{
			CurrentXp     -= XpToNextLevel;
			CurrentLevel++;
			XpToNextLevel *= 1.2f;
			EventManager.Instance.EmitLevelUp(CurrentLevel);
		}
		EventManager.Instance.EmitXpChanged(CurrentXp,XpToNextLevel);
	}
	public void AddCoins(int amount)
	{
		Coins += amount;
		EventManager.Instance.EmitCoinChanged(amount);
	}

	public void ResetRun()
	{
		CurrentLevel = 1;
		CurrentXp = 0f;
		Coins = 0;
		XpToNextLevel = 100f;
		ElapsedTime   = 0f;
		Coins         = 0;
	}
}
