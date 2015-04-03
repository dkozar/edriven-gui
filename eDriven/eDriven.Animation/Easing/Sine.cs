using System;

namespace eDriven.Animation.Easing
{
    public class Sine
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            return (float) (-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p) {
            return (float) (c * Math.Sin(t / d * (Math.PI / 2)) + b);
	    }

	    public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
            return (float) (-c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b);
	    }
    }
}