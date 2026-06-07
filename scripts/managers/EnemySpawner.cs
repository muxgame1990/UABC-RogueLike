using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene[] MeleeEnemies;
	[Export] public PackedScene[] RangedEnemies;
	[Export] public float SpawnRadius       = 400f;
	[Export] public float BaseSpawnInterval = 0.5f; //Normalmente es 2f
	[Export] public float RangedSpawnChance = 0.3f;
	[Export] public float waveDuration = 30f;
	[Export] public float waveCooldown = 180f;
	[Export] public float waveSpawnInterval = 0.01f;
	public bool isWaveActive = false;
	public float waveTimer = 0f;
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
		waveTimer += (float)delta;
		if(!isWaveActive)
		{
			if(waveTimer >= waveCooldown)
			{
				isWaveActive = true;
				waveTimer = 0f;
				GD.Print("Iniciamo la oleada");
			}
		}
		else
		{
			if(waveTimer >= waveDuration)
			{
				isWaveActive = false;
				waveTimer = 0f;
				GD.Print("Acaba la oleada");
			}
		}
		float timeFactor = GameManager.Instance.ElapsedTime / 180f;
		float diffFactor = GameManager.Instance.diffModifier;
		float normalInterval = Mathf.Max(0.2f, BaseSpawnInterval - (timeFactor * diffFactor));
		_currentInterval = isWaveActive ? waveSpawnInterval : normalInterval;

		if (_spawnTimer >= _currentInterval)
		{
			_spawnTimer = 0f;
			SpawnEnemy();
		}
	}

	private void SpawnEnemy()
	{
		bool isElite = GD.Randf() < GameManager.Instance.eliteProbability;
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
			SpawnRanged(chosenScene, spawnPosition, scale,isElite);
		else
			SpawnMelee(chosenScene, spawnPosition, scale,isElite);
	}

	private void SpawnMelee(PackedScene scene, Vector2 position, float scale,bool isElite)
	{
		Enemy enemy  = scene.Instantiate<Enemy>();
		float eliteMultiplier = isElite ? 3.0f : 1.0f;
		float scaleMultiplier = scale * GameManager.Instance.diffModifier * eliteMultiplier;
		enemy.MaxHp  *= scaleMultiplier;
		enemy.Speed  *= Mathf.Min(scaleMultiplier, 2f);
		enemy.Damage *= scaleMultiplier;
		if (isElite)
			{
				enemy.Modulate = new Color(1, 0, 0);
				enemy.Scale *= 2f;
				enemy.XpValue *= 3;
			}
		_player.GetParent().AddChild(enemy);
		enemy.GlobalPosition = position;
	}

	private void SpawnRanged(PackedScene scene, Vector2 position, float scale, bool isElite)
	{
		RangedEnemy enemy = scene.Instantiate<RangedEnemy>();
		float eliteMultiplier = isElite ? 1.5f : 1.0f;
		float scaleMultiplier = scale * GameManager.Instance.diffModifier * eliteMultiplier;
		enemy.MaxHp       *= scaleMultiplier;
		enemy.Speed       *= Mathf.Min(scaleMultiplier, 2f);
		enemy.Damage      *= scaleMultiplier;
			if (isElite)
			{
				enemy.Modulate = new Color(1, 0, 0);
				enemy.Scale *= 2f;
				enemy.XpValue *= 3;
			}
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
