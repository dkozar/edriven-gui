using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class ZeroFadeIn : Sequence
    {
        public ZeroFadeIn()
        {
            Add(

                new SetProperty("Visible", true),

                new SetProperty("Alpha", 0f),

                new Tween
                {
                    Property = "Alpha",
                    Duration = 0.35f,
                    Easer = Linear.EaseNone,
                    StartValue = 0f,
                    //StartValueReader = new PropertyReader("Alpha"),
                    EndValue = 1f
                }
            );
        }
    }
}