using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene[] MeleeEnemies;
	[Export] public PackedScene[] RangedEnemies;

	[Export] public float SpawnRadius       = 400f;
	[Export] public float BaseSpawnInterval = 2f; // original 2
	[Export] public float RangedSpawnChance = 0.3f;

	private float  _spawnTimer      = 0f;
	private float  _currentInterval;
	private Player _player;

	public override void _Ready()
	{
		_player          = GetTree().GetFirstNodeInGroup("player") as Player;
		_currentInterval = BaseSpawnInterval;
	}

	public override void _Process(double delta)
	{
		if (_player == null)
		{
			_player = GetTree().GetFirstNodeInGroup("player") as Player;
			return;
		}

		_spawnTimer += (float)delta;
		_currentInterval = Mathf.Max(0.3f, BaseSpawnInterval - GameManager.Instance.ElapsedTime / 180f); //180

		if (_spawnTimer >= _currentInterval)
		{
			_spawnTimer = 0f;
			SpawnEnemy();
		}
	}

	private void SpawnEnemy()
	{
		bool spawnRanged = RangedEnemies != null
						   && RangedEnemies.Length > 0
						   && GD.Randf() < RangedSpawnChance;

		PackedScene[] pool = spawnRanged ? RangedEnemies : MeleeEnemies;

		if (pool == null || pool.Length == 0) return;

		PackedScene chosenScene = pool[GD.RandRange(0, pool.Length - 1)];
		if (chosenScene == null) return;

		Vector2 spawnPosition = GetSpawnPosition();
		float   scale         = 1f + GameManager.Instance.ElapsedTime / 280f; //120

		if (spawnRanged)
			SpawnRanged(chosenScene, spawnPosition, scale);
		else
			SpawnMelee(chosenScene, spawnPosition, scale);
	}

	private void SpawnMelee(PackedScene scene, Vector2 position, float scale)
	{
		Enemy enemy  = scene.Instantiate<Enemy>();
		enemy.MaxHp  *= scale;
		enemy.Speed  *= Mathf.Min(scale, 2f);
		enemy.Damage *= scale;
		_player.GetParent().AddChild(enemy);
		enemy.GlobalPosition = position;
	}

	private void SpawnRanged(PackedScene scene, Vector2 position, float scale)
	{
		RangedEnemy enemy = scene.Instantiate<RangedEnemy>();
		enemy.MaxHp       *= scale;
		enemy.Speed       *= Mathf.Min(scale, 2f);
		enemy.Damage      *= scale;
		_player.GetParent().AddChild(enemy);
		enemy.GlobalPosition = position;
	}

	private Vector2 GetSpawnPosition()
	{
		float angle = GD.Randf() * Mathf.Tau;
		return _player.GlobalPosition + new Vector2(
			Mathf.Cos(angle) * SpawnRadius,
			Mathf.Sin(angle) * SpawnRadius
		);
	}
}
