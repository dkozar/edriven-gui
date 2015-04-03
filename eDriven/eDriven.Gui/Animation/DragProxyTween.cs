#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Geom;

namespace eDriven.Gui.Animation
{
    public class DragProxyTween : Parallel
    {
        public static float DefaultDuration = 0.6f;
        public static Tween.EasingFunction DefaultEaser = Expo.EaseOut;

        public Rectangle OldBounds;
        public Rectangle NewBounds;

        public DragProxyTween(object target, Rectangle oldBounds, Rectangle newBounds)
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
                new TweenOption(TweenOptionType.EndValue, NewBounds.Width)
                //new TweenOption(TweenOptionType.Proxy, new SetActualWidthProxy(Target))
            ));

            Add(Tween.New().SetOptions(
                new TweenOption(TweenOptionType.Property, "Height"),
                new TweenOption(TweenOptionType.Duration, DefaultDuration),
                new TweenOption(TweenOptionType.Easer, DefaultEaser),
                new TweenOption(TweenOptionType.StartValue, OldBounds.Height),
                new TweenOption(TweenOptionType.EndValue, NewBounds.Height)
                //new TweenOption(TweenOptionType.Proxy, new SetActualHeightProxy(Target))
            ));
            
            /*Property = "Bounds";
            //Interpolator = new RectangleInterpolator();
            Duration = DefaultDuration;
            Easer = DefaultEaser;
            StartValueReader = new PropertyReader("Bounds");*/
        }
    }
}