using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;

namespace Assets.eDriven.Demo.Tweens
{
    /// <summary>
    /// This one moves the dialog off screen
    /// </summary>
    public class DialogFallDown : Parallel
    {
        private static object GetStartValue(object target)
        {
            float y = 0;
            DisplayObject displayObject = target as DisplayObject;
            if (null != displayObject)
                y = -displayObject.Height;

            return y;
        }

        private static object GetEndValue(object target)
        {
            var y = SystemManager.Instance.ScreenSize.Y;

            DisplayObject displayObject = target as DisplayObject;

            if (null != displayObject)
                y -= displayObject.Height;

            return y / 2; 
        }

        public DialogFallDown()
        {
            Add(
                Tween.New()
                    .SetProperty("Y")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Bounce.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction) GetStartValue),
                        new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction) GetEndValue)
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