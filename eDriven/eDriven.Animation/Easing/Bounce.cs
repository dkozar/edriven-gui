namespace eDriven.Animation.Easing
{
    public class Bounce
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            return c - EaseOut(d - t, 0, c, d, a, p) + b;
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p)
	    {
	        if ((t /= d) < (1 / 2.75))
            {
                return (float) (c * (7.5625 * t * t) + b);
            }
	        if (t < (2 / 2.75))
	        {
                return (float) (c * (7.5625 * (t -= (float)(1.5 / 2.75)) * t + .75) + b);
	        }
	        if (t < (2.5 / 2.75))
	        {
                return (float) (c * (7.5625 * (t -= (float)(2.25 / 2.75)) * t + .9375) + b);
	        }
	        return (float) (c * (7.5625 * (t -= (float)(2.625 / 2.75)) * t + .984375) + b);
	    }

        public static float EaseInOut (float t, float b, float c, float d, float a, float p)
        {
            if (t < d/2) return (float) (EaseIn(t*2, 0, c, d, a, p) * .5 + b);
            return (float) (EaseOut(t * 2 - d, 0, c, d, a, p) * .5 + c * .5 + b);
        }
    }
}