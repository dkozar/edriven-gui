using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class ZeroFadeOut : Sequence
    {
        public ZeroFadeOut()
        {
            Add(

                new Tween
                    {
                        Property = "Alpha",
                        Duration = 1f,
                        Easer = Linear.EaseNone,
                        //StartValue = 0f,
                        StartValueReader = new PropertyReader("Alpha"),
                        EndValue = 0f
                    },

                new SetProperty("Visible", false)

                //new Action(delegate { Debug.Log("Finished"); })
            );
        }
    }
}