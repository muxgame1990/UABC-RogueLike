using Godot;
using System;
using System.Collections.Generic;

public partial class PauseScreen : CanvasLayer
{
	private BaseButton resumeButton;
	private BaseButton configButton;
	private BaseButton exitButton;
	private TextureRect[] weaponIconSlots;
	private bool isPaused = false;
	private Label statsLabel;

	public override void _Ready()
	{
		Visible = false;
		resumeButton = GetNode<BaseButton>("MainPanel/ReanudarButton");
		configButton = GetNode<BaseButton>("MainPanel/ConfigButton");
		exitButton = GetNode<BaseButton>("MainPanel/ExitButton");
		resumeButton.Pressed += Resume;
		configButton.Pressed += Config;
		exitButton.Pressed += Exit;
		weaponIconSlots = new TextureRect[]
		{
			GetNode<TextureRect>("MainPanel/InventoryVBox/InventoryPanel/WeaponsHBox/FirstWeapon"),
			GetNode<TextureRect>("MainPanel/InventoryVBox/InventoryPanel/WeaponsHBox/SecondWeapon"),
			GetNode<TextureRect>("MainPanel/InventoryVBox/InventoryPanel/WeaponsHBox/ThirdWeapon"),
			GetNode<TextureRect>("MainPanel/InventoryVBox/InventoryPanel/WeaponsHBox/FourthWeapon")
		};
		statsLabel = GetNode<Label>("MainPanel/VBoxContainer/StatsPanel/StatsText");
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
		UpdateInventoryUI();
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
	private void UpdateInventoryUI(){
		Player player = GetTree().GetFirstNodeInGroup("player") as Player;
		if(player == null) return;
		UpdateStats(player.Stats);
		UpdateWeaponsIcons(player);
	}
	private void UpdateStats(PlayerStats stats){
		if(statsLabel == null) return;
		string text = "";
		text += $"Vida Max: {stats.MaxHp}\n";
		text += $"Regen: {stats.HpRegen}/s\n";
		text += $"Daño Fisico: +{Mathf.RoundToInt(stats.MeleeDamageBonus * 100)}%\n";
		text += $"Daño Magico: +{Mathf.RoundToInt(stats.MagicDamageBonus * 100)}%\n";
		text += $"Vel. Ataque: x{stats.AttackSpeedMultiplier:F2}\n";
		text += $"Vel. Mov: x{stats.MovementSpeedMult:F2}\n";
		text += $"Proyectiles: +{stats.BonusProjectiles}\n";
		text += $"Suerte: {stats.Luck}";
		statsLabel.Text = text;
	}
	private void UpdateWeaponsIcons(Player player){
		foreach (var slot in weaponIconSlots){
			slot.Texture = null;
		}
		WeaponManager wm = player.GetNode<WeaponManager>("WeaponManager");
		if(wm == null) return;
		List<WeaponData> activeWeapons = wm.GetActiveWeapons();
		for (int i = 0; i < activeWeapons.Count; i++)
		{
			if (i >= weaponIconSlots.Length) break; 
			string path = activeWeapons[i].IconPath;
			
			if (!string.IsNullOrEmpty(path))
			{
				weaponIconSlots[i].Texture = GD.Load<Texture2D>(path);
			}
		}
	}
}
