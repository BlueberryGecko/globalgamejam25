using Godot;
using System;

public partial class Options : Control
{
	[Export] public CheckBox FullscreenCheckBox;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FullscreenCheckBox.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TestUiCancel();
	}

	private void Close()
	{
		QueueFree();
	}

	private void SetFullScreen(bool fullscreen)
	{
		DisplayServer.WindowSetMode(fullscreen ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
	}

	private void TestUiCancel()
	{
		if (Input.IsActionJustPressed("ui_cancel")) {
			Close();
		}
	}

	private void _OnBackButtonPressed()
	{
		Close();
	}

	private void _OnFullscreenCheckboxChanged(bool fullscreen)
	{
		SetFullScreen(fullscreen);
	}
}
