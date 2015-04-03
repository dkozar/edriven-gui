using eDriven.Animation;
using eDriven.Animation.Easing;
using UnityEngine;

public class CollapseAlphaLeftTop : Sequence
{
    public Rect Bounds;

    public CollapseAlphaLeftTop()
    {
        Add(

            // tween alpha to 1.0
            Tween.New()
                .SetProperty("Alpha")
                .SetOptions(
                new TweenOption(TweenOptionType.Duration, 1f),
                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Alpha")),
                new TweenOption(TweenOptionType.EndValue, 1f)
            ),

            // expand width to stage width
            Tween.New()
                .SetProperty("Width")
                .SetOptions(
                new TweenOption(TweenOptionType.Duration, 1f),
                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Width")),
                new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)delegate {
                        return Bounds.width;
                    })
                ),

            // expand height to stage height
            Tween.New()
                .SetProperty("Height")
                .SetOptions(
                new TweenOption(TweenOptionType.Duration, 1f),
                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Bounce.EaseOut),
                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)delegate {
                      return Bounds.height;
                  })
                )
            );
    }
}