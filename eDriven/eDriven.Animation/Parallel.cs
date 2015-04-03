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

using System.Collections.Generic;
using UnityEngine;

namespace eDriven.Animation
{
    public class Parallel : Composite
    {
#if DEBUG
        public new static bool DebugMode;
#endif

        public Parallel()
        {
            CheckTarget = false;
            CheckProperty = false;
            CheckStartValue = false;
            CheckEndValue = false;
        }

        public Parallel(params TweenBase[] tweens)
            : base(tweens)
        {
            CheckTarget = false;
            CheckProperty = false;
            CheckStartValue = false;
            CheckEndValue = false;
        }

        public override void Play()
        {
            base.Play();

//            if (0 == Tweens.Count)
//            {
//#if DEBUG
//                if (DebugMode)
//                    Debug.Log("Parallel: No tweens defined");
//#endif
//                return;
//            } 
            
//#if DEBUG
//            if (DebugMode)
//                Debug.Log("*** Parallel PLAY *** " + this);
//#endif

            //Debug.Log("Tweens.Count: " + Tweens.Count);

            //Tweens.ForEach(delegate(ITween tween)
            //                   {
            //                       Debug.Log("### tween.StartValue: " + tween.StartValue);
            //                   });

//            base.Play();
        }

        protected override void Launch()
        {
            if (0 == Children.Count)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("Parallel: No tweens defined");
#endif
                return;
            }

#if DEBUG
            if (DebugMode)
                Debug.Log("*** Parallel Launch *** " + this);
#endif

            // play all tweens in parallel
            Children.ForEach(delegate(IAsyncAction action)
            {
                //Debug.Log("### " + tween.StartValue);

                TweenBase tb = action as TweenBase;
                if (null != tb) {
                    tb.Delay += Delay;
                    if (null != Target && null == tb.Target)
                        tb.Play(Target);
                    else
                        tb.Play();
                }
            });
        }

        private readonly List<IAsyncAction> _finished = new List<IAsyncAction>();

        public override bool Tick()
        {
            base.Tick();

            //Debug.Log("*** Parallel Tick *** " + this);
            
            //if (!IsLaunched)
            //    Launch();

            if (!IsLaunched)
                return false;

            Children.ForEach(delegate(IAsyncAction tween)
            {
                //Tween t = tween as Tween;
                //if (null != t)
                //    Debug.Log("   Tween.StartValue -> " + t.StartValue);

                if (!_finished.Contains(tween) && tween.Tick())
                {
                    //FinishedCount++;
                    _finished.Add(tween);
                    Count--;
                }
            });

            //Tweens.RemoveAll(delegate(IAsyncAction action)
            //                     {
            //                         return _finished.Contains(action);
            //                     });

            ////_finished.Clear();

            return Count == 0;
            
            // this was the last one, return true
            //if (FinishedCount > Tweens.Count)
            //    return true;

            //return false;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            Parallel parallel = new Parallel { Name = Name }; //(Parallel)MemberwiseClone();
            parallel.Add(CloneTweens().ToArray());
            return parallel;
        }

        public override string ToString()
        {
            return string.Format("Parallel {0}{1}[Delay: {2}]", string.IsNullOrEmpty(Name) ? string.Empty : string.Format(@"""{0}"" ", Name), base.ToString(), Delay);
        }

        public override void Reset()
        {
            base.Reset();

            _finished.Clear();
        }
    }
}