using Godot;
using System.Collections.Generic;

public partial class LevelUpScreen : CanvasLayer
{
	private WeaponManager _weaponManager;
	private Player        _player;
	private List<WeaponData>    _weaponPool;
	private List<string>        _ownedWeapons = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible     = false;
		GameManager.Instance.LevelUp -= ShowLevelUpScreen;
		GameManager.Instance.LevelUp += ShowLevelUpScreen;
	}

	public override void _ExitTree()
	{
		GameManager.Instance.LevelUp -= ShowLevelUpScreen;
	}

	private void ShowLevelUpScreen()
	{
		if (_player       == null) _player       = GetTree().GetFirstNodeInGroup("player") as Player;
		if (_weaponManager == null) _weaponManager = _player?.GetNode<WeaponManager>("WeaponManager");

		_weaponPool = WeaponLibrary.BuildAll();

		Visible          = true;
		GetTree().Paused = true;

		var available = GetAvailableWeapons();
		bool canAddWeapon = available.Count >= 1 && _weaponManager.WeaponCount < WeaponManager.MaxWeapons;
		bool showWeapons  = canAddWeapon && GD.RandRange(0, 1) == 0;

		if (showWeapons)
			ShowWeaponCards(available);
		else
			ShowWeaponUpgradeCards(); //mejoras por arma específica
	}

	// ── Pool de armas nuevas ───────────────────────────────────────────────
	private void ShowWeaponCards(List<WeaponData> available)
	{
		available.Sort((a, b) => GD.RandRange(0, 1) == 0 ? -1 : 1);
		Button[] cards = GetCards();

		for (int i = 0; i < cards.Length; i++)
		{
			DisconnectCard(cards[i]);
			if (i < available.Count)
			{
				var w = available[i];
				cards[i].Visible = true;
				cards[i].Text    = $"{w.Name}\nDano:{w.Damage} FR:{w.FireRate}s Pierce:{w.PierceCount}";
				cards[i].Pressed += () => SelectWeapon(w);
			}
			else cards[i].Visible = false;
		}
	}

	private void SelectWeapon(WeaponData weapon)
	{
		_weaponManager.TryAddWeapon(weapon);
		_ownedWeapons.Add(weapon.Name);
		Close();
	}

	// ── Mejoras por arma específica ───────────────────────────────────────
	private void ShowWeaponUpgradeCards()
	{
		var activeWeapons = _weaponManager.GetActiveWeapons(); // devuelve List<WeaponData>
		Button[] cards    = GetCards();

		for (int i = 0; i < cards.Length; i++)
		{
			DisconnectCard(cards[i]);

			if (i < activeWeapons.Count)
			{
				// Elegir un upgrade aleatorio para esta arma
				var upgrades = WeaponLibrary.GetUpgradesForWeapon(activeWeapons[i]);
				upgrades.Sort((a, b) => GD.RandRange(0, 1) == 0 ? -1 : 1);
				var upgrade = upgrades[0];

				var weaponRef = activeWeapons[i]; // capturar referencia

				cards[i].Visible = true;
				cards[i].Text    = $"{upgrade.Name}\n{upgrade.Description}";
				cards[i].Pressed += () => { upgrade.Apply(weaponRef); Close(); };
			}
			else cards[i].Visible = false;
		}
	}

	private List<WeaponData> GetAvailableWeapons()
	{
		var owned     = _weaponManager.GetActiveWeaponNames();
		var available = new List<WeaponData>();
		foreach (var w in _weaponPool)
			if (!owned.Contains(w.Name) && !_ownedWeapons.Contains(w.Name))
				available.Add(w);
		return available;
	}

	private Button[] GetCards() => new Button[]
	{
		GetNode<Button>("Panel/VBox/UpgradeCard1"),
		GetNode<Button>("Panel/VBox/UpgradeCard2"),
		GetNode<Button>("Panel/VBox/UpgradeCard3"),
	};

	private void DisconnectCard(Button card)
	{
		foreach (var conn in card.GetSignalConnectionList("pressed"))
			card.Disconnect("pressed", (Callable)conn["callable"]);
	}

	private void Close() { Visible = false; GetTree().Paused = false; }
}
