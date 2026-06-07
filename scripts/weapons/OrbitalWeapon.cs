using System;
using Godot;

public partial class OrbitalWeapon : Area2D
{
	[Export] public float OrbitRadius   = 80f;
	[Export] public float OrbitSpeed    = 3f;
	[Export] public float DamageCooldown = 0.05f; //0.1 original

	public WeaponData Data;

	private Player _player;
	private float  _angle       = 0f;
	private float  _damageTimer = 0f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public void Initialize(float startAngle, WeaponData data)
	{
		_angle = startAngle;
		Data   = data;
	}

	public override void _Process(double delta)
	{
		if (_player == null)
		{
			_player = GetTree().GetFirstNodeInGroup("player") as Player;
			return;
		}

		// Sincronizar tamaño con WeaponData cada frame
		if (Data != null)
			Scale = Vector2.One * Data.BulletScale * _player.Stats.SizeMultiplier;
		float baseAngle = GameManager.Instance.ElapsedTime * (OrbitSpeed * _player.Stats.AttackSpeedMultiplier);
		float finalAngle = baseAngle + _angle;
		//_angle         += OrbitSpeed * (float)delta;
		GlobalPosition  = _player.GlobalPosition + new Vector2(
			Mathf.Cos(finalAngle) * OrbitRadius,
			Mathf.Sin(finalAngle) * OrbitRadius
		);
		Rotation = finalAngle + Mathf.Pi / 2f;

		if (_damageTimer > 0f) _damageTimer -= (float)delta;
	}
	public void SetOrbitData(float angleOffset, float orbitRadious, WeaponData data){
		_angle = angleOffset;
		OrbitRadius = orbitRadious;
		Data = data;
	}
	private void OnBodyEntered(Node2D body)
	{
		if (_damageTimer > 0f || !body.IsInGroup("enemies")) return;

		if (body is IDamageable damageable && Data != null)
		{
			//Leer daño actualizado de WeaponData
			float dmg = Data.Damage * (_player?.Stats.DamageMultiplier ?? 1f)
									* (_player?.PassiveDamageBonus    ?? 1f);
			damageable.TakeDamage(dmg);
			_damageTimer = DamageCooldown;
		}
	}
}
