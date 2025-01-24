using System;

namespace Globalgamejam25;

public static class Util {
	private static Random r = new Random();
    
	public static float getRandomFloat(float min, float max) => r.NextSingle() * (max - min) + min;
	public static float getRandomFloat(float max) => r.NextSingle() * max;
}