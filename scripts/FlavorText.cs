using Godot;
using System;
using System.Collections.Generic;

public partial class FlavorText : Label
{
	private static readonly string[] FlavorTexts = {
		"Have you tried dodging?",
		"Skill issue \ud83d\ude14",
		"Skill issue fr fr ong no 🧢",
		"Interesting technique...",
		"It was lag, I swear!",
		"I meant to do that.",
		"You looked away 0.03 seconds too long.",
		"You're supposed to avoid these things, not collect them.",
		"Sorry to burst your bubble, but you're dead.",
		"Knock knock. Who's there? Death.",
		"WEEEEEEEEEEEEEEEEEE",
		"You dashed too close to the sun.",
		"Have you tried eating soap?",
		"Help they're forcing me to write these flavor texts!!!",
		"You did done died",
		"Should've shot more bubbles n'stuff",
		"I recommend not dying.",
		"Haha we have less than two hours left and I'm writing this stupid flavor text"
	};
	
	private static readonly List<string> Unseen = new(FlavorTexts);
	
	public override void _Ready()
	{
		if (Unseen.Count == 0) Unseen.AddRange(FlavorTexts);
		var randomText = Unseen[Random.Shared.Next(0, Unseen.Count)];
		Unseen.Remove(randomText);
		Text = $"\"{randomText}\"";
	}

}
