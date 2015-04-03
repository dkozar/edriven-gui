using Assets.eDriven.Demo.Tweens.Interpolators;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Geom;

namespace Assets.eDriven.Demo.Tweens
{
    public class FlyToLeft : Sequence
    {
        public float XOffset = 800;

        private object StartValueReaderFunc(object target)
        {
            return ((Point)target.GetType().GetProperty("Position").GetValue(target, new object[] { })).Add(new Point(XOffset, 0));
        }

        public FlyToLeft()
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
                        Interpolator = new FlyingBeeInterpolator(),
                        Duration = 1.5f,
                        Easer = Quart.EaseOut,
                        EndValueReader = new PropertyReader("Position"),
                        StartValueReaderFunction = StartValueReaderFunc
                    }
                )
            );
        }
    }
}