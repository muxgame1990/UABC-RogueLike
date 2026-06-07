using Godot;

public partial class ShieldOrbital : OrbitalWeapon
{
	[Export] public float RechargeTime = 4f;
	public int ShieldIndex;
	public bool IsActive = true;
	private float rechargeTimer = 0f;
	private Sprite2D sprite;
	private CollisionShape2D collision;
public override void _Ready()
{
	sprite = GetNode<Sprite2D>("Sprite2D");
	collision = GetNode<CollisionShape2D>("CollisionShape2D");
	AddToGroup("shields");
}
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (!IsActive)
		{
			rechargeTimer -= (float)delta;
			if (rechargeTimer <= 0f)
			{
				RestoreShield();
			}
		}
	}
	public bool ConsumeShield()
	{
		if (!IsActive)
			return false;
		IsActive = false;
		sprite.Visible = false;
		collision.Disabled = true;
		rechargeTimer = RechargeTime;
		return true;
	}
	private void RestoreShield()
	{
		IsActive = true;
		sprite.Visible = true;
		collision.Disabled = false;
	}
	public override void SetOrbitData(
		float angleOffset,
		float orbitRadius,
		WeaponData data)
	{
		base.SetOrbitData(
			angleOffset,
			45f,
			data
		);
	}
	protected override void UpdateOrbit()
	{
		base.UpdateOrbit();
		Rotation = 0f;
	}
}
