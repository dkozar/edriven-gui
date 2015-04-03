namespace eDriven.Animation.Easing
{
    public class Linear
    {
// ReSharper disable UnusedMember.Local
        public static float EaseNone(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c * t / d + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseIn(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c * t / d + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c * t / d + b;
        }
// ReSharper disable UnusedMember.Local
        public static float EaseInOut(float t, float b, float c, float d, float a, float p)
// ReSharper restore UnusedMember.Local
        {
            return c * t / d + b;
        }
    }
}