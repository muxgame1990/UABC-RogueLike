using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

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
	
	//estado entre mapas
	public List<WeaponData> SavedWeapons           { get; set; } = null;
	public float            SavedCurrentHp         { get; set; } = -1f;
	public PlayerStats      SavedStats             { get; set; } = null;
	public float            SavedPassiveDamageBonus { get; set; } = 1f;
	public bool             HasSavedState          => SavedWeapons != null;
	
	public override void _EnterTree() => Instance = this;

	public override void _Process(double delta)
	{
		ElapsedTime += (float)delta;
		if (ElapsedTime >= WinTime)
			EventManager.Instance.EmitGameWon();
	}
	
	public void SavePlayerState(Player player){ //guardar estado del jugador
		var weaponManager = player.GetNodeOrNull<WeaponManager>("WeaponManager");
		if (weaponManager != null){
			SavedWeapons = weaponManager.GetActiveWeapons();
		}
		
		SavedCurrentHp = player.CurrentHp;
		SavedStats = player.Stats;
		SavedPassiveDamageBonus = player.PassiveDamageBonus;
	}

	public void AddXp(float amount)
	{
		CurrentXp += amount * 500f; // aquí se puede aplicar XpMultiplier del PlayerStats | original x1
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
		
		//limpiar estado al reiniciar
		SavedWeapons = null;
		SavedCurrentHp = -1f;
		SavedStats = null;
		SavedPassiveDamageBonus = 1f;
	}
}
