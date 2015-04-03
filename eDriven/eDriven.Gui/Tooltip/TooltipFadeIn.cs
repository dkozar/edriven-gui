using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;

namespace eDriven.Gui.Tooltip
{
    public class TooltipFadeIn : Tween
    {
        private static object GetStartValue(object target)
        {
            float a = 0;
            DisplayObject displayObject = target as DisplayObject;
            if (null != displayObject)
                a = displayObject.Alpha;

            return a;
        }

        public TooltipFadeIn()
        {
            SetProperty("Alpha");
            SetOptions(
                new TweenOption(TweenOptionType.Duration, 0.7f),
                new TweenOption(TweenOptionType.Easer, (EasingFunction) Linear.EaseNone),
                new TweenOption(TweenOptionType.StartValueReaderFunction, (PropertyReaderFunction) GetStartValue),
                new TweenOption(TweenOptionType.EndValue, 1f)
                );

            Callback = delegate
                           {
                               var displayObject = Target as DisplayObject;
                               if (null != displayObject)
                                    displayObject.Alpha = 1f;
                           };
        }
    }
}