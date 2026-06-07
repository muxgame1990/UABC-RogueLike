using Godot;

public partial class DialogoNPC : Area2D
{
	private Sprite2D dialogo;

	public override void _Ready()
	{
		GD.Print("Si se preparo el dialogo");

		dialogo = GetNode<Sprite2D>("../SpriteDialogo");
		dialogo.Visible = false;

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body)
	{
		GD.Print("Se acerco al NPC");

		if (body.Name == "CharacterBody2D")
		{
			dialogo.Visible = true;
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body.Name == "CharacterBody2D")
		{
			dialogo.Visible = false;
		}
	}
}
