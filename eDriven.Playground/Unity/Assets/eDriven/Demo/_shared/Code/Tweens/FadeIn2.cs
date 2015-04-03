using eDriven.Animation;
using eDriven.Animation.Easing;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeIn2 : Parallel
    {
        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { }) + 100;
        }

        private static object EndValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { });
        }

        public FadeIn2()
        {
            Add(
                new Tween
                {
                    Property = "Alpha",
                    Duration = 0.5f,
                    Easer = Linear.EaseNone,
                    StartValue = 0f,
                    EndValue = 1f
                },
                new Tween
                {
                    Property = "Y",
                    Duration = 0.5f,
                    Easer = Expo.EaseOut,
                    StartValueReaderFunction = StartValueReaderFunc,
                    EndValueReaderFunction = EndValueReaderFunc
                }
            );
        }
    }
}