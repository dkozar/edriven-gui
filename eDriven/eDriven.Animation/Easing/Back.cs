namespace eDriven.Animation.Easing
{
    public class Back
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            if (0 == a) a = 1.70158f;
            return c * (t /= d) * t * ((a + 1) * t - a) + b;
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p) {
            if (0 == a) a = 1.70158f;
            return c * ((t = t / d - 1) * t * ((a + 1) * t + a) + 1) + b;
	    }

	    public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
            if (0 == a) a = 1.70158f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((a *= ((float)1.525)) + 1) * t - a)) + b;
            return c / 2 * ((t -= 2) * t * (((a *= ((float)1.525)) + 1) * t + a) + 2) + b;
	    }
    }
}