using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using eDriven.Audio;
using UnityEngine;

namespace Assets.eDriven.Demo.Tweens
{
    public class ButtonRollover : Parallel
    {
        private static object StartValueReaderFunc(object target)
        {
            return ReadValue(target, "Scale");
        }

        public ButtonRollover()
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
                new Action(delegate(IAnimation anim)
                {
                    AudioPlayerMapper.GetDefault().PlaySound("portlet_add");
                }),
                new Tween
                {
                    Property = "Scale",
                    Interpolator = new Vector2Interpolator(),
                    Duration = 0.4f,
                    Easer = Quint.EaseOut,
                    StartValueReaderFunction = StartValueReaderFunc,
                    EndValue = new Vector2(1.1f, 1.1f)
                }
            );
        }
    }
}