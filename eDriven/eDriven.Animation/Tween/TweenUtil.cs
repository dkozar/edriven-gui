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

using System;
using eDriven.Animation.Interpolators;
using eDriven.Core.Reflection;
using UnityEngine;

namespace eDriven.Animation
{
    public class TweenUtil
    {
#if DEBUG
        public static bool DebugMode;
#endif

        private static int _count;
        public static string CreateUniqueName()
        {
            _count++;

            //Debug.Log("CreateUniqueName: " + type);

            return string.Format("{0}_{1}", "Tween", _count);
        }

        public static void InitializeTween(ITween animation)
        {
            animation.Target = null;
            animation.Property = null;
            animation.Parent = null;
            animation.Interpolator = new FloatInterpolator();
            animation.StartValue = null;
            animation.EndValue = null;
            animation.StartValueReader = null;
            animation.EndValueReader = null;
            animation.IsDone = false;
            animation.Name = CreateUniqueName();
        }

        internal static void ApplyOptions(TweenBase animation, TweenOption[] options)
        {
            //Debug.Log("ApplyOptions");

            if (options == null) return;

            ITween tween = animation as ITween;

            int len = options.Length;
            for (int i = 0; i < len; i++)
            {
                TweenOption option = options[i];

#if DEBUG
                if (DebugMode)
                    Debug.Log("Option: " + option);
#endif

                switch (option.Type)
                {
                    case TweenOptionType.Target:
                        animation.Target = option.Value;
                        break;

                    case TweenOptionType.Property:
                        animation.Property = (string)option.Value;
                        break;

                    case TweenOptionType.Duration:
                        animation.Duration = (float)option.Value;
                        break;

                    case TweenOptionType.Easer:
                        if (null != tween)
                            tween.Easer = (Tween.EasingFunction)option.Value;
                        break;

                    case TweenOptionType.Interpolator:
                        if (null != tween)
                            tween.Interpolator = (IInterpolator)option.Value;
                        break;

                    case TweenOptionType.StartValue:
                        if (null != tween)
                            tween.StartValue = option.Value;
                        break;

                    case TweenOptionType.StartValueReader:
                        if (null != tween)
                            tween.StartValueReader = (PropertyReader)option.Value;
                        break;

                    case TweenOptionType.StartValueReaderFunction:
                        if (null != tween)
                            tween.StartValueReaderFunction = (Tween.PropertyReaderFunction)option.Value;
                        break;

                    case TweenOptionType.EndValue:
                        if (null != tween)
                            tween.EndValue = option.Value;
                        break;

                    case TweenOptionType.EndValueReader:
                        if (null != tween)
                            tween.EndValueReader = (PropertyReader)option.Value;
                        break;

                    case TweenOptionType.EndValueReaderFunction:
                        if (null != tween)
                            tween.EndValueReaderFunction = (Tween.PropertyReaderFunction)option.Value;
                        break;

                    case TweenOptionType.Callback:
                        animation.Callback = (Tween.CallbackFunction)option.Value;
                        break;

                    case TweenOptionType.Proxy:
                        animation.Proxy = (ISetterProxy)option.Value;
                        break;

                    default:
                        throw new Exception(string.Format(@"Unknown tween option type ""{0}""", option.Value));
                }
            }
        }
    }
}