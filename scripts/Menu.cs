using Godot;
using System;

public partial class Menu : Control
{
	[Export] public PackedScene OptionsScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/world_nils.tscn");
	}
	
	private void OnOptionsButtonPressed()
	{
		var optionsScene = OptionsScene.Instantiate();
		GetTree().CurrentScene.AddChild(optionsScene);
	}
	
	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
