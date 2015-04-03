using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using UnityEngine;

namespace Assets.eDriven.Demo.Tweens
{
    public class Jumpy : Parallel
    {
        public Jumpy()
        {
            Add(
                new Tween
                {
                    Property = "Alpha",
                    Duration = 0.6f,
                    Easer = Linear.EaseNone,
                    StartValue = 0f,
                    EndValue = 1f
                },

                new Tween
                {
                    Property = "Scale",
                    Interpolator = new Vector2Interpolator(),
                    Duration = 0.8f,
                    Easer = Elastic.EaseOut,
                    StartValue = new Vector2(0f, 0.5f),
                    EndValue = new Vector2(1f, 1f)
                }
            );
        }
    }
}