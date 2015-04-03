using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace Assets.eDriven.Demo.Tweens
{
    public class DialogSlideUp : Sequence
    {
        private static object GetStartValue(object target)
        {
            return SystemManager.Instance.ScreenSize.Y;
        }

        private static object GetEndValue(object target)
        {
            var y = SystemManager.Instance.ScreenSize.Y;

            DisplayObject displayObject = target as DisplayObject;
            if (null != displayObject)
                y -= displayObject.Height;
            else
                Debug.LogWarning(target + " is not a DisplayObject");

            return y / 2;
        }

        private static Group GetParent(object target)
        {
            Component component = (Component)target;
            return ((Group)component.Parent);
        }

        public DialogSlideUp()
        {
            Add(

                new Action(delegate(IAnimation anim)
                {
                    GetParent(anim.Target).AutoLayout = false;
                }),

                new SetProperty("Y", Screen.height),

                new Parallel(
                    Tween.New()
                        .SetProperty("Y")
                        .SetOptions(
                            new TweenOption(TweenOptionType.Duration, 0.5f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction) GetStartValue),
                            new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction) GetEndValue)
                        ),

                    Tween.New()
                        .SetProperty("Alpha")
                        .SetOptions(
                            new TweenOption(TweenOptionType.Duration, 0.5f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValue, 0f),
                            new TweenOption(TweenOptionType.EndValue, 1f)
                    )    
                ),

                new Action(delegate(IAnimation anim)
                {
                    GetParent(anim.Target).AutoLayout = true;
                })
            );
                
        }
    }
}