using Godot;
using System;

#nullable enable

public partial class PauseMenu : Control
{
	[Export] public PackedScene OptionsPackedScene;
	// [Export] public CanvasLayer HealthBarLayer;
	
	private AnimationPlayer _animationPlayer;
	private Options? _options;
	
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
		TestPauseAction();
	}

	private void Pause()
	{
		GetTree().Paused = true;
		Show();
		_animationPlayer.Play("blur");
	}

	private void Resume()
	{
		GetTree().Paused = false;
		_animationPlayer.PlayBackwards("blur");
		Hide();
	}

	private void Restart()
	{
		Resume();
		GetTree().ReloadCurrentScene();
	}

	private void BackToMainMenu()
	{
		Resume();
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}

	private void OpenOptions()
	{
		_options = OptionsPackedScene.Instantiate<Options>();
		GetParent().AddChild(_options);
	}

	private void TestPauseAction()
	{
		if (!Input.IsActionJustPressed("pause")) return;
		if (!GetTree().Paused) {
			Pause();
		} else if (GetTree().Paused && Visible && !IsInstanceValid(_options)) {
			Resume();
		}
	}

	private void _OnResumeButtonPressed()
	{
		Resume();
	}

	private void _OnRestartButtonPressed()
	{
		Restart();
	}

	private void _OnOptionsButtonPressed()
	{
		OpenOptions();
	}

	private void _OnMainMenuButtonPressed()
	{
		BackToMainMenu();
	}
	
	private void _OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
