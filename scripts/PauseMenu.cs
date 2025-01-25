using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public PackedScene optionsPackedScene;
	[Export] public PackedScene mainMenuPackedScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void OnResumeButtonPressed()
	{
		QueueFree();
	}

	private void OnOptionsButtonPressed()
	{
		var optionsScene = optionsPackedScene.Instantiate();
		GetTree().CurrentScene.AddChild(optionsScene);
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToPacked(mainMenuPackedScene);
	}
	
	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
