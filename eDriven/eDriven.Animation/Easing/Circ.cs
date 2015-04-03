using System;

namespace eDriven.Animation.Easing
{
    public class Circ
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            return (float) (-c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b);
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p) {
            return (float) (c * Math.Sqrt(1 - (t = t / d - 1) * t) + b);
	    }

	    public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
            if ((t /= d / 2) < 1) return (float) (-c / 2 * (Math.Sqrt(1 - t * t) - 1) + b);
            return (float) (c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b);
	    }
    }
}