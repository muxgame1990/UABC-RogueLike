using Godot;
using System.Collections.Generic;

public partial class CharacterSelect : Control
{
	//datos de cada personaje
	private readonly string[] _names        = { "Cimarron", "Abad", "Edwin", "Anguiano", "Bosch" };
	private readonly string[] _descriptions = { "No hay papel...", "Muchachoos", "Jovenes se me olvidaron los examenes", "Busquenlo en la pagina de ProfesorIvanAnguiano", "Arrastren el lapiz cabezas" };
	private readonly float[]  _speeds       = { 200f, 120f, 150f, 130f, 180f };
	private readonly float[]  _maxHps       = {  80f, 150f, 100f, 120f,  90f };
	private readonly float[]  _damages      = {  25f,  15f,  20f,  30f,  22f };
	
	private readonly string[] _scenePaths = {
		"res://scenes/player/cimarron_char.tscn",
		"res://scenes/player/abad_char.tscn",
		"res://scenes/player/edwin_char.tscn",
		"res://scenes/player/anguiano_char.tscn",
	};
	
	private readonly string[] _defaultWeapons = {
		"res://scenes/weapons/pencil_weapon.tscn",  // Cimarron
		"res://scenes/weapons/revolver_weapon.tscn",  // Abad
		"res://scenes/weapons/bone_weapon.tscn",  // Edwin
		"res://scenes/weapons/axe_weapon.tscn", //Anguiano
		"res://scenes/weapons/pencil_weapon.tscn", //Bosch
	};
	
	private int _selectedIndex = 0;
	private List<TextureButton> _portraitButtons = new();

	private TextureRect   _charPreview;
	private Label         _charNameLabel;
	private Label         _charDescLabel;
	private Label         _charStatsLabel;
	private Button        _confirmButton;
	private GridContainer _charGrid;
	private Button        _backButton;

	private Panel _selectionBorder;

	public override void _Ready()
	{
		_charPreview    = GetNode<TextureRect>("RightPanel/VBoxContainer/CharPreview");
		_charNameLabel  = GetNode<Label>("RightPanel/VBoxContainer/CharNameLabel");
		_charDescLabel  = GetNode<Label>("RightPanel/VBoxContainer/CharDescLabel");
		_charStatsLabel = GetNode<Label>("RightPanel/VBoxContainer/CharStatsLabel");
		_confirmButton  = GetNode<Button>("RightPanel/VBoxContainer/ConfirmButton");
		_charGrid       = GetNode<GridContainer>("LeftPanel/VBoxContainer/ScrollContainer/CharGrid");
		_backButton     = GetNode<Button>("BackButton");
		
		_backButton.Pressed += OnBack;
		_confirmButton.Pressed += OnConfirm;

		//Crear el panel de borde una sola vez y añadirlo a la escena
		_selectionBorder = new Panel();
		_selectionBorder.MouseFilter = Control.MouseFilterEnum.Ignore; // no bloquea clicks

		var style = new StyleBoxFlat();
		style.BgColor           = new Color(0, 0, 0, 0); // transparente
		style.BorderColor       = new Color(1f, 0.85f, 0f); // amarillo
		style.BorderWidthTop    = 3;
		style.BorderWidthBottom = 3;
		style.BorderWidthLeft   = 3;
		style.BorderWidthRight  = 3;
		style.CornerRadiusTopLeft     = 4;
		style.CornerRadiusTopRight    = 4;
		style.CornerRadiusBottomLeft  = 4;
		style.CornerRadiusBottomRight = 4;
		_selectionBorder.AddThemeStyleboxOverride("panel", style);

		//Añadirlo encima de todo en el LeftPanel
		GetNode("LeftPanel").AddChild(_selectionBorder);

		//Leer botones existentes de la escena
		foreach (Node child in _charGrid.GetChildren())
		{
			TextureButton btn = null;

			if (child is TextureButton tb)
				btn = tb;
			else if (child is VBoxContainer vbox)
				btn = vbox.GetChildOrNull<TextureButton>(0);

			if (btn != null)
			{
				int captured = _portraitButtons.Count;
				btn.Pressed += () => SelectCharacter(captured);
				_portraitButtons.Add(btn);
			}
		}

		//Esperar un frame para que los botones tengan su posición calculada
		CallDeferred(nameof(SelectCharacter), 0);
	}

	private void SelectCharacter(int index)
	{
		_selectedIndex = index;

		//Mover el panel de borde al botón seleccionado
		if (index < _portraitButtons.Count)
		{
			var btn = _portraitButtons[index];
			var leftPanel = GetNode<Control>("LeftPanel");

			// Convertir la posición global del botón a local del LeftPanel
			Vector2 globalPos = btn.GlobalPosition;
			Vector2 localPos = globalPos - leftPanel.GlobalPosition;

			_selectionBorder.Position = localPos;
			_selectionBorder.Size     = btn.Size;
			_selectionBorder.Visible  = true;

			// Preview del personaje
			_charPreview.Texture = btn.TextureNormal;
		}

		_charNameLabel.Text = index < _names.Length        ? _names[index]        : "???";
		_charDescLabel.Text = index < _descriptions.Length ? _descriptions[index] : "";

		float spd = index < _speeds.Length  ? _speeds[index]  : 150f;
		float hp  = index < _maxHps.Length  ? _maxHps[index]  : 100f;
		float dmg = index < _damages.Length ? _damages[index] : 20f;

		_charStatsLabel.Text =
			$"Velocidad:  {spd}\n" +
			$"Vida max:   {hp}\n" +
			$"Daño:       {dmg}";
	}

	private void OnConfirm()
	{
		GameManager.Instance.SelectedCharacterIndex = _selectedIndex;
		if (_selectedIndex < _scenePaths.Length)
			GameManager.Instance.SelectedCharacterScene = _scenePaths[_selectedIndex];

		//GameManager.Instance.ResetRun();
		GetTree().ChangeSceneToFile("res://scenes/ui/class_select.tscn");
	}
	
	private void OnBack()
	{
		GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
	}
}
