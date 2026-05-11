using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 150f;
	[Export] public float MaxHp = 100f;

	[Export] public string AnimIdle  = "idle";
	[Export] public string AnimWalk  = "walk";

	public float CurrentHp { get; private set; }

	private AnimatedSprite2D _sprite;
	private bool _isDead = false;

	public override void _Ready()
	{
		int idx = GameManager.Instance.SelectedCharacterIndex;
		float[] speeds  = { 200f, 120f, 150f, 130f, 180f };
		float[] maxHps  = {  80f, 150f, 100f, 120f,  90f };

		if (idx < speeds.Length) Speed = speeds[idx];
		if (idx < maxHps.Length) MaxHp = maxHps[idx];

		CurrentHp = MaxHp;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AddToGroup("player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead) return;

		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			_sprite.Play(AnimWalk);
			if (direction.X != 0)
				_sprite.FlipH = direction.X < 0;
		}
		else
		{
			Velocity = Vector2.Zero;
			_sprite.Play(AnimIdle);
		}

		MoveAndSlide();
	}

	public void TakeDamage(float amount)
	{
		if (_isDead) return;
		CurrentHp -= amount;
		CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);
		if (CurrentHp <= 0) Die();
	}

	public void Heal(float amount)
	{
		CurrentHp = Mathf.Clamp(CurrentHp + amount, 0, MaxHp);
	}

	private void Die()
	{
		_isDead = true;
		GameManager.Instance.CallDeferred(
			GodotObject.MethodName.EmitSignal,
			GameManager.SignalName.PlayerDied
		);
	}
}
