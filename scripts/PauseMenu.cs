using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public PackedScene OptionsPackedScene;
	
	private VBoxContainer _vBoxContainer;
	private AnimationPlayer _animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_vBoxContainer = (VBoxContainer)GetNode("VBoxContainer");
		_vBoxContainer.Hide();
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
		_vBoxContainer.Show();
		_animationPlayer.Play("blur");
	}

	private void Resume()
	{
		GetTree().Paused = false;
		_animationPlayer.PlayBackwards("blur");
		_vBoxContainer.Hide();
	}

	private void Restart()
	{
		Resume();
		GetTree().ReloadCurrentScene();
	}

	private void TestPauseAction()
	{
		if (Input.IsActionJustPressed("pause") && !GetTree().Paused) {
			Pause();
		} else if (Input.IsActionJustPressed("pause") && GetTree().Paused) {
			Resume();
		}
		/* else if (Input.IsActionJustPressed("ui_cancel")) {
			Resume();
		}*/
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
