using Godot;
using System;

public partial class BossEnemy : Enemy
{
	public override void _Ready()
	{
		base._Ready();
		Modulate = new Color(0.5f,0f,1f);
		Scale = new Vector2(3f,3f);
	}
	public override void Die(){
		GameManager.Instance.isBossDefeated = true;
		GD.Print("Boss derrotado");
		if(GameManager.Instance.currentMapLevel == 3)
		{
			EventManager.Instance.EmitGameWon();
		}
		base.Die();
	}
}
