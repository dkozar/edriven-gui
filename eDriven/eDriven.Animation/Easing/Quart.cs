namespace eDriven.Animation.Easing
{
    public class Quart
    {
// ReSharper disable UnusedMember.Local
        public static float EaseIn(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c * (t /= d) * t * t * t + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseInOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
    }
}