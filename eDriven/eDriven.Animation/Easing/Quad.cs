namespace eDriven.Animation.Easing
{
    public class Quad
    {
// ReSharper disable UnusedMember.Local
        public static float EaseIn(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c*(t/=d)*t + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return -c * (t /= d) * (t - 2) + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseInOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }
    }
}