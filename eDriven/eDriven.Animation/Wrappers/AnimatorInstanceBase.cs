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

namespace eDriven.Animation.Wrappers
{
    /// <summary>
    /// Animation instance is a wrapper around the basic tween
    /// It is an adapter for a specific properties
    /// </summary>
    public abstract class AnimatorInstanceBase : IAnimation
    {
        public object Target { get; set; }
        
        public string Property { get; set; }
        
        public float Delay { get; set; }

        public float Duration { get; set; }

        /// <summary>
        /// Just a descriptive name for logging etc.
        /// </summary>
        public string Name { get; set; }

        private Tween.EasingFunction _easer = Easing.Expo.EaseOut; // default easer
        public Tween.EasingFunction Easer
        {
            get
            {
                return _easer;
            } 
            set
            {
                _easer = value;
            }
        }

        private IAsyncAction _tween;
        public IAsyncAction Tween
        {
            get { return _tween; }
            set { _tween = value; }
        }

        public abstract void Configure(object target);

        public void Play(object target)
        {
            Configure(target);

            TweenBase tb = _tween as TweenBase;
            Tween t = _tween as Tween;
            
            if (null != t)
                t.Easer = Easer;

            if (null != tb)
                tb.Play(target);
        }

        public void Play(object target, bool registerToTweenManager)
        {
            Play(target);
        }

        public void Stop()
        {
            Dispose();
        }

        public bool Tick()
        {
            return _tween.Tick();
        }

        public Tween.CallbackFunction Callback { get; set; }

        public abstract object Clone();

        public void Dispose()
        {
            IDisposable disp = _tween as IDisposable;
            if (null != disp)
                disp.Dispose();

            _tween = null;
        }
    }
}