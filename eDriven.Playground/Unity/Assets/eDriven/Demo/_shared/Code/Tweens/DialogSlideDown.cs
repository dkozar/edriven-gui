using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;

namespace Assets.eDriven.Demo.Tweens
{
    public class DialogSlideDown : Parallel
    {
        private static object GetStartValue(object target)
        {
            var y = SystemManager.Instance.ScreenSize.Y;

            DisplayObject displayObject = target as DisplayObject;

            if (null != displayObject)
                y -= displayObject.Height;

            return y / 2; 
        }

        private static object GetEndValue(object target)
        {
            return SystemManager.Instance.ScreenSize.Y;
        }

        public DialogSlideDown()
        {
            Add(
                Tween.New()
                    .SetProperty("Y")
                    .SetOptions(
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Bounce.EaseOut),
                    new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction) GetStartValue),
                    new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction) GetEndValue)
                    ),

                // tween alpha to 0.4
                Tween.New()
                    .SetProperty("Alpha")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseIn),
                        new TweenOption(TweenOptionType.StartValue, 1f),
                        new TweenOption(TweenOptionType.EndValue, 0f)
                    )
            );

            Callback = delegate
                           {
                               var displayObject = Target as DisplayObject;
                               if (null != displayObject)
                                   PopupManager.Instance.RemovePopup(displayObject);
                           };
        }
    }
}