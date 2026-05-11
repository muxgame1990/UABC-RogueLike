using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public float Speed    = 350f;
	[Export] public float Damage   = 30f;
	[Export] public float Lifetime = 3f;
	[Export] public float BulletScale = 1f;
	[Export] public int   PierceCount = 1;   

	private Vector2 _direction;
	private float   _lifeTimer  = 0f;
	private int     _pierceLeft;

	public override void _Ready()
	{
		_pierceLeft = PierceCount;
		Scale = Vector2.One * BulletScale;
		BodyEntered += OnBodyEntered;
	}

	public void Initialize(Vector2 direction)
	{
		_direction = direction.Normalized();
		Rotation   = _direction.Angle();
	}

	public override void _Process(double delta)
	{
		GlobalPosition += _direction * Speed * (float)delta;

		_lifeTimer += (float)delta;
		if (_lifeTimer >= Lifetime)
			QueueFree();
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is IDamageable damageable)
		{
			damageable.TakeDamage(Damage);
			_pierceLeft--;
			if (_pierceLeft <= 0)
				QueueFree();
		}
	}
}
