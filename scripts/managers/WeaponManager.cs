using Godot;
using System.Collections.Generic;

//Datos de un arma individual
public class WeaponData
{
	public string Name;
	public float  Damage     = 20f;
	public float  Speed      = 350f;
	public float  Lifetime   = 3f;
	public float  FireRate   = 0.8f;
	public float  BulletScale = 1f;
	public int    PierceCount = 1;
	public int    BulletsPerShot = 1;
	public PackedScene BulletScene;
}

//Instancia activa de un arma (tiene su propio timer)
public class WeaponInstance
{
	public WeaponData Data;
	public float      FireTimer = 0f;
}

public partial class WeaponManager : Node
{
	[Export] public float DetectionRange = 200f;
	[Export] public PackedScene DefaultBulletScene;

	public const int MaxWeapons = 3;

	private List<WeaponInstance> _activeWeapons = new();
	private Player _player;

	public override void _Ready()
	{
		_player = GetParent<Player>();

		//Obtener el arma por defecto del personaje seleccionado
		int idx = GameManager.Instance.SelectedCharacterIndex;
		string defaultWeaponName = idx < WeaponLibrary.CharacterDefaultWeapons.Length
			? WeaponLibrary.CharacterDefaultWeapons[idx]
			: "Lapiz Basico";

		WeaponData startWeapon = WeaponLibrary.Get(defaultWeaponName);

		if (startWeapon != null)
			_activeWeapons.Add(new WeaponInstance { Data = startWeapon });
		else
			GD.PrintErr($"WeaponManager: No se encontró el arma '{defaultWeaponName}'");
	}

	public override void _Process(double delta)
	{
		Node2D target = GetNearestEnemy();

		foreach (var weapon in _activeWeapons)
		{
			weapon.FireTimer += (float)delta;
			if (weapon.FireTimer >= weapon.Data.FireRate)
			{
				weapon.FireTimer = 0f;
				if (target != null)
					Shoot(weapon.Data, target);
			}
		}
	}

	private void Shoot(WeaponData data, Node2D target)
	{
		Vector2 direction = (target.GlobalPosition - _player.GlobalPosition).Normalized();

		for (int i = 0; i < data.BulletsPerShot; i++)
		{
			Vector2 finalDir = direction;
			if (data.BulletsPerShot > 1)
			{
				float spread = Mathf.DegToRad(20f);
				float offset = spread * (i - (data.BulletsPerShot - 1) / 2f);
				finalDir = direction.Rotated(offset);
			}

			PackedScene scene = data.BulletScene ?? DefaultBulletScene;
			if (scene == null) return;

			Bullet bullet       = scene.Instantiate<Bullet>();
			bullet.Damage       = data.Damage;
			bullet.Speed        = data.Speed;
			bullet.Lifetime     = data.Lifetime;
			bullet.BulletScale  = data.BulletScale;
			bullet.PierceCount  = data.PierceCount;

			_player.GetParent().AddChild(bullet);
			bullet.GlobalPosition = _player.GlobalPosition;
			bullet.Initialize(finalDir);
		}
	}

	//Agregar un arma nueva (llamado desde LevelUpScreen)
	public bool TryAddWeapon(WeaponData data)
	{
		if (_activeWeapons.Count >= MaxWeapons) return false;
		_activeWeapons.Add(new WeaponInstance { Data = data });
		return true;
	}

	public int WeaponCount => _activeWeapons.Count;

	public List<string> GetActiveWeaponNames()
	{
		var names = new List<string>();
		foreach (var w in _activeWeapons)
			names.Add(w.Data.Name);
		return names;
	}

	// Upgrades del pool de mejoras (afectan TODAS las armas)
	public void UpgradeDamage(float amount)
	{
		foreach (var w in _activeWeapons)
			w.Data.Damage += amount;
	}
	public void UpgradeFireRate(float amount)
	{
		foreach (var w in _activeWeapons)
			w.Data.FireRate = Mathf.Max(0.1f, w.Data.FireRate - amount);
	}
	public void UpgradeBulletCount()
	{
		foreach (var w in _activeWeapons)
			w.Data.BulletsPerShot++;
	}

	private Node2D GetNearestEnemy()
	{
		var enemies = GetTree().GetNodesInGroup("enemies");
		Node2D nearest = null;
		float minDist = float.MaxValue;

		foreach (Node node in enemies)
		{
	   		if (node is Node2D enemyNode)
			{
				float d = _player.GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
				if (d < DetectionRange && d < minDist)
				{ 
					minDist = d; 
					nearest = enemyNode; 
				}
			}
		}
		return nearest;
	}
}
