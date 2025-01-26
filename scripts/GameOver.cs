using Godot;
using System;

public partial class GameOver : Control
{
	[Export] public PackedScene OptionsPackedScene;
	// [Export] public CanvasLayer HealthBarLayer;
	
	private AnimationPlayer _animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.Play("RESET");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Initiate()
	{
		Pause();
	}

	private void Pause()
	{
		GetTree().Paused = true;
		Show();
		// HealthBarLayer.Hide();
		_animationPlayer.Play("blur");
	}

	private void Resume()
	{
		GetTree().Paused = false;
		_animationPlayer.PlayBackwards("blur");
		Hide();
		// HealthBarLayer.Show();
	}
	
	private void Restart()
	{
		Resume();
		GetTree().ReloadCurrentScene();
	}

	private void _OnRetryButtonPressed()
	{
		Restart();
	}

	private void _OnOptionsButtonPressed()
	{
		var optionsScene = OptionsPackedScene.Instantiate();
		GetParent().AddChild(optionsScene);
	}

	private void _OnMainMenuButtonPressed()
	{
		Resume();
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
	
	private void _OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
