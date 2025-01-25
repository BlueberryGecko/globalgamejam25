using System;
using Godot;

namespace Globalgamejam25;

public static class Util {
	private static Random r = new Random();
    
	public static float getRandomFloat(float min, float max) => r.NextSingle() * (max - min) + min;
	public static float getRandomFloat(float max) => r.NextSingle() * max;

	public static Rect2 plus(this Rect2 rect, Vector2 v) => new(rect.Position + v, rect.Size);

	/// <summary>
	/// Sample a point on a rectangular border
	/// </summary>
	public static Vector2 samplePointOnRect(Rect2 rect) {
		var circumference = rect.Size.X * 2 + rect.Size.Y * 2;
		var f = r.NextSingle() * circumference; // sample number in [0, circumference) and "project" it onto the rectangle counterclockwise
		if (f < rect.Size.Y) {
			return new Vector2(0, f) + rect.Position;
		}
		f -= rect.Size.Y;
		if (f < rect.Size.X) {
			return new Vector2(f, rect.Size.Y) + rect.Position;
		}
		f -= rect.Size.X;
		if (f < rect.Size.Y) {
			return new Vector2(rect.Size.X, rect.Size.Y-f) + rect.Position;
		}
		f -= rect.Size.Y;
		return new Vector2(rect.Size.X-f, 0) + rect.Position;
	}
	
	/// <summary>
	/// Let a node blink. Return tween for further modification.
	/// </summary>
	public static PropertyTweener BlinkWithTween(this Node2D n) {
		var blinkDuration = 0.2;
		n.Modulate = new Color(2f, 2f, 2f);
		var t = n.CreateTween().TweenProperty(n, nameof(n.Modulate).ToLower(), Colors.White, blinkDuration).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
		return t;
	}
	
}