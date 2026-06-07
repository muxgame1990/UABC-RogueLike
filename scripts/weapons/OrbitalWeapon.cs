using Godot;

public partial class OrbitalWeapon : Area2D
{
	[Export] public float OrbitRadius = 80f;
	[Export] public float OrbitSpeed = 3f;
	[Export] public float DamageCooldown = 0.05f; //0.1 original

	public WeaponData Data;
	protected Player Player;
	protected float AngleOffset;
	public virtual void Initialize(float startAngle, WeaponData data)
	{
		AngleOffset = startAngle;
		Data = data;
	}
	public virtual void SetOrbitData(
		float angleOffset,
		float orbitRadius,
		WeaponData data)
	{
		AngleOffset = angleOffset;
		OrbitRadius = orbitRadius;
		Data = data;
	}
	public override void _Process(double delta)
	{
		if (Player == null)
		{
			Player = GetTree().GetFirstNodeInGroup("player") as Player;
			return;
		}
			Scale = Vector2.One * Data.BulletScale * Player.Stats.SizeMultiplier;
		UpdateOrbit();
	}
	
	protected virtual void UpdateOrbit()
	{
		float baseAngle = GameManager.Instance.ElapsedTime * (OrbitSpeed 
		* Player.Stats.AttackSpeedMultiplier);
		
		float finalAngle = baseAngle + AngleOffset;
		
		GlobalPosition = Player.GlobalPosition +new Vector2( Mathf.Cos(finalAngle) 
		* OrbitRadius,Mathf.Sin(finalAngle) 
		* OrbitRadius);
		
		Rotation = finalAngle + Mathf.Pi / 2f;
	}
}
