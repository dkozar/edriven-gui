using System;

namespace eDriven.Animation.Easing
{
    public class Expo
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            return (float) ((t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b);
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p) {
            return (float) ((t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b);
	    }

	    public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return (float) (c / 2 * Math.Pow(2, 10 * (t - 1)) + b);
            return (float) (c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b);
	    }
    }
}