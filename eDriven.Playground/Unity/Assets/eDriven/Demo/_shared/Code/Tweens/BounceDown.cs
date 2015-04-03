using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class BounceDown : Sequence
    {
        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { }) - 100f;
        }

        public BounceDown()
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
                            Property = "Y",
                            Duration = 0.8f,
                            Easer = Bounce.EaseOut, // Expo.EaseOut,
                            EndValueReader = new PropertyReader("Y"),
                            StartValueReaderFunction = StartValueReaderFunc
                        }
                    )

                    //new Action(delegate { Debug.Log("Finished"); })
                );
        }
    }
}