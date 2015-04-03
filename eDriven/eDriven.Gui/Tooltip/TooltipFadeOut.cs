using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.Tooltip
{
    public class TooltipFadeOut : Tween
    {
        private static object GetStartValue(object target)
        {
            float a = 0;
            DisplayObject displayObject = target as DisplayObject;
            
            if (null != displayObject)
                a = -displayObject.Alpha;
            else
                Debug.LogWarning(target + " is not a DisplayObject");

            return a;
        }

        public TooltipFadeOut()
        {
            SetProperty("Alpha");
            SetOptions(
                new TweenOption(TweenOptionType.Duration, 0.7f),
                new TweenOption(TweenOptionType.Easer, (EasingFunction) Linear.EaseNone),
                new TweenOption(TweenOptionType.StartValueReaderFunction, (PropertyReaderFunction) GetStartValue),
                new TweenOption(TweenOptionType.EndValue, 0f)
            );

            Callback = delegate
                           {
                               var displayObject = Target as DisplayObject;
                               if (null != displayObject){
                                   displayObject.Alpha = 0f;
                                   displayObject.Visible = false;
                               }
                           };
        }
    }
}