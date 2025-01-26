using Godot;
using System;
using Globalgamejam25.scripts;

public partial class GameOver : Control
{
	[Export] public Label ScoreLabel;
	
	private AnimationPlayer _animationPlayer;
	private Options _options;
	
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
		if (Visible) TestUiCancel();
	}

	public void Initiate()
	{
		Pause();
	}

	private void Pause()
	{
		GetTree().Paused = true;
		Show();
		ScoreLabel.Text = Consts.world.player.Score.ToString();
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

	private void TestUiCancel()
	{
		if (IsInstanceValid(_options)) return;
		if (!Input.IsActionJustPressed("ui_cancel")) return;
		BackToMainMenu();
	}

	private void _OnRetryButtonPressed()
	{
		Restart();
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
