using Godot;
using System.Collections.Generic;

public static class WeaponLibrary
{
	// Nombres de las armas por defecto de cada personaje
	// Mismo orden que _scenePaths en CharacterSelect
	public static readonly string[] CharacterDefaultWeapons =
	{
		"Lapiz",   // Cimarron
		"Revolver", // Abad
		"Hueso",   // Edwin
		"Hacha",   // Anguiano
		"Lapiz",   // Bosch
	};

	public static List<WeaponData> BuildAll()
	{
		var pencil   = GD.Load<PackedScene>("res://scenes/weapons/pencil_weapon.tscn");
		var revolver = GD.Load<PackedScene>("res://scenes/weapons/revolver_weapon.tscn");
		var bone     = GD.Load<PackedScene>("res://scenes/weapons/bone_weapon.tscn");
		var axe      = GD.Load<PackedScene>("res://scenes/weapons/axe_weapon.tscn");

		return new List<WeaponData>
		{
			new WeaponData {
				Name           = "Lapiz",
				Damage         = 20f,
				Speed          = 350f,
				Lifetime       = 3f,
				FireRate       = 0.8f,
				BulletScale    = 1f,
				PierceCount    = 1,
				BulletsPerShot = 1,
				BulletScene    = pencil
			},
			new WeaponData {
				Name           = "Revolver",
				Damage         = 60f,
				Speed          = 600f,
				Lifetime       = 4f,
				FireRate       = 1.5f,
				BulletScale    = 1f,
				PierceCount    = 5,
				BulletsPerShot = 1,
				BulletScene    = revolver
			},
			new WeaponData {
				Name           = "Hueso",
				Damage         = 20f,
				Speed          = 100f,
				Lifetime       = 4f,
				FireRate       = 1f,
				BulletScale    = 1f,
				PierceCount    = 1,
				BulletsPerShot = 1,
				BulletScene    = bone
			},
			new WeaponData {
				Name           = "Hacha",
				Damage         = 40f,
				Speed          = 120f,
				Lifetime       = 4f,
				FireRate       = 1.3f,
				BulletScale    = 1f,
				PierceCount    = 1,
				BulletsPerShot = 1,
				BulletScene    = axe
			},
		};
	}

	// Devuelve un arma específica por nombre
	public static WeaponData Get(string name)
	{
		return BuildAll().Find(w => w.Name == name);
	}
}
