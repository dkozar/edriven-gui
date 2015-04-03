using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Reflection;

namespace Assets.eDriven.Demo.Tweens
{
    public class FadeInLeft2 : Sequence
    {
        public float Offset = 30f;

        private static object StartValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("X").GetValue(target, new object[] { });
        }

        private object EndValueReaderFunc(object target)
        {
            return (float)target.GetType().GetProperty("X").GetValue(target, new object[] { }) - Offset;
        }

        public FadeInLeft2()
        {
            Name = "Fade in left";
            
            Add(

                new SetProperty("Visible", true) { Name = "Setting Visible" },

                new SetProperty("Alpha", 0f) { Name = "Setting Alpha" },

                new SetPropertyFunc(delegate(object target)
                {
                    CoreReflector.SetValue(target, "X", (float)CoreReflector.GetValue(target, "X") + Offset);
                }),

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
                        Property = "X",
                        Duration = 0.8f,
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