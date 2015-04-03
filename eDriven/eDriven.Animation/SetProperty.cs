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

using eDriven.Core.Reflection;
using UnityEngine;

namespace eDriven.Animation
{
    public class SetProperty : TweenBase
    {
#if DEBUG
        public static bool DebugMode;
#endif
        public object Value;

        public bool Relative;

        public SetProperty(string property, object value)
        {
            Property = property;
            Value = value;

            //CheckTarget = false;
            //CheckProperty = false;
            //CheckStartValue = false;
            //CheckEndValue = false;
        }

        // TODO: Relative
        public SetProperty(string property, object value, bool relative)
        {
            Property = property;
            Value = value;
            Relative = relative;

            //CheckTarget = false;
            //CheckProperty = false;
            //CheckStartValue = false;
            //CheckEndValue = false;
        }

        public override void Play()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("SetProperty.Play {0}, {1}, {2}", Target, Property, Value));
#endif
            CoreReflector.SetValue(Target, Property, Value);

            //if (IsChild)
            //{
            //    //Parent.FinishedCount++;
            //    Parent.Count--;
            //}

            //if (DestroyOnFinish)
            //    Destroy();
        }

        private bool _played;

        public override void Play(object target)
        {
            if (_played)
                return;

            _played = true;

#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("SetProperty.Play {0}, {1}, {2}", Target, Property, Value));
#endif
            //object val;
            //if (Relative)
            //    val = ReflectionUtil.GetValue(Target, Property) + Value;
            //else
            //    val = Value;
            CoreReflector.SetValue(Target, Property, Value);

            //if (IsChild)
            //{
            //    //Parent.FinishedCount++;
            //    Parent.Count--;
            //}
        }

        public override bool Tick()
        {
            Play();
            return true; // we are finished
        }

        //public override bool Tick()
        //{
        //    return true;
        //}

        //protected override void ApplyValue()
        //{
        //    // do not tween anything!
        //}

        public override string ToString()
        {
            string n = string.Empty;
            if (!string.IsNullOrEmpty(Name))
            {
                n = string.Format(@"""[{0}]"" ", Name);
            }

            string t = string.Format("Target:{0};", Target ?? "-");
            string p = string.Format(@"Property:""{0}"";", Property ?? "-");
            string v = string.Format(@"Value:{0};", Value ?? "-");

            return string.Format(@"SetProperty {0}[{1}{2}{3}]", n, t, p, v);
        }

        public override object Clone()
        {
            return new SetProperty(Property, Value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            
        }
    }
}