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
using System.Collections.Generic;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
#if RELEASE
using eDriven.Core;
using eDriven.Animation.Check;
#else
using System.Text;
#endif
using UnityEngine;

namespace eDriven.Animation
{
    public abstract class Composite : TweenBase, ICompositeAsyncAction
    {
#if DEBUG
        public static bool DebugMode;
#endif

        public IComposite Parent { get; set; }

        private readonly List<IAsyncAction> _children = new List<IAsyncAction>();
        public List<IAsyncAction> Children
        {
            get { return _children; }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        protected Composite()
        {
            //_startTime = DateTime.Now;
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="tweens"></param>
        public Composite(params IAsyncAction[] tweens)
        {
            foreach (IAsyncAction tween in tweens)
            {
                Add(tween);
            }
        }

        protected Composite(IEnumerable<IAsyncAction> tweens)
        {
            foreach (IAsyncAction tween in tweens)
            {
                Add(tween);
            }
        }

        internal void InitializeChildren()
        {

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Initializing composite tween with {0} children", _children.Count));
            }
#endif
            _count = _children.Count;

#if RELEASE
            /* RELEASE HACK CHECK */
            Acme2 acme = (Acme2)Framework.GetComponent<Acme2>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy || !acme.enabled)
                return;
#endif
            foreach (IAsyncAction child in _children)
            {
                InitializeChild(child);

                Composite composite = child as Composite;
                if (null != composite)
                    composite.InitializeChildren();
            }
        }

        private void InitializeChild(IAsyncAction action)
        {
            action.Parent = this;
            
            TweenBase tb = action as TweenBase;
            if (null == tb)
                return;

            //Debug.Log("Initalizing: " + action);
            //Debug.Log("Target: " + Target);
            
            if (null == tb.Target)
                tb.Target = Target;
            if (null == tb.Property)
                tb.Property = Property;

            //Debug.Log("-- tb.Target: " + tb.Target);
        }

        private int _count;
        /// <summary>
        /// Child tween count
        /// Upon the composite tween's child initialization, this number is increased
        /// The duty of each child tween is to decrease this count when finished, using the internal setter
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value == _count)
                    return;

                _count = value;

