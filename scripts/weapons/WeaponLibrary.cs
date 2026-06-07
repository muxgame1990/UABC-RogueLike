using Godot;
using System.Collections.Generic;

public class WeaponUpgrade
{
	public string               Name;
	public string               Description;
	public string               ForWeapon; // nombre del arma que mejora
	public System.Action<WeaponData> Apply;
}

public static class WeaponLibrary
{
	public static List<WeaponData> BuildAll()
	{
		//armas de la alfa
		var pencil   = Load("res://scenes/weapons/pencil_weapon.tscn");
		var revolver = Load("res://scenes/weapons/revolver_weapon.tscn");
		var bone     = Load("res://scenes/weapons/bone_weapon.tscn");
		//var axe      = Load("res://scenes/weapons/axe_weapon.tscn");
		
		//armas de las clases actuales
		var longSword    = Load("res://scenes/weapons/long_sword.tscn");
		var bow          = Load("res://scenes/weapons/bow.tscn");
		var dagger       = Load("res://scenes/weapons/dagger.tscn");
		var mallet       = Load("res://scenes/weapons/mallet.tscn");
		var catalystBeam = Load("res://scenes/weapons/catalyst_beam.tscn");
		var ironAxe      = Load("res://scenes/weapons/iron_axe.tscn");
		var magicBook    = Load("res://scenes/weapons/magic_book.tscn");
		var ironSpear    = Load("res://scenes/weapons/iron_spear.tscn");
		var axeWeapon    = Load("res://scenes/weapons/axe_weapon.tscn");
		var lightning       = Load("res://scenes/weapons/lightning.tscn");
		var meteor       = Load("res://scenes/weapons/meteor.tscn");
		var laser       = Load("res://scenes/weapons/laser_orbital.tscn");
		var shield       = Load("res://scenes/weapons/shield_orbital.tscn");
		var fireWalker  = Load("res://scenes/weapons/fire_walker.tscn");
		var fireBall  = Load("res://scenes/weapons/fire_ball.tscn"); 


		return new List<WeaponData>
		{
			// ── Armas de clase ─────────────────────────────────────────
			new WeaponData {
				Name="Espada Larga", WeaponType="melee",
				Damage=35f, Speed=0f, Lifetime=0.3f, FireRate=1.0f,
				BulletScale=1.5f, PierceCount=10, BulletsPerShot=1,
				SpawnOffset=40f,                        //aparece frente al jugador
				BulletScene=longSword,
				IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
				Name="Arco", WeaponType="ranged",
				Damage=25f, Speed=500f, Lifetime=3f, FireRate=1.0f,
				BulletScale=0.9f, PierceCount=1, BulletsPerShot=1,
				SpawnOffset=16f,
				BulletScene=bow,
				IconPath = "res://sprites/Bow.png"
			},
			new WeaponData {
				Name="Dagas", WeaponType="melee",
				Damage=20f, Speed=0f, Lifetime=0.2f, FireRate=0.4f,
				BulletScale=1.0f, PierceCount=10, BulletsPerShot=2,
				SpawnOffset=35f,
				IsOnEnemyWeapon=true,
				BulletScene=dagger,
				IconPath = "res://sprites/Dagger.png"
			},
			new WeaponData {
				Name="Mazo", WeaponType="melee",
				Damage=50f, Speed=0f, Lifetime=0.3f, FireRate=1.5f,
				BulletScale=2.0f, PierceCount=10, BulletsPerShot=1,
				SpawnOffset=45f,
				BulletScene=mallet,
				IconPath = "res://sprites/mace.png"
			},
			new WeaponData {
				Name="Catalizador de Energia", WeaponType="magic",
				Damage=30f, Speed=600f, Lifetime=4f, FireRate=0.6f,
				BulletScale=0.8f, PierceCount=5, BulletsPerShot=1,
				SpawnOffset=10f,
				BulletScene=catalystBeam,
				IconPath = "res://sprites/Catalizador.png"
			},
			new WeaponData {
				Name="Hacha de Acero", WeaponType="melee",
				Damage=40f, Speed=0f, Lifetime=0f, FireRate=0f,
				BulletScale=1.2f, PierceCount=0, BulletsPerShot=1,
				IsOrbital=true,
				BulletScene=ironAxe,
				IconPath = "res://sprites/Battleaxe.png"
			},
			new WeaponData {
				Name="Libro Magico", WeaponType="magic",
				Damage=22f, Speed=250f, Lifetime=5f, FireRate=0.8f,
				BulletScale=1.1f, PierceCount=1, BulletsPerShot=1,
				BounceCount=3, SpawnOffset=10f,
				BulletScene=magicBook,
				IconPath = "res://sprites/Magic_Tome.png"
			},
			new WeaponData {
				Name="Fireball", WeaponType="magic",
				Damage=22f, Speed=250f, Lifetime=5f, FireRate=0.8f,
				BulletScale=1.1f, PierceCount=1, BulletsPerShot=1,
				BounceCount=3, SpawnOffset=10f,
				BulletScene=fireBall,
				IconPath = "res://sprites/baculo_magico.png"
			},
			new WeaponData {
				Name="Lanza de Acero", WeaponType="melee",
				Damage=45f, Speed=0f, Lifetime=0.35f, FireRate=1.2f,
				BulletScale=1.3f, PierceCount=10, BulletsPerShot=1,
				SpawnOffset=50f,
				BulletScene=ironSpear,
				IconPath = "res://sprites/lance.png"
			},
			// ── Armas del pool (level up) ──────────────────────────────
						new WeaponData {
			Name="Lightning",
			WeaponType="magic",
			Damage=40f,
			FireRate=0.8f,
			BounceCount=1,
			IsOnEnemyWeapon=true,
			BulletScene=lightning,
			IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
			Name="Meteor",
			WeaponType="magic",
			Damage=40f,
			FireRate=0.8f,
			IsOnEnemyWeapon=true,
			BulletScene=meteor,
			IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
			Name="Laser",
			WeaponType="magic",
			Damage=40f,
			FireRate=0.8f,
			IsOrbital=true,
			BulletScene=laser,
			IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
			Name="Shield",
			WeaponType="magic",
			IsOrbital=true,
			BulletScene=shield,
			IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
			Name="Fire Walker",
			WeaponType="magic",
			Damage=12f,
			FireRate=0.4f,
			Lifetime=3f,
			IsZoneWeapon=true,
			FollowPlayer=false,
			BulletScene=fireWalker,
			IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
				Name="Lapiz", WeaponType="ranged",
				Damage=15f, Speed=300f, Lifetime=2f, FireRate=0.6f,
				BulletScale=1.0f, PierceCount=1, BulletsPerShot=3,
				SpawnOffset=10f, BulletScene=pencil,
				IconPath = "res://sprites/Sword.png"
			},
			new WeaponData {
				Name="Revolver", WeaponType="ranged",
				Damage=60f, Speed=600f, Lifetime=4f, FireRate=1.5f,
				BulletScale=1.0f, PierceCount=5, BulletsPerShot=1,
				SpawnOffset=10f, BulletScene=revolver,
				IconPath = "res://sprites/Sword.png"
			},
			//new WeaponData {
			//	Name="Hacha", WeaponType="melee",
			//	Damage=40f, Speed=0f, Lifetime=4f, FireRate=1.3f,
			//	BulletScale=1.2f, PierceCount=3, BulletsPerShot=1,
			//	IsOrbital=true, BulletScene=axeWeapon
			//},
			new WeaponData {
				Name="Hueso", WeaponType="ranged",
				Damage=20f, Speed=100f, Lifetime=4f, FireRate=1.0f,
				BulletScale=1.0f, PierceCount=1, BulletsPerShot=1,
				SpawnOffset=10f, BulletScene=bone,
				IconPath = "res://sprites/Sword.png"
			},
		};
	}

