using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;

namespace Assets.eDriven.Demo.Tweens
{
    /// <summary>
    /// Something like this has to be used when wanting the fade out on a dialog
    /// Of course, it has to be disabled, so it cannot be clickable
    /// </summary>
    public class DialogFadeOut : Parallel
    {
        private static object GetStartValue(object target)
        {
            float a = 0;
            DisplayObject displayObject = target as DisplayObject;
            if (null != displayObject)
                a = displayObject.Alpha;

            return a;
        }

        public DialogFadeOut()
        {
            Add(

                Tween.New()
                    .SetProperty("Alpha")
                    .SetOptions(
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                    new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction)GetStartValue),
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