using System;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float  Speed    = 150f;
	[Export] public float  MaxHp    = 100f;
	[Export] public string AnimIdle = "idle";
	[Export] public string AnimWalk = "walk";
	public float       CurrentHp { get; private set; }
	public PlayerStats Stats     { get; private set; } = new();

	//Separado de Stats.DamageMultiplier para no sobreescribir upgrades
	public float PassiveDamageBonus { get; private set; } = 1f;

	private PassiveType      _passive;
	private AnimatedSprite2D _sprite;
	private bool             _isDead = false;

	// Catalizador
	private float _catalystMultiplier = 1f;
	private const float CatalystMax   = 8f;

	// Ogro
	private bool  _bloodMarkActive   = false;
	private float _bloodMarkTimer    = 0f;
	private float _bloodMarkCooldown = 0f;
	private const float BloodMarkRegen    = 0.05f;
	private const float BloodMarkDuration = 10f;
	private const float BloodMarkCooldownTime = 20f;

	// Caballero con Corcel
	private float _walkTimer     = 0f;
	private bool  _chargeActive  = false;
	private float _chargeCooldown = 0f;
	[Export] public int MaxDash = 2;
	[Export] public float DashDistance = 120f;
	[Export] public float DashDuration = 0.25f;
	[Export] public float DashCooldown = 0.2f;
	[Export] public float DashRecharge = 1.5f;
	private Vector2 lastDirection = Vector2.Right;
	private bool isDashing = false;
	private int DashCount = 0;
	private float DashTimer = 0f;
	private float DashCooldownTimer = 0f;
	private float DashRechargeTimer = 0f;
	private Vector2 DashDirection = Vector2.Zero;
	public override void _Ready()
	{
		int idx = GameManager.Instance.SelectedCharacterIndex;
		float[] speeds  = { 200f, 120f, 150f, 130f, 180f };
		float[] maxHps  = {  80f, 150f, 100f, 120f,  90f };
		DashCount = MaxDash;
		if (idx < speeds.Length) Speed = speeds[idx];
		if (idx < maxHps.Length) MaxHp = maxHps[idx];
		CurrentHp = MaxHp;
		_passive = GameManager.Instance.SelectedPassive;
		ClassLibrary.ApplyPassiveStats(_passive, Stats);
		
		//Speed se multiplica UNA vez aquí
		Speed    *= Stats.MovementSpeedMult;
		_sprite   = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AddToGroup("player");
		
		//restaurar estado del jugador si viene de otra escena
		if(GameManager.Instance.HasSavedState){
			if (GameManager.Instance.SavedStats != null){
				Stats = GameManager.Instance.SavedStats;
			}
			if (GameManager.Instance.SavedCurrentHp > 0f){
				CurrentHp = Mathf.Min(
					GameManager.Instance.SavedCurrentHp, MaxHp
				);
			}
			PassiveDamageBonus = GameManager.Instance.SavedPassiveDamageBonus;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_isDead) return;
		float dt = (float)delta;
		if (DashCooldownTimer > 0)
			DashCooldownTimer -= dt;
		RechargeDash(dt);
		if (isDashing)
		{
			DashTimer -= dt;
			float DashSpeed = DashDistance / DashDuration;
			Velocity = DashDirection * DashSpeed;
			MoveAndSlide();
			if (DashTimer <= 0)
			{
				isDashing = false;
				Velocity = Vector2.Zero;
			}
			
			return;
		}
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		bool    isMoving  = direction != Vector2.Zero;

		if (isMoving)
		{
			//Solo Speed — MovementSpeedMult ya fue aplicado en _Ready

			direction = direction.Normalized();
			lastDirection = direction;
			Velocity = direction * Speed;
			_sprite.Play(AnimWalk);
			if (direction.X != 0) _sprite.FlipH = direction.X < 0;
		}
		else
		{
			Velocity = Vector2.Zero;
			_sprite.Play(AnimIdle);
		}
	if (Input.IsActionJustPressed("dash"))
		{
			Dash();
		}
		MoveAndSlide();
		UpdatePassive((float)delta, isMoving);
	}

	private void UpdatePassive(float delta, bool isMoving)
	{
		switch (_passive)
		{
			case PassiveType.Berserker:
				// Modifica PassiveDamageBonus
				int nearby = CountNearbyEnemies(150f);
				PassiveDamageBonus = 1f + nearby * 0.05f;
				break;

			case PassiveType.Ogro:
				// Daño escala con HP faltante
				float missing = 1f - (CurrentHp / MaxHp);
				PassiveDamageBonus = 1f + missing; // hasta x2

				// Regeneración de Marca de Sangre
				if (_bloodMarkActive)
				{
					Heal(MaxHp * BloodMarkRegen * delta);
					_bloodMarkTimer -= delta;
					if (_bloodMarkTimer <= 0f) _bloodMarkActive = false;
				}
				if (_bloodMarkCooldown > 0f) _bloodMarkCooldown -= delta;
				break;

			case PassiveType.CaballeroCorcel:
				if (_chargeCooldown > 0f) _chargeCooldown -= delta;

				if (isMoving)
				{
					_walkTimer += delta;
					if (_walkTimer >= 10f && _chargeCooldown <= 0f && !_chargeActive)
					{
						_chargeActive = true;
						GD.Print("Carga activada!");
					}
				}
				else
				{
					// Detenerse cancela la carga sin cooldown
					if (_chargeActive) GD.Print("Carga cancelada.");
					_walkTimer    = 0f;
					_chargeActive = false;
				}
				break;
		}

		// HP Regen pasiva
		if (Stats.HpRegen > 0f)
			Heal(Stats.HpRegen * delta);
	}
	public void TakeDamage(float amount)
	{
		if (_isDead) return;

		// Ogro: golpe mayor al 50% de HP max activa Marca de Sangre
		if (_passive == PassiveType.Ogro
			&& amount > MaxHp * 0.5f
			&& _bloodMarkCooldown <= 0f)
		{
			_bloodMarkActive   = true;
			_bloodMarkTimer    = BloodMarkDuration;
			_bloodMarkCooldown = BloodMarkCooldownTime;
			GD.Print("Marca de Sangre activada!");
		}

		if (isDashing) return;
if(ConsumeOrbitalShield())
{
	amount = 1f;
}
		CurrentHp -= amount;
		CurrentHp  = Mathf.Clamp(CurrentHp, 0f, MaxHp);
		if (CurrentHp <= 0f) Die();
	}
	public void Heal(float amount)
	{
		CurrentHp = Mathf.Clamp(CurrentHp + amount, 0f, MaxHp);
	}

	//Llamado desde WeaponManager al impactar un enemigo
	public void OnDamageDealt(float amount)
	{
		// LifeSteal
		if (Stats.LifeSteal > 0f)
			Heal(amount * Stats.LifeSteal);

		// Catalizador: acumula multiplicador por cada hit
		if (_passive == PassiveType.Catalizador)
		{
			_catalystMultiplier    = Mathf.Min(_catalystMultiplier + 0.1f, CatalystMax);
			PassiveDamageBonus     = _catalystMultiplier;
		}
	}

	public bool IsChargeActive()  => _chargeActive;
	public void ConsumeCharge()
	{
		_chargeActive  = false;
		_walkTimer     = 0f;
		_chargeCooldown = 25f;
		GD.Print("Carga consumida! Cooldown 25s");
	}

	private int CountNearbyEnemies(float radius)
	{
		int count = 0;
		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
			if (node is Node2D e && GlobalPosition.DistanceTo(e.GlobalPosition) < radius)
				count++;
		return count;
	}
	public void Dash(){
	if (DashCooldownTimer > 0)
	{
		GD.Print("Dash en cooldown");
		return;
	}
	if (DashCount <= 0)
	{
		GD.Print("Sin cargas de dash");
		return;
	}
		
		isDashing = true;
		DashTimer = DashDuration;
		DashCooldownTimer = DashCooldown;
		DashDirection = lastDirection;
		GD.Print("Dash hacia: " + DashDirection);
		DashCount--;
		GD.Print("Dash usado. Cargas restantes: " + DashCount);
	}
	private void Die()
	{
		_isDead = true;
		EventManager.Instance.EmitPlayerDied();
	}
	private void RechargeDash(float dt)
{
	if (DashCount >= MaxDash)
	{
		DashRechargeTimer = 0f;
		return;
	}
	DashRechargeTimer += dt;
	if (DashRechargeTimer >= DashRecharge)
	{
		DashCount++;
		DashRechargeTimer = 0f;
		GD.Print("Dash recargado. Cargas actuales: " + DashCount);
	}
}
private bool ConsumeOrbitalShield()
{
	foreach(Node node in GetTree().GetNodesInGroup("shields"))
	{
		if(node is ShieldOrbital shield &&
		   shield.IsActive)
		{
			shield.ConsumeShield();
			return true;
		}
	}

	return false;
}
}
