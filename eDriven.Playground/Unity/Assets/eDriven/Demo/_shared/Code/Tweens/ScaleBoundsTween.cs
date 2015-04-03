using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Geom;
using eDriven.Gui.Animation.Proxies;

namespace Assets.eDriven.Demo._shared.Code.Tweens
{
    /// <summary>
    /// A tween scaling the component bounds<br/>
    /// Usefull for moving/resizing windows and dialogs 
    /// </summary>
    public class ScaleBoundsTween : Parallel
    {
        public static float DefaultDuration = 0.6f;
        public static Tween.EasingFunction DefaultEaser = Expo.EaseOut;

        public Rectangle OldBounds;
        public Rectangle NewBounds;

        public ScaleBoundsTween(object target, Rectangle oldBounds, Rectangle newBounds)
        {
            Target = target;
            OldBounds = oldBounds;
            NewBounds = newBounds;

            //Duration = DefaultDuration;
            //Easer = DefaultEaser;

            Add(Tween.New().SetOptions(
                new TweenOption(TweenOptionType.Property, "X"),
                new TweenOption(TweenOptionType.Duration, DefaultDuration),
                new TweenOption(TweenOptionType.Easer, DefaultEaser),
                new TweenOption(TweenOptionType.StartValue, OldBounds.X),
                new TweenOption(TweenOptionType.EndValue, NewBounds.X)
            ));

            Add(Tween.New().SetOptions(
                new TweenOption(TweenOptionType.Property, "Y"),
                new TweenOption(TweenOptionType.Duration, DefaultDuration),
                new TweenOption(TweenOptionType.Easer, DefaultEaser),
                new TweenOption(TweenOptionType.StartValue, OldBounds.Y),
                new TweenOption(TweenOptionType.EndValue, NewBounds.Y)
            ));

            Add(Tween.New().SetOptions(
                new TweenOption(TweenOptionType.Property, "Width"),
                new TweenOption(TweenOptionType.Duration, DefaultDuration),
                new TweenOption(TweenOptionType.Easer, DefaultEaser),
                new TweenOption(TweenOptionType.StartValue, OldBounds.Width),
                new TweenOption(TweenOptionType.EndValue, NewBounds.Width)/*,
                new TweenOption(TweenOptionType.Proxy, new SetActualWidthProxy(target))*/
            ));

            Add(Tween.New().SetOptions(
                new TweenOption(TweenOptionType.Property, "Height"),
                new TweenOption(TweenOptionType.Duration, DefaultDuration),
                new TweenOption(TweenOptionType.Easer, DefaultEaser),
                new TweenOption(TweenOptionType.StartValue, OldBounds.Height),
                new TweenOption(TweenOptionType.EndValue, NewBounds.Height)/*,
                new TweenOption(TweenOptionType.Proxy, new SetActualHeightProxy(target))*/
            ));
        }
    }
}