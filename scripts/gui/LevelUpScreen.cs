using Godot;
using System.Collections.Generic;

public partial class LevelUpScreen : CanvasLayer
{
	private WeaponManager _weaponManager;
	private Player        _player;

	private List<(string Name, string Desc, System.Action Apply)> _upgradePool;
	private List<WeaponData> _weaponPool;      // todas las armas disponibles
	private List<string>     _ownedWeapons = new(); // armas ya obtenidas

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;
		
		GameManager.Instance.LevelUp -= ShowLevelUpScreen;
		GameManager.Instance.LevelUp += ShowLevelUpScreen;
		//GameManager.Instance.LevelUp += ShowLevelUpScreen;
	}
	
	public override void _ExitTree()
	{
		GameManager.Instance.LevelUp -= ShowLevelUpScreen;
	}
	
	// ─── Pools ────────────────────────────────────────────────────────────────

	private void BuildUpgradePool()
	{
		_upgradePool = new List<(string, string, System.Action)>
		{
			("Daño +10",          "Todas tus armas hacen mas daño",   () => _weaponManager.UpgradeDamage(10f)),
			("Vel. disparo",      "Todas disparan mas rapido",         () => _weaponManager.UpgradeFireRate(0.1f)),
			("Bala extra",        "Todas disparan una bala adicional", () => _weaponManager.UpgradeBulletCount()),
			("+20 HP max",        "Aumenta tu vida maxima",            () => { _player.MaxHp += 20f; _player.Heal(20f); }),
			("+20 Velocidad",     "Te mueves mas rapido",              () => _player.Speed += 20f),
			("Curacion",          "Recupera 30 HP ahora",              () => _player.Heal(30f)),
		};
	}

	private void BuildWeaponPool()
	{
		_weaponPool = WeaponLibrary.BuildAll();
	}

	// ─── Mostrar pantalla ─────────────────────────────────────────────────────

	private void ShowLevelUpScreen()
	{
		if (_player == null)
			_player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (_weaponManager == null)
			_weaponManager = _player?.GetNode<WeaponManager>("WeaponManager");

		BuildUpgradePool();
		BuildWeaponPool();

		Visible = true;
		GetTree().Paused = true;

		var available = GetAvailableWeapons();

		//Solo necesita al menos 1 arma disponible y que no tenga el máximo
		bool weaponPoolAvailable = available.Count >= 1
							   	&& _weaponManager.WeaponCount < WeaponManager.MaxWeapons;

		bool showWeapons = weaponPoolAvailable && GD.RandRange(0, 1) == 0;

		if (showWeapons)
			ShowWeaponCards(available);
		else
			ShowUpgradeCards();
	}

	// ─── Pool de mejoras ──────────────────────────────────────────────────────

	private void ShowUpgradeCards()
	{
		var shuffled = new List<(string, string, System.Action)>(_upgradePool);
		shuffled.Sort((a, b) => GD.RandRange(0, 1) == 0 ? -1 : 1);

		Button[] cards = GetCards();

		for (int i = 0; i < cards.Length; i++)
		{
			cards[i].Visible = true; //siempre visible en mejoras
			var captured = shuffled[i];
			DisconnectCard(cards[i]);
			cards[i].Text = $"{captured.Item1}\n{captured.Item2}";
			cards[i].Pressed += () => SelectUpgrade(captured.Item3);
		}
	}

	private void SelectUpgrade(System.Action apply)
	{
		apply.Invoke();
		Close();
	}

	// ─── Pool de armas ────────────────────────────────────────────────────────

	private void ShowWeaponCards(List<WeaponData> available)
	{
		available.Sort((a, b) => GD.RandRange(0, 1) == 0 ? -1 : 1);

		Button[] cards = GetCards();

		for (int i = 0; i < cards.Length; i++)
		{
			DisconnectCard(cards[i]);

			if (i < available.Count)
			{
				//Mostrar carta con arma disponible
				var weapon = available[i];
				cards[i].Visible = true;
				cards[i].Text =
					$"{weapon.Name}\n" +
					$"Daño:{weapon.Damage}  FR:{weapon.FireRate}s  " +
					$"Pierce:{weapon.PierceCount}  Balas:{weapon.BulletsPerShot}";
				cards[i].Pressed += () => SelectWeapon(weapon);
			}
			else
			{
				//Ocultar cartas sobrantes si hay menos de 3 armas disponibles
				cards[i].Visible = false;
			}
		}
	}

	private void SelectWeapon(WeaponData weapon)
	{
		_weaponManager.TryAddWeapon(weapon);
		_ownedWeapons.Add(weapon.Name); //marcar como obtenida
		Close();
	}

	// ─── Helpers ──────────────────────────────────────────────────────────────

	// Devuelve armas del pool que el jugador NO tiene aún
	private List<WeaponData> GetAvailableWeapons()
	{
		var owned = _weaponManager.GetActiveWeaponNames();
		var available = new List<WeaponData>();

		foreach (var w in _weaponPool)
		{
			if (!owned.Contains(w.Name) && !_ownedWeapons.Contains(w.Name))
				available.Add(w);
		}
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

	private void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
