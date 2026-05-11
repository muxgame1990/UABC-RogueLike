using Godot;
using System;

public partial class Enemy : CharacterBody2D, IDamageable
{
	[Export] public float MaxHp = 30f;
	[Export] public float Damage = 10f;
	[Export] public float Speed = 60f;
	[Export] public float DamageCooldown = 3f;
	[Export] public float XpValue = 10f;
	
	//Drops
	[Export] public PackedScene ExpOrbScene;
	[Export] public PackedScene GoldCoinScene;
	[Export] public int CoinValue = 1;
	
	private float _currentHp;
	private Player _player;
	private AnimatedSprite2D _sprite;
	private float _damageCooldownTimer = 0f;
	private bool _isDead = false;

	public override void _Ready()
	{
		_currentHp = MaxHp;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_player = GetTree().GetFirstNodeInGroup("player") as Player;
		AddToGroup("enemies");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead || _player == null) return;
		
		if (_damageCooldownTimer > 0)
			_damageCooldownTimer -= (float)delta;
		//Seguimiento
		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		if (direction.X != 0)
			_sprite.FlipH = direction.X < 0;

		MoveAndSlide();
		
		float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
		if (distanceToPlayer < 30f && _damageCooldownTimer <= 0f)
		{
			_player.TakeDamage(Damage);
			_damageCooldownTimer = DamageCooldown;
		}
	}
	
	public void TakeDamage(float amount)
	{
		if (_isDead) return;
		_currentHp -= amount;
		if (_currentHp <= 0)
			Die();
	}
	
	public void Die()
	{
		_isDead = true;
		CallDeferred(nameof(SpawnDrops));
		CallDeferred("queue_free");
	}
	
	private void SpawnDrops()
	{
		// Soltar orbe de XP
		if (ExpOrbScene != null)
		{
			ExpOrb orb    = ExpOrbScene.Instantiate<ExpOrb>();
			orb.XpValue   = XpValue;
			GetParent().AddChild(orb);
			orb.GlobalPosition = GlobalPosition;
		}

		// Soltar moneda
		if (GoldCoinScene != null)
		{
			GoldCoin coin   = GoldCoinScene.Instantiate<GoldCoin>();
			coin.CoinValue  = CoinValue;
			GetParent().AddChild(coin);
			coin.GlobalPosition = GlobalPosition;
		}
	}
}
