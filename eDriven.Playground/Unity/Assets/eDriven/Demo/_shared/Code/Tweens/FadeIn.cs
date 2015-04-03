using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeIn : Tween
    {
        public FadeIn()
        {
            Property = "Alpha";
            Duration = 0.5f;
            Easer = Linear.EaseNone;
            StartValue = 0f;
            //StartValueReader = new PropertyReader("Alpha");
            EndValue = 1f;
        }
    }
}