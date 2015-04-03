using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeInUp : Sequence
    {
        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { }) + 100f;
        }

        public FadeInUp()
        {
            Name = "Fade in up";
            Add(

                new SetProperty("Visible", true) { Name = "Setting Visible" },

                new SetProperty("Alpha", 0f) { Name = "Setting Alpha" },

                new Parallel(

                    new Tween
                        {
                            Property = "Alpha",
                            Duration = 0.5f,
                            Easer = Linear.EaseIn,
                            StartValue = 0f,
                            //StartValueReader = new PropertyReader("Alpha"),
                            EndValue = 1f
                        },

                    new Tween
                        {
                            Property = "Y",
                            Duration = 0.5f,
                            Easer = Expo.EaseOut,
                            EndValueReader = new PropertyReader("Y"),
                            StartValueReaderFunction = StartValueReaderFunc
                        }
                    )

                    //new Action(delegate { Debug.Log("Finished"); })
                );
        }
    }
}