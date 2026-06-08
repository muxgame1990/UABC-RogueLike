using Godot;
using System;

public partial class PlayerStats : Node
{
	// Vida
	public float MaxHp                   = 999999f;
	public float HpRegen                 = 0f;    // HP/segundo
	public float LifeSteal               = 0f;    // 0.0 - 1.0 (%)
	public float Thorns                  = 0f;    // daño reflejado

	// Daño
	public float DamageMultiplier        = 6f;
	public float MeleeDamageBonus        = 0f;    // bonus solo a melee
	public float RangedDamageBonus       = 0f;    // bonus solo a distancia
	public float MagicDamageBonus        = 0f;    // bonus solo a mágicas

	// Ataque
	public float AttackSpeedMultiplier   = 1f;    // >1 = más rápido
	public int   BonusProjectiles        = 0;
	public int   BonusBounce             = 0;
	public int   BonusPierce             = 0;
	public float SizeMultiplier          = 1f;
	public float ProjectileSpeedMult     = 1f;

	// Movimiento
	public float MovementSpeedMult       = 1f;

	// Recolección
	public float PickupRangeMult         = 1f;
	public float XpMultiplier            = 1f;
	public float GoldMultiplier          = 1f;

	// Otros
	public float Luck                    = 0f;
}
