using Godot;
using System;

public partial class RangedEnemy : CharacterBody2D, IDamageable
{
	// ── Stats base exportables ─────────────────────────────────────────────
	[Export] public float MaxHp        = 25f;
	[Export] public float Damage       = 8f;   // se suma al daño del proyectil
	[Export] public float Speed        = 45f;
	[Export] public float XpValue      = 15f;

	// ── Comportamiento de disparo ──────────────────────────────────────────
	[Export] public float ShootRange   = 200f; // distancia a la que empieza a disparar
	[Export] public float FireRate     = 2.5f; // segundos entre disparos
	[Export] public PackedScene ProjectileScene; // SlimeThornWeapon.tscn

	// ── Stats del proyectil ────────────────────────────────────────────────
	[Export] public float ProjectileSpeed    = 120f;
	[Export] public float ProjectileLifetime = 4f;

	//Drops
	[Export] public PackedScene ExpOrbScene;
	[Export] public PackedScene GoldCoinScene;
	[Export] public int         CoinValue = 1;
	
	private float            _currentHp;
	private Player           _player;
	private AnimatedSprite2D _sprite;
	private float            _fireTimer = 0f;
	private bool             _isDead    = false;

	public override void _Ready()
	{
		_currentHp = MaxHp;
		_sprite    = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_player    = GetTree().GetFirstNodeInGroup("player") as Player;
		AddToGroup("enemies");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead || _player == null) return;

		float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);
		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

		// Flip sprite
		if (direction.X != 0)
			_sprite.FlipH = direction.X < 0;

		if (distance > ShootRange)
		{
			// Acercarse al jugador
			Velocity = direction * Speed;
			_sprite.Play("walk");
		}
		else
		{
			// Dentro del rango: detenerse y disparar
			Velocity = Vector2.Zero;
			_sprite.Play("idle");

			_fireTimer += (float)delta;
			if (_fireTimer >= FireRate)
			{
				_fireTimer = 0f;
				Shoot(direction);
			}
		}

		MoveAndSlide();
	}

	private void Shoot(Vector2 direction)
	{
		if (ProjectileScene == null) return;

		EnemyProjectile projectile = ProjectileScene.Instantiate<EnemyProjectile>();

		projectile.Damage   = projectile.Damage + Damage;
		projectile.Speed    = ProjectileSpeed;
		projectile.Lifetime = ProjectileLifetime;

		GetParent().AddChild(projectile);
		projectile.GlobalPosition = GlobalPosition;
		projectile.Initialize(direction);
	}

	public void TakeDamage(float amount)
	{
		if (_isDead) return;
		_currentHp -= amount;
		if (_currentHp <= 0) Die();
	}

	private void Die()
	{
		_isDead = true;
		CallDeferred(nameof(SpawnDrops));
		CallDeferred("queue_free");
	}
	
	private void SpawnDrops()
	{
		if (ExpOrbScene != null)
		{
			ExpOrb orb  = ExpOrbScene.Instantiate<ExpOrb>();
			orb.XpValue = XpValue;
			GetParent().AddChild(orb);
			orb.GlobalPosition = GlobalPosition;
		}

		if (GoldCoinScene != null)
		{
			GoldCoin coin  = GoldCoinScene.Instantiate<GoldCoin>();
			coin.CoinValue = CoinValue;
			GetParent().AddChild(coin);
			coin.GlobalPosition = GlobalPosition;
		}
	}
}
