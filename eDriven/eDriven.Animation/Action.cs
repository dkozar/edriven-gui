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

using UnityEngine;

namespace eDriven.Animation
{
    public class Action : TweenBase
    {
#if DEBUG
        public static bool DebugMode;
#endif

        public Action(Tween.CallbackFunction callback)
        {
            Callback = callback;

            //CheckTarget = false;
            //CheckProperty = false;
            //CheckStartValue = false;
            //CheckEndValue = false;

            Delay = 0;
            Duration = 0;
        }

        //public Action()
        //{
        //    CheckTarget = false;
        //    CheckProperty = false;
        //    CheckStartValue = false;
        //    CheckEndValue = false;
        //}

        private bool _played;

        public override void Play()
        {
            if (_played)
                return;

            _played = true;

#if DEBUG
            if (DebugMode)
                Debug.Log("Action.Play");
#endif
            if (null != Callback)
                Callback(this);

            //if (IsChild)
            //{
            //    //Parent.FinishedCount++;
            //    Parent.Count--;
            //}

            //if (null != Callback)
            //    Callback(this);

            //if (DestroyOnFinish)
            //    Destroy();
        }

        public override bool Tick()
        {
            //Debug.Log("=== ACTION TICK ===");
            //if (IsChild)
            //{
            //    Play();
            //    //Parent.FinishedCount++;
            //    //Parent.Count--;
            //}
            Play();
            return true; // we are finished
        }

        public override string ToString()
        {
            string n = string.Empty;
            if (!string.IsNullOrEmpty(Name))
            {
                n = string.Format(@"""[{0}]"" ", Name);
            }

            string t = string.Format("Target:{0};", Target ?? "-");
            string c = string.Format(@"Callback:""{0}"";", Callback);

            return string.Format(@"Action{0} [{1}{2}]", n, t, c);
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