using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using eDriven.Audio;
using eDriven.Core.Geom;
using eDriven.Gui.Containers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;


namespace Assets.eDriven.Demo.Tweens
{
    public class DialogResizeUp : Sequence
    {
        public static readonly Dictionary<Component, Rectangle> DialogSizes = new Dictionary<Component, Rectangle>();

        private static object EndValueReaderFunc(object target)
        {
            //return new Rectangle(0, 0, Screen.width, Screen.height);
            Dialog dialog = (Dialog) target;

            if (!DialogSizes.ContainsKey(dialog)) {
                DialogSizes[dialog] = (Rectangle)dialog.Bounds.Clone();
                return new Rectangle(0, 0, Screen.width, Screen.height);
            }
            
            var bounds = DialogSizes[dialog];
            DialogSizes.Remove(dialog);
            return bounds;
        }
        private static object StartValueReaderFunc(object target)
        {
            return target.GetType().GetProperty("Bounds").GetValue(target, new object[] { });
        }

        public DialogResizeUp()
        {
            Add(
                new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("pager_click"); }),
                new Tween
                {
                    Property = "Bounds",
                    Interpolator = new RectangleInterpolator(),
                    Duration = 0.4f,
                    Easer = Quad.EaseInOut,
                    EndValueReaderFunction = EndValueReaderFunc,
                    StartValueReaderFunction = StartValueReaderFunc
                }
            );
        }
    }
}