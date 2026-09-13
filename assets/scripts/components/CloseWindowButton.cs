using Godot;
using System;

public partial class CloseWindowButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += _ButtonPressed;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void _ButtonPressed()
	{
		debugLog.Log("Window «" + GetWindow().Name + "» closed");
		GetWindow().QueueFree();
	}
}
