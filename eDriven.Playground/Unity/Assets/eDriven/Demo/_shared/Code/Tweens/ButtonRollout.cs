using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using UnityEngine;

namespace Assets.eDriven.Demo.Tweens
{
    public class ButtonRollout : Parallel
    {
        private static object StartValueReaderFunc(object target)
        {
            return ReadValue(target, "Scale");
        }

        public ButtonRollout()
        {
            Add(
//                new Tween
//                {
//                    Property = "Alpha",
//                    Duration = 0.3f,
//                    Easer = Linear.EaseNone,
//                    StartValue = 0f,
//                    EndValue = 1f
//                },
                new Tween
                {
                    Property = "Scale",
                    Interpolator = new Vector2Interpolator(),
                    Duration = 0.4f,
                    Easer = Quint.EaseOut,
                    StartValueReaderFunction = StartValueReaderFunc,
                    EndValue = new Vector2(1f, 1f)
                }
            );
        }
    }
}