                if (_count <= 0)
                {
                    //if (IsChild)
                    //    Parent.Count--;

                    IsDone = true;
                    //Debug.Log("IS DONE ***");
                    OnFinished();
                }
            }
        }

        #region Options

        private Tween.EasingFunction _easer = Linear.EaseNone;
        public Tween.EasingFunction Easer
        {
            get { return _easer; }
            set
            {
                if (null == value)
                    throw new Exception("Cannot add null as easer");

                _easer = value;
            }
        }

        private IInterpolator _interpolator = new FloatInterpolator();
        public IInterpolator Interpolator
        {
            get { return _interpolator; }
            set
            {
                if (null == value)
                    throw new Exception("Cannot add null as interpolator");

                _interpolator = value;
            }
        }

        private object _startValue;
        public object StartValue
        {
            get { return _startValue; }
            set { _startValue = value; }
        }

        private object _endValue;
        public object EndValue
        {
            get { return _endValue; }
            set { _endValue = value; }
        }

        public PropertyReader StartValueReader { get; set; }
        public PropertyReader EndValueReader { get; set; }

        public Tween.PropertyReaderFunction StartValueReaderFunction { get; set; }
        public Tween.PropertyReaderFunction EndValueReaderFunction { get; set; }

        #endregion

        public void Add(IAsyncAction tween)
        {
            if (null == tween)
                throw new Exception("Cannot add null as an action");

            _children.Add(tween);
        }

        public void Add(params IAsyncAction[] tweens)
        {
            foreach (IAsyncAction tween in tweens)
            {
                //Debug.Log("Adding: " + tween);
                Add(tween);
                //Debug.Log("tween.Parent: " + tween.Parent);
            }
        }

        /// <summary>
        /// Removes the action
        /// </summary>
        /// <param name="action"></param>
        public void Remove(IAsyncAction action)
        {
            throw new NotImplementedException();
        }
        
        public override void Dispose()
        {
            _children.ForEach(delegate(IAsyncAction animation)
                                {
                                    IDisposable disp = animation as IDisposable;
                                    if (null != disp)
                                        disp.Dispose();
                                });
            _children.Clear();
        }

        public override void Play()
        {
            /**
             * Note: initializing children here, not before!
             * That's because in the early stages there is no target of composite effect set yet
             * */
            InitializeChildren();

            base.Play();

            _startTime = DateTime.Now;

            TweenRegistry.Instance.Add(this);
        }

        public override void Play(object target)
        {
            //Debug.Log("Composite Play: " + target);
            Target = target;
            Play();
        }
        
        private bool _isLaunched;
        public bool IsLaunched
        {
            get
            {
                return _isLaunched;
            }
            set
            {

            }
        }

        private DateTime _startTime;

        public override bool Tick()
        {
            //Debug.Log("Composite tick: " + this);

            if (IsDone)
                return true;

            float currentTime = (float)(DateTime.Now - _startTime).TotalSeconds;

            if (!_isLaunched)
            {
                if (currentTime < Delay)
                    return false;

                //Debug.Log(string.Format("Launching: {0} [Delay: {1}]", this, Delay));

                _startTime = DateTime.Now;

                currentTime = 0;

                Launch();

                _isLaunched = true;
            }

            return false;
        }

        protected abstract void Launch();

        public override void Stop()
        {
            _children.ForEach(delegate(IAsyncAction action)
            {
                ITween tween = action as ITween;
                if (null != tween)
                    tween.Stop();
            });

            IsDone = true;

            TweenRegistry.Instance.Remove(this);
        }

        public virtual void Reset()
        {
            IsDone = false;

            _children.ForEach(delegate(IAsyncAction action)
                                {
                                    ITween tween = action as ITween;
                                    if (null != tween)
                                        tween.Stop();
                                });

            _count = _children.Count;
            //Callback.Dispose(); // TODO
            //Callback = null;

            TweenRegistry.Instance.Remove(this);
        }

        protected virtual void OnFinished()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("-> OnFinished called");
#endif

            //if (null != Callback) // NOTE: This gets duplicated because is fired by TweenRegisty
            //    Callback(this);

            TweenRegistry.Instance.Remove(this);

            IsDone = true;

            // override in subclass
        }

        #region Implementation of ICloneable

        protected List<IAsyncAction> CloneTweens()
        {
            var tweens = new List<IAsyncAction>();

            _children.ForEach(delegate(IAsyncAction tween)
            {
                tweens.Add((IAsyncAction)tween.Clone());
            });

            return tweens;
        }

//        /// <summary>
//        /// Creates a new object that is a copy of the current instance.
//        /// </summary>
//        /// <returns>
//        /// A new object that is a copy of this instance.
//        /// </returns>
//        /// <filterpriority>2</filterpriority>
//        public abstract object Clone();
////        {
////            //Composite c = (Composite)MemberwiseClone();
////            //Debug.Log("Cloning: " + c);

////            Composite c = new Composite(CloneTweens().ToArray())
////                              {
////                                  /*Property = Property,
////                                  Target = Target*/
////                              };

////#if DEBUG
////            if (DebugMode)
////            {
////                Debug.Log(string.Format(@"Composite cloned: 
////{0}", this));
////            }
////#endif


////            //c.Tweens.Clear();

////            //foreach (ITween tween in _tweens)
////            //_tweens.ForEach(delegate(ITween tween)
////            //                    {
////            //                        c.Tweens.Add((ITween) tween.Clone());
////            //                    });
////            return c;
////        }

        #endregion

        public override string ToString()
        {
            string t = string.Empty;
            if (null != Target){
                t = string.Format("on {0}; ", Target);
            }

            string d = string.Empty;

#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder();
                int count = 0;
                _children.ForEach(delegate(IAsyncAction tween)
                {
                    count++;
                    sb.AppendFormat(@"   [{0}] {1}
", count, tween);
                });

                d = string.Format(@"
---->
{0}", sb);
            }
#endif

            return string.Format("[{0} Tweens to go: {1}/{2}]{3}", t, Count, Children.Count, d);
        }
    }
}