using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeInLeft : Sequence
    {
        private static object StartValueReaderFunc(object target)
        {
            return ((float)target.GetType().GetProperty("X").GetValue(target, new object[] { })) + 300;
        }

        public FadeInLeft()
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
                            EndValue = 1f
                        },

                    new Tween
                        {
                            Property = "X",
                            Duration = 0.8f,
                            Easer = Quart.EaseOut,
                            EndValueReader = new PropertyReader("X"),
                            StartValueReaderFunction = StartValueReaderFunc
                        }
                    )
                );
        }
    }
}