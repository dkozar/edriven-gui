using eDriven.Animation;
using eDriven.Audio;
using eDriven.Gui.Components;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace Assets.eDriven.Demo.Tweens
{
    public class ToolbarShow : Sequence
    {
        private static object GetStartValue(object target)
        {
            float y = 0;
            DisplayObject displayObject = target as DisplayObject;
            if (null != displayObject)
                y = -displayObject.Height;
            else
                Debug.LogWarning(target + " is not a DisplayObject");

            //return -50;

            return y;
        }

        private static object GetEndValue(object target)
        {
            return 0;
        }

        private static Group GetParent(object target)
        {
            Component component = (Component)target;
            return ((Group)component.Parent);
        }

        public ToolbarShow()
        {
            Add(

                new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
                new Action(delegate(IAnimation anim)
                {
                    GetParent(anim.Target).AutoLayout = false;
                }),

                /*new Tween
                {
                    Property = "X",
                    Duration = 2f,
                    Easer = Linear.EaseNone,
                    StartValue = 0f,
                    EndValue = 100f
                },*/

                // BUG!?

                /*new Tween
                {
                    Property = "Y",
                    Duration = 2f,
                    Easer = Linear.EaseNone,
                    EndValue = 50f
                }.SetOptions(
                    new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction)GetStartValue),
                    new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)GetEndValue)
                ),*/

                /*Tween.New()
                    .SetProperty("Y")
                    .SetOptions(
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Sine.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReaderFunction, (Tween.PropertyReaderFunction)GetStartValue),
                        new TweenOption(TweenOptionType.EndValueReaderFunction, (Tween.PropertyReaderFunction)GetEndValue)),*/

                new Action(delegate(IAnimation anim)
                {
                    GetParent(anim.Target).AutoLayout = true;
                }),
                
                new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); })
            );
        }
    }
}