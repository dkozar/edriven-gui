using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Reflection;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeInUp2 : Sequence
    {
        public float Offset = 30f;
        
        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { });
        }

        private object EndValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("Y").GetValue(target, new object[] { }) - Offset;
        }

        public FadeInUp2()
        {
            Name = "Fade in up";
            Add(

                new SetProperty("Visible", true) { Name = "Setting Visible" },

                new SetProperty("Alpha", 0f) { Name = "Setting Alpha" },

                new SetPropertyFunc(delegate(object target)
                {
                    CoreReflector.SetValue(target, "Y", (float)CoreReflector.GetValue(target, "Y") + Offset * 2);
                }),

                new Sequence(

                    new Parallel(
                        new Tween
                        {
                            Property = "Alpha",
                            Duration = 0.3f,
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
                            StartValueReaderFunction = StartValueReaderFunc,
                            EndValueReaderFunction = EndValueReaderFunc
                        }
                    ),

                    new Tween
                    {
                        Property = "Y",
                        Duration = 0.5f,
                        Easer = Expo.EaseInOut,
                        StartValueReaderFunction = StartValueReaderFunc,
                        EndValueReaderFunction = EndValueReaderFunc
                    }
                )

                //new Action(delegate { Debug.Log("Finished"); })
            );
        }
    }
}