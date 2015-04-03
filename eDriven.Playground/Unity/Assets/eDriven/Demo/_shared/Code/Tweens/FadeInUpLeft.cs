using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Geom;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeInUpLeft : Sequence
    {
        private static object StartValueReaderFunc(object target)
        {
            return ((Point)target.GetType().GetProperty("Position").GetValue(target, new object[] { })).Add(new Point(400, 400));
        }

        public FadeInUpLeft()
        {
            Add(

                new SetProperty("Visible", true),

                new SetProperty("Alpha", 0f),

                new Parallel(

                    new Tween
                        {
                            Property = "Alpha",
                            Duration = 0.4f,
                            Easer = Linear.EaseIn,
                            StartValue = 0f,
                            //StartValueReader = new PropertyReader("Alpha"),
                            EndValue = 1f
                        },

                    new Tween
                        {
                            Property = "Position",
                            Duration = 0.7f,
                            Easer = Quart.EaseOut,
                            EndValueReader = new PropertyReader("Position"),
                            StartValueReaderFunction = StartValueReaderFunc
                        }
                    )

                //new Action(delegate { Debug.Log("Finished"); })
                );
        }
    }
}