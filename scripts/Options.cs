using Godot;
using System;

public partial class Options : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var fullscreenCheckbox = GetNode<CheckBox>("VBoxContainer/NinePatchRect/MarginContainer/VBoxContainer/FullscreenCheckBox");
		fullscreenCheckbox.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnBackButtonPressed()
	{
		QueueFree();
	}

	private void _OnFullscreenCheckboxChanged(bool fullscreen)
	{
		DisplayServer.WindowSetMode(fullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
	}
}