	private static PackedScene Load(string path) => GD.Load<PackedScene>(path);

	public static WeaponData Get(string name) =>
		BuildAll().Find(w => w.Name == name);

	// ── Upgrades por arma ──────────────────────────────────────────────
	public static List<WeaponUpgrade> GetUpgradesForWeapon(WeaponData weapon)
	{
		var upgrades = new List<WeaponUpgrade>
		{
			new WeaponUpgrade { Name="Dano +10",     Description=$"{weapon.Name}: +10 dano",               ForWeapon=weapon.Name, Apply=w => w.Damage       += 10f },
			new WeaponUpgrade { Name="Vel. ataque",  Description=$"{weapon.Name}: dispara mas rapido",      ForWeapon=weapon.Name, Apply=w => w.FireRate      = Mathf.Max(0.05f, w.FireRate - 0.1f) },
			new WeaponUpgrade { Name="+1 proyectil", Description=$"{weapon.Name}: +1 bala por disparo",     ForWeapon=weapon.Name, Apply=w => w.BulletsPerShot++ },
			new WeaponUpgrade { Name="Tamaño +20%", Description=$"{weapon.Name}: proyectiles mas grandes", ForWeapon=weapon.Name, Apply=w => w.BulletScale  *= 1.2f },
		};

		// Upgrades adicionales según tipo
		if (weapon.WeaponType == "ranged" || weapon.WeaponType == "magic")
		{
			upgrades.Add(new WeaponUpgrade { Name="Vel. proyectil", Description=$"{weapon.Name}: +20% velocidad", ForWeapon=weapon.Name, Apply=w => w.Speed *= 1.2f });
			upgrades.Add(new WeaponUpgrade { Name="+1 Perforar",   Description=$"{weapon.Name}: perfora un enemigo mas", ForWeapon=weapon.Name, Apply=w => w.PierceCount++ });
		}

		if (weapon.WeaponType == "magic")
			upgrades.Add(new WeaponUpgrade { Name="+1 Rebote", Description=$"{weapon.Name}: rebota una vez mas", ForWeapon=weapon.Name, Apply=w => w.BounceCount++ });

		return upgrades;
	}
}
