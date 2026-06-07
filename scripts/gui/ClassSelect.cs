using Godot;
using System.Collections.Generic;

public partial class ClassSelect : Control
{
	// Retratos de cada clase — asignar en el Inspector en orden
	[Export] public Texture2D[] ClassPortraits;

	private int                _selectedIndex = 0;
	private List<TextureButton> _classButtons  = new();
	private List<ClassData>    _classes;

	private TextureRect _classPreview;
	private Label       _classNameLabel;
	private Label       _classDescLabel;
	private Label       _classBuffsLabel;
	private Label       _classWeaponLabel;
	private Button      _playButton;
	private Button      _backButton;
	private GridContainer _classGrid;

	private Panel        _selectionBorder;
	private StyleBoxFlat _styleSelected;

	public override void _Ready()
	{
		_classPreview   = GetNode<TextureRect>("RightPanel/VBox/ClassPreview");
		_classNameLabel = GetNode<Label>("RightPanel/VBox/ClassNameLabel");
		_classDescLabel = GetNode<Label>("RightPanel/VBox/ClassDescLabel");
		_classBuffsLabel  = GetNode<Label>("RightPanel/VBox/ClassBuffsLabel");
		_classWeaponLabel = GetNode<Label>("RightPanel/VBox/ClassWeaponLabel");
		_playButton     = GetNode<Button>("RightPanel/VBox/PlayButton");
		_backButton     = GetNode <Button>("BackButton");
		_classGrid      = GetNode<GridContainer>("LeftPanel/VBox/ScrollContainer/ClassGrid");

		_backButton.Pressed += OnBack;
		_playButton.Pressed += OnPlay;

		// Borde de selección
		_styleSelected = new StyleBoxFlat();
		_styleSelected.BgColor          = new Color(0, 0, 0, 0);
		_styleSelected.BorderColor       = new Color(1f, 0.85f, 0f);
		_styleSelected.BorderWidthTop    = _styleSelected.BorderWidthBottom =
		_styleSelected.BorderWidthLeft   = _styleSelected.BorderWidthRight  = 3;
		_styleSelected.CornerRadiusTopLeft = _styleSelected.CornerRadiusTopRight =
		_styleSelected.CornerRadiusBottomLeft = _styleSelected.CornerRadiusBottomRight = 4;

		_selectionBorder = new Panel();
		_selectionBorder.MouseFilter = Control.MouseFilterEnum.Ignore;
		_selectionBorder.AddThemeStyleboxOverride("panel", _styleSelected);
		GetNode("LeftPanel").AddChild(_selectionBorder);

		_classes = ClassLibrary.BuildAll();

		// Leer botones del grid
		foreach (Node child in _classGrid.GetChildren())
		{
			TextureButton btn = null;
			if (child is TextureButton tb)  btn = tb;
			else if (child is VBoxContainer vb) btn = vb.GetChildOrNull<TextureButton>(0);

			if (btn != null)
			{
				int captured = _classButtons.Count;
				btn.Pressed += () => SelectClass(captured);
				_classButtons.Add(btn);
			}
		}

		CallDeferred(nameof(SelectClass), 0);
	}

	private void SelectClass(int index)
	{
		if (index >= _classes.Count) return;
		_selectedIndex = index;

		// Mover borde
		if (index < _classButtons.Count)
		{
			var btn      = _classButtons[index];
			var leftPanel = GetNode<Control>("LeftPanel");
			_selectionBorder.Position = btn.GlobalPosition - leftPanel.GlobalPosition;
			_selectionBorder.Size     = btn.Size;
			_selectionBorder.Visible  = true;

			if (ClassPortraits != null && index < ClassPortraits.Length)
				_classPreview.Texture = ClassPortraits[index];
		}

		ClassData cls = _classes[index];
		_classNameLabel.Text  = cls.Name;
		_classDescLabel.Text  = cls.Description;
		_classBuffsLabel.Text = $"{cls.PassiveName}: {cls.PassiveDescription}";
		_classWeaponLabel.Text = $"Arma inicial: {cls.StartingWeaponName}";
	}
	
	private void OnBack()
	{
		GetTree().ChangeSceneToFile("res://scenes/ui/character_select.tscn");
	}
	
	private void OnPlay()
	{
		ClassData cls = _classes[_selectedIndex];
		GameManager.Instance.SelectedClassIndex = _selectedIndex;
		GameManager.Instance.SelectedPassive    = cls.Passive;
		GameManager.Instance.ResetRun();
		GetTree().ChangeSceneToFile("res://scenes/mapas/forest_map.tscn");
	}
}
