using Godot;
using System.Collections.Generic;

//Datos de un arma individual
public class WeaponData
{
	public string Name;
	public string WeaponType    = "ranged"; // "melee", "ranged", "magic"

	public float Damage         = 20f;
	public float Speed          = 350f;
	public float Lifetime       = 3f;
	public float FireRate       = 0.8f;
	public float BulletScale    = 1f;
	public int   PierceCount    = 1;
	public int   BulletsPerShot = 1;
	public float SpawnOffset    = 0;
	public bool IsOrbital       = false;
	public int   BounceCount    = 0;

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
	
	//|--------------------------------------|
	public const int MaxWeapons = 3; // !!!  | valor original = 3
	//|--------------------------------------|
	
	private List<WeaponInstance> _activeWeapons = new();
	private Dictionary<string, List<OrbitalWeapon>> _orbitalsByWeapon = new();
	private Player _player;
	
	private WeaponData _pendingStartWeapon;

	public override void _Ready()
	{
		_player = GetParent<Player>();

		ClassData selectedClass = ClassLibrary.Get(GameManager.Instance.SelectedClassIndex);
		if (selectedClass != null)
		{
			_pendingStartWeapon = WeaponLibrary.Get(selectedClass.StartingWeaponName);
			//Diferir para que Player._Ready() corra primero
			CallDeferred(nameof(InitStartWeapon));
		}
	}

	public override void _Process(double delta)
	{
		Node2D target = GetNearestEnemy();

		foreach (var weapon in _activeWeapons)
		{
			if (weapon.Data.IsOrbital)
			{
				CheckOrbitalCount(weapon.Data);
				continue;
			}
			
			weapon.FireTimer += (float)delta;
			if (weapon.FireTimer >= weapon.Data.FireRate / _player.Stats.AttackSpeedMultiplier)
			{
				weapon.FireTimer = 0f;
				if (target != null)
					Shoot(weapon.Data, target);
			}
		}
	}

	private void Shoot(WeaponData data, Node2D target)
	{
		PlayerStats stats     = _player.Stats;
		Vector2     direction = (target.GlobalPosition - _player.GlobalPosition).Normalized();

		float finalDamage = data.Damage * stats.DamageMultiplier * _player.PassiveDamageBonus;
		if (data.WeaponType == "melee")  finalDamage *= (1f + stats.MeleeDamageBonus);
		if (data.WeaponType == "ranged") finalDamage *= (1f + stats.RangedDamageBonus);
		if (data.WeaponType == "magic")  finalDamage *= (1f + stats.MagicDamageBonus);
		if (_player.IsChargeActive())    { finalDamage *= 1.3f; _player.ConsumeCharge(); }

		int   finalBullets  = data.BulletsPerShot + stats.BonusProjectiles;
		float finalSpeed    = data.Speed          * stats.ProjectileSpeedMult;
		float finalScale    = data.BulletScale    * stats.SizeMultiplier;
		int   finalPierce   = data.PierceCount    + stats.BonusPierce;
		int   finalBounce   = data.BounceCount    + stats.BonusBounce;

		for (int i = 0; i < finalBullets; i++)
		{
			Vector2 finalDir = direction;
			if (finalBullets > 1)
			{
				float spread = Mathf.DegToRad(20f);
				float offset = spread * (i - (finalBullets - 1) / 2f);
				finalDir = direction.Rotated(offset);
			}

			PackedScene scene = data.BulletScene ?? DefaultBulletScene;
			if (scene == null) return;

			Bullet bullet      = scene.Instantiate<Bullet>();
			bullet.Damage      = finalDamage;
			bullet.Speed       = finalSpeed;
			bullet.Lifetime    = data.Lifetime;
			bullet.BulletScale = finalScale;
			bullet.PierceCount = finalPierce;
			bullet.BounceCount = finalBounce;

			_player.GetParent().AddChild(bullet);
			bullet.GlobalPosition = _player.GlobalPosition + direction * data.SpawnOffset;
			bullet.Initialize(finalDir);

			bullet.OnHit += (dmg) => _player.OnDamageDealt(dmg);
		}
	}

	//Agregar un arma nueva (llamado desde LevelUpScreen)
	public bool TryAddWeapon(WeaponData data)
	{
		if (_activeWeapons.Count >= MaxWeapons) return false;
		_activeWeapons.Add(new WeaponInstance { Data = data });
		
		if (data.IsOrbital)
			SpawnOrbital(data);
		
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
	
	public List<WeaponData> GetActiveWeapons()
	{
		var list = new List<WeaponData>();
		foreach (var w in _activeWeapons)
			list.Add(w.Data);
		return list;
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
	
	//Spawnea orbitales faltantes si BulletsPerShot aumentó
	private void CheckOrbitalCount(WeaponData data)
	{
		if (!_orbitalsByWeapon.ContainsKey(data.Name))
			_orbitalsByWeapon[data.Name] = new List<OrbitalWeapon>();

		var list    = _orbitalsByWeapon[data.Name];
		int target  = Mathf.Max(1, data.BulletsPerShot);

		while (list.Count < target)
			SpawnSingleOrbital(data, list);
	}

	private void SpawnSingleOrbital(WeaponData data, List<OrbitalWeapon> list)
	{
		if (data.BulletScene == null) return;

		// Distribuir hachas en ángulos iguales
		int    total      = Mathf.Max(1, data.BulletsPerShot);
		float  startAngle = list.Count * (Mathf.Tau / total);

		OrbitalWeapon orbital = data.BulletScene.Instantiate<OrbitalWeapon>();
		_player.GetParent().CallDeferred("add_child", orbital);
		orbital.Initialize(startAngle, data);
		list.Add(orbital);
	}
	
	private void SpawnOrbital(WeaponData data)
	{
		CheckOrbitalCount(data);
	}
	
	private void InitStartWeapon()
	{
		if (_pendingStartWeapon != null)
			TryAddWeapon(_pendingStartWeapon);
	}
}
