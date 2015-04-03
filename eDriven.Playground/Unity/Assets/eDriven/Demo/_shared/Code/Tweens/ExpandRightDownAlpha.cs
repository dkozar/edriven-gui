using eDriven.Animation;
using eDriven.Animation.Easing;
using UnityEngine;

namespace Assets.eDriven.Demo.Tweens
{
    public class ExpandRightDownAlpha : Sequence
    {
        public Rect Bounds;

        public ExpandRightDownAlpha()
        {
            Add(

                // expand width & height to 400x400 first
                //new Parallel(

                //    Tween.New()
                //        .SetProperty("Width")
                //        .SetOptions(
                //        new TweenOption(TweenOptionType.Duration, 1.3f),
                //        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Elastic.EaseOut),
                //        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Width")),
                //        new TweenOption(TweenOptionType.EndValue, 400f)
                //        ),

                //    Tween.New()
                //        .SetProperty("Height")
                //        .SetOptions(
                //        new TweenOption(TweenOptionType.Duration, 1.3f),
                //        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Elastic.EaseOut),
                //        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                //        new TweenOption(TweenOptionType.EndValue, 400f)
                //        )
                //    ),

                // expand width to stage width
                Tween.New()
                    .SetProperty("Width")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Width")),
                        new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)delegate
                        {
                            return Bounds.width;
                        })),

                // expand height to stage height
                Tween.New()
                    .SetProperty("Height")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                        new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)delegate
                        {
                            return Bounds.height;
                        })),

                // tween alpha to 0.5
                Tween.New()
                    .SetProperty("Alpha")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 2f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Alpha")),
                        new TweenOption(TweenOptionType.EndValue, 0.5f)
                        ));
        }
    }
